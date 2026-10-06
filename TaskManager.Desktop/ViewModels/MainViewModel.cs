using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Desktop.Middleware;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Services;

namespace TaskManager.Desktop.ViewModels;

public sealed partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IExceptionHandlingMiddleware _middleware;

    private readonly CancellationTokenSource _lifeTimeCancellationTokenSource = new();

    private bool _disposed;

    [ObservableProperty] private string _newTaskTitle = string.Empty;

    [ObservableProperty] private string? _errorMessage;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private ObservableCollection<TaskRowViewModel> _tasks = [];

    public MainViewModel(IServiceScopeFactory scopeFactory, IExceptionHandlingMiddleware middleware)
    {
        _scopeFactory = scopeFactory;
        _middleware = middleware;
    }

    [RelayCommand]
    private Task ReloadAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async (service, token) =>
        {
            var tasks = await service.GetAllAsync(token);

            var rows = new ObservableCollection<TaskRowViewModel>();

            foreach (var task in tasks)
            {
                rows.Add(CreateRow(task));
            }

            Tasks = rows;
        }, cancellationToken);
    }

    [RelayCommand]
    private Task AddAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async (service, token) =>
        {
            var task = new TaskModel { Title = NewTaskTitle };
            var created = await service.CreateAsync(task, token);

            Tasks.Add(CreateRow(created));
            NewTaskTitle = string.Empty;
        }, cancellationToken);
    }

    private TaskRowViewModel CreateRow(TaskModel task)
    {
        return new TaskRowViewModel(task, ToggleCompletionAsync, DeleteRowAsync);
    }

    private Task ToggleCompletionAsync(TaskRowViewModel row, CancellationToken cancellationToken)
    {
        var newValue = row.IsCompleted;

        return ExecuteAsync(Save, cancellationToken, Restore);

        Task Save(ITaskService service, CancellationToken token)
        {
            return service.SetCompletionAsync(row.Id, newValue, token);
        }

        void Restore()
        {
            row.IsCompleted = !newValue;
        }
    }

    private Task DeleteRowAsync(TaskRowViewModel row, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async (service, token) =>
        {
            await service.DeleteAsync(row.Id, token);

            Tasks.Remove(row);
        }, cancellationToken);
    }

    private async Task ExecuteAsync(Func<ITaskService, CancellationToken, Task> operation,
        CancellationToken cancellationToken, Action? onFailure = null)
    {
        if (_disposed) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _lifeTimeCancellationTokenSource.Token);

            ErrorMessage = await _middleware.InvokeAsync(async () =>
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

                await operation(service, linkedToken.Token);
            });

            if (ErrorMessage is not null)
            {
                onFailure?.Invoke();
                await HideError();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _lifeTimeCancellationTokenSource.Cancel();
        _lifeTimeCancellationTokenSource.Dispose();
    }

    private async Task HideError()
    {
        var currentError = ErrorMessage;

        await Task.Delay(TimeSpan.FromSeconds(2));

        if (ErrorMessage == currentError)
        {
            ErrorMessage = null;
        }
    }
}