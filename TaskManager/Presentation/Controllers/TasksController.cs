using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Models;
using TaskManager.Domain.Services;
using TaskManager.Presentation.Contracts;

namespace TaskManager.Presentation.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;
    private readonly IMapper _mapper;

    public TasksController(ITaskService tasks, IMapper mapper)
    {
        _tasks = tasks;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var tasks = await _tasks.GetAllAsync(cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<TaskResponse>>(tasks));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var task = await _tasks.GetByIdAsync(id, cancellationToken);
        return Ok(_mapper.Map<TaskResponse>(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = _mapper.Map<TaskItem>(request);
        var created = await _tasks.CreateAsync(task, cancellationToken);
        var response = _mapper.Map<TaskResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPatch("{id:guid}/completion")]
    public async Task<ActionResult<TaskResponse>> SetCompletion(
        Guid id,
        UpdateCompletionRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _tasks.SetCompletionAsync(id, request.IsCompleted, cancellationToken);
        return Ok(_mapper.Map<TaskResponse>(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _tasks.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
