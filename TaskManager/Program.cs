using Microsoft.EntityFrameworkCore;
using TaskManager.DependencyInjection;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTaskManager(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await ApplyMigrationsAsync(app);

app.Run();

static async Task ApplyMigrationsAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Migrations");

    try
    {
        logger.LogInformation("Применение миграций базы данных");
        await context.Database.MigrateAsync();
    }
    catch (Exception exception)
    {
        logger.LogError(exception, "Не удалось применить миграции. Запросы к базе вернут ошибку, процесс не завершается.");
    }
}
