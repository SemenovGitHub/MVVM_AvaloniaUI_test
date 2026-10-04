using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Desktop.Middleware;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Services;

namespace TaskManager.Desktop.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    private readonly IExceptionHandlingMiddleware _middleware;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    [ObservableProperty]
    private string _newTaskTitle = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private TaskRowViewModel? _selectedTask;

    public MainViewModel(IServiceScopeFactory scopeFactory, IExceptionHandlingMiddleware middleware)
    {
        _scopeFactory = scopeFactory;
        _middleware = middleware;
    }

    public ObservableCollection<TaskRowViewModel> Tasks { get; } = new();

    public bool HasError => ErrorMessage is not null;

    [RelayCommand]
    private Task ReloadAsync()
    {
        return ExecuteAsync(async service =>
        {
            var tasks = await service.GetAllAsync(_cancellationTokenSource.Token);

            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(CreateRow(task));
            }
        });
    }

    [RelayCommand]
    private Task AddAsync()
    {
        return ExecuteAsync(async service =>
        {
            var task = new TaskModel { Title = NewTaskTitle };
            var created = await service.CreateAsync(task, _cancellationTokenSource.Token);

            Tasks.Add(CreateRow(created));
            NewTaskTitle = string.Empty;
        });
    }

    [RelayCommand]
    private Task DeleteSelectedAsync()
    {
        if (SelectedTask is null)
        {
            return Task.CompletedTask;
        }

        return DeleteRowAsync(SelectedTask, _cancellationTokenSource.Token);
    }

    private TaskRowViewModel CreateRow(TaskModel task)
    {
        return new TaskRowViewModel(task, ToggleCompletionAsync, DeleteRowAsync);
    }

    private Task ToggleCompletionAsync(TaskRowViewModel row)
    {
        var requested = row.IsCompleted;

        return ExecuteAsync(
            service => service.SetCompletionAsync(row.Id, requested, CancellationToken.None),
            () => row.IsCompleted = !requested);
    }

    private Task DeleteRowAsync(TaskRowViewModel row, CancellationToken token)
    {
        return ExecuteAsync(async service =>
        {
            await service.DeleteAsync(row.Id, token);

            Tasks.Remove(row);
        });
    }

    private async Task ExecuteAsync(Func<ITaskService, Task> operation, Action? onFailure = null)
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            ErrorMessage = await _middleware.InvokeAsync(async () =>
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

                await operation(service);
            });

            if (HasError)
            {
                onFailure?.Invoke();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnErrorMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    public void CancelPending()
    {
        _cancellationTokenSource.Cancel();
    }
}
