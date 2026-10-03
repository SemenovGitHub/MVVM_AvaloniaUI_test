namespace TaskManager.Presentation.Contracts;

public sealed class CreateTaskRequest
{
    public required string Title { get; init; }
}
