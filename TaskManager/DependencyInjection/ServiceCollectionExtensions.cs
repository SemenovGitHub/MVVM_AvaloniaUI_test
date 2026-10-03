using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Models;
using TaskManager.Domain.Services;
using TaskManager.Infrastructure.Mapping;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Persistence.Repositories;
using TaskManager.Infrastructure.Validation;

namespace TaskManager.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskManager(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default должен быть задан.");

        services.AddDbContext<TaskDbContext>(
            options => options.UseNpgsql(connectionString),
            ServiceLifetime.Scoped);

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IValidator<TaskItem>, TaskItemValidator>();
        services.AddAutoMapper(config => config.AddProfile<TaskMappingProfile>());

        return services;
    }
}
