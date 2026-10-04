using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Services;

public interface ITaskService : IServiceBase<TaskItem>
{
    Task<TaskItem> SetCompletionAsync(Guid id, bool isCompleted, CancellationToken cancellationToken);
}
