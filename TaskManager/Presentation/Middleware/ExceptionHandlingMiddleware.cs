using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskManager.Domain.Errors;
using TaskManager.Presentation.Contracts;

namespace TaskManager.Presentation.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            await WriteErrorAsync(context, exception);
        }
    }

    private async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = Map(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Ошибка при обработке запроса {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                "Запрос {Method} {Path} отклонён: {Error}",
                context.Request.Method,
                context.Request.Path,
                response.Error);
        }

        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private static (int StatusCode, ErrorResponse Response) Map(Exception exception)
    {
        return exception switch
        {
            ValidationException validation => (
                StatusCodes.Status400BadRequest,
                MapValidation(validation)),
            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                new ErrorResponse
                {
                    Error = notFound.Message,
                    Errors = [notFound.Message]
                }),
            _ when ContainsDatabaseException(exception) => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Error = "Не удалось выполнить операцию с базой данных.",
                    Errors = ["Не удалось выполнить операцию с базой данных."]
                }),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Error = "Внутренняя ошибка сервера.",
                    Errors = ["Внутренняя ошибка сервера."]
                })
        };
    }

    private static ErrorResponse MapValidation(ValidationException exception)
    {
        var errors = exception.Errors
            .Select(error => error.ErrorMessage)
            .ToArray();

        return new ErrorResponse
        {
            Error = errors.Length > 0 ? errors[0] : exception.Message,
            Errors = errors
        };
    }

    private static bool ContainsDatabaseException(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is DbUpdateException or NpgsqlException)
            {
                return true;
            }
        }

        return false;
    }
}
