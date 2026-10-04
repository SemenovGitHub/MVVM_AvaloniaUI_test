using TaskManager.Desktop.Data.Entities;
using TaskManager.Desktop.Data.Repository.RepositoryBase;

namespace TaskManager.Desktop.Data.Repository;

public interface ITaskRepository : IRepositoryBase<TaskEntity>
{
    Task<TaskEntity> SetCompletionAsync(Guid id, bool isCompleted, CancellationToken cancellationToken);
}
