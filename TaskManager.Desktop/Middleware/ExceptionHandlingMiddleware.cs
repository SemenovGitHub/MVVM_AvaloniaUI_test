using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Desktop.Errors;

namespace TaskManager.Desktop.Middleware;

public sealed class ExceptionHandlingMiddleware : IExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task<string?> InvokeAsync(Func<Task> operation)
    {
        try
        {
            await operation();
            return null;
        }
        catch (Exception exception)
        {
            var message = ErrorText.Resolve(exception);

            if (exception is ValidationException or NotFoundException)
            {
                _logger.LogWarning("Операция отклонена: {Error}", message);
            }
            else
            {
                _logger.LogError(exception, "Операция не выполнена");
            }

            return message;
        }
    }
}
