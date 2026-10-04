using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.ViewModels;

public sealed partial class TaskRowViewModel : ObservableObject
{
    private readonly TaskModel _task;
    private readonly Func<TaskRowViewModel, CancellationToken, Task> _completionChanged;
    private readonly Func<TaskRowViewModel, CancellationToken, Task> _deleted;

    public TaskRowViewModel(
        TaskModel task,
        Func<TaskRowViewModel, CancellationToken, Task> completionChanged,
        Func<TaskRowViewModel, CancellationToken, Task> deleted)
    {
        _task = task;
        _completionChanged = completionChanged;
        _deleted = deleted;
    }

    public Guid Id => _task.Id;

    public string Title => _task.Title;

    public DateTime CreatedAt => _task.CreatedAt.ToLocalTime();

    public bool IsCompleted
    {
        get => _task.IsCompleted;
        set => SetProperty(_task.IsCompleted, value, _task, static (task, completed) => task.IsCompleted = completed);
    }

    [RelayCommand]
    private Task ToggleCompletionAsync(CancellationToken cancellationToken)
    {
        return _completionChanged(this, cancellationToken);
    }

    [RelayCommand]
    private Task DeleteAsync(CancellationToken cancellationToken)
    {
        return _deleted(this, cancellationToken);
    }
}
