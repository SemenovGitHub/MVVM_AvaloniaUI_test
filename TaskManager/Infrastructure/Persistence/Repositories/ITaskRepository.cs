using TaskManager.Infrastructure.Persistence.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public interface ITaskRepository : IRepositoryBase<TaskItemEntity>
{
    Task<TaskItemEntity> SetCompletionAsync(Guid id, bool isCompleted, CancellationToken cancellationToken);
}
