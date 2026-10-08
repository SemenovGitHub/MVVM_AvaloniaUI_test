using Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.DI;

namespace TaskManager.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var services = AppServices.Build();

        ApplyMigrations(services);

        var app = AppBuilder.Configure(() => new App(services))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

        app.StartWithClassicDesktopLifetime(args);
    }

    private static void ApplyMigrations(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TaskDbContext>>();

        try
        {
            logger.LogInformation("Применение миграций базы данных");

            var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            context.Database.Migrate();
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "Не удалось применить миграции. Окно откроется, операции с данными вернут ошибку.");
        }
    }
}