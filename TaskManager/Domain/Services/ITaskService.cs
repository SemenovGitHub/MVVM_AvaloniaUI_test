using TaskManager.Domain.Models;
using TaskManager.Domain.ServiceBase;

namespace TaskManager.Domain.Services;

public interface ITaskService : IServiceBase<TaskItem>
{
    Task<TaskItem> SetCompletionAsync(Guid id, bool isCompleted, CancellationToken cancellationToken);
}
