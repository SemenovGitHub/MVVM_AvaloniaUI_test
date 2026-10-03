namespace TaskManager.Presentation.Contracts;

public sealed class ErrorResponse
{
    public string Error { get; init; } = string.Empty;

    public IReadOnlyList<string> Errors { get; init; } = [];
}
