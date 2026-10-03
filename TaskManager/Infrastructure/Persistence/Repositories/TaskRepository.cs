using TaskManager.Infrastructure.Persistence.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository : RepositoryBase<TaskItemEntity>, ITaskRepository
{
    public TaskRepository(TaskDbContext context) : base(context)
    {
    }

    protected override string NotFoundMessage => "Задача не найдена.";

    public Task<TaskItemEntity> SetCompletionAsync(Guid id, bool isCompleted, CancellationToken cancellationToken)
    {
        return ExecuteInTransactionAsync(
            async () =>
            {
                var entity = await GetByIdAsync(id, cancellationToken);
                entity.IsCompleted = isCompleted;
                return entity;
            },
            cancellationToken);
    }
}
