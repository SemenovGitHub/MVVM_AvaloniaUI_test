using TaskManager.Desktop.Data.Entities;
using TaskManager.Desktop.Models;

namespace TaskManagerTests.Services;

public static class TaskData
{
    public static readonly Func<TaskModel> TaskModel = () => new TaskModel
    {
        Id = Guid.Empty,
        Title = "Correct Title"
    };

    public static readonly Func<TaskEntity> TaskEntity = () => new TaskEntity
    {
        Id = Guid.Empty,
        Title = "Correct Title"
    };
}