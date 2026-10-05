namespace TaskManager.Desktop.Middleware;

public interface IExceptionHandlingMiddleware
{
    /// <summary>Возвращает <c>null</c> при успехе или текст ошибки для интерфейса.</summary>
    Task<string?> InvokeAsync(Func<Task> operation);
}
