using Microsoft.EntityFrameworkCore;
using TaskManager.Desktop.Data;
using Testcontainers.PostgreSql;

namespace TaskManagerTests.Services.TaskRepository;

public class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("tasks")
        .WithUsername("taskmanager")
        .WithPassword("taskmanager")
        .Build();

    public Task InitializeAsync()
    {
        return Container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Container.DisposeAsync().AsTask();
    }

    public TaskDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseNpgsql(Container.GetConnectionString())
            .Options;
        
        var context = new TaskDbContext(options);
        
        context.Database.Migrate();
        
        return context;
    }
}