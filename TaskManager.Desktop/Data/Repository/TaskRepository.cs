using TaskManager.Desktop.Data.Entities;
using TaskManager.Desktop.Data.Repository.RepositoryBase;

namespace TaskManager.Desktop.Data.Repository;

public sealed class TaskRepository : RepositoryBase<TaskEntity>, ITaskRepository
{
    public TaskRepository(TaskDbContext context) : base(context)
    {
    }

    protected override string NotFoundMessage => "Задача не найдена.";

    public Task<TaskEntity> SetCompletionAsync(Guid id, bool isCompleted, CancellationToken cancellationToken)
    {
        return ExecuteInTransactionAsync(
            async (token) =>
            {
                var entity = await GetByIdAsync(id, token);
                entity.IsCompleted = isCompleted;
                return entity;
            },
            cancellationToken);
    }
}