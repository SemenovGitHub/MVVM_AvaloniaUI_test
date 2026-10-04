using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Desktop.Data.Entities;
using TaskManager.Desktop.Data.Repository;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Services;

public sealed class TaskService : ServiceBase<TaskItem, TaskItemEntity, ITaskRepository>, ITaskService
{
    public TaskService(
        ITaskRepository repository,
        IValidator<TaskItem> validator,
        IMapper mapper,
        ILogger<TaskService> logger)
        : base(repository, validator, mapper, logger)
    {
    }

    public async Task<TaskItem> SetCompletionAsync(Guid id, bool isCompleted, CancellationToken cancellationToken)
    {
        var entity = await Repository.SetCompletionAsync(id, isCompleted, cancellationToken);

        Logger.LogInformation(
            "Задача {TaskId} отмечена как {CompletionState}",
            entity.Id,
            isCompleted ? "выполненная" : "невыполненная");

        return Mapper.Map<TaskItem>(entity);
    }
}
