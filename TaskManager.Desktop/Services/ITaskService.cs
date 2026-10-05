using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Services;

public interface ITaskService : IServiceBase<TaskModel>
{
    Task<TaskModel> SetCompletionAsync(Guid id, bool isCompleted, CancellationToken cancellationToken);
}
