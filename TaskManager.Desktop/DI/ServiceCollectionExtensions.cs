using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.Data.Repository;
using TaskManager.Desktop.Mapping;
using TaskManager.Desktop.Middleware;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Services;
using TaskManager.Desktop.Validation;
using TaskManager.Desktop.ViewModels;

namespace TaskManager.Desktop.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskManager(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default должен быть задан.");

        services.AddDbContext<TaskDbContext>(
            options => options.UseNpgsql(connectionString));

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IValidator<TaskModel>, TaskModelValidator>();
        services.AddAutoMapper(config => config.AddProfile<TaskMappingProfile>());

        services.AddSingleton<IExceptionHandlingMiddleware, ExceptionHandlingMiddleware>();
        services.AddSingleton<MainViewModel>();

        return services;
    }
}
