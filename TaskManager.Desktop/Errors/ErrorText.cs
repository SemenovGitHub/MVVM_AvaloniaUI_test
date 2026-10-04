using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TaskManager.Desktop.Errors;

public static class ErrorText
{
    public const string Database = "Не удалось выполнить операцию с базой данных.";

    public const string Unexpected = "Непредвиденная ошибка.";

    public static string Resolve(Exception exception)
    {
        return exception switch
        {
            ValidationException validation => Describe(validation)[0],
            NotFoundException notFound => notFound.Message,
            _ when IsDatabaseFailure(exception) => Database,
            _ => Unexpected
        };
    }

    public static IReadOnlyList<string> Describe(ValidationException exception)
    {
        var errors = exception.Errors
            .Select(error => error.ErrorMessage)
            .ToArray();

        return errors.Length > 0 ? errors : new[] { exception.Message };
    }

    public static bool IsDatabaseFailure(Exception exception)
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
