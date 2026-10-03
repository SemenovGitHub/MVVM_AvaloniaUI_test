namespace TaskManager.Domain.Models;

public sealed class TaskItem : IBusinessModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; }
}
