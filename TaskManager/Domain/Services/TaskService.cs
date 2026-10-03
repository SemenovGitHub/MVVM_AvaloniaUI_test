using AutoMapper;
using FluentValidation;
using TaskManager.Domain.Models;
using TaskManager.Domain.ServiceBase;
using TaskManager.Infrastructure.Persistence.Entities;
using TaskManager.Infrastructure.Persistence.Repositories;

namespace TaskManager.Domain.Services;

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
