namespace TaskManager.Presentation.Contracts;

public sealed class TaskResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }

    public DateTime CreatedAt { get; init; }
}
