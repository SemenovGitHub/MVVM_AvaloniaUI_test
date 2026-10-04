namespace TaskManager.Desktop.Models;

public sealed class TaskModel : IModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; }
}
