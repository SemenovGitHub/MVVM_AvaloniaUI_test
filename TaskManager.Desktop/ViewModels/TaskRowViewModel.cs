using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.ViewModels;

public sealed partial class TaskRowViewModel : ObservableObject
{
    private readonly TaskItem _task;
    private readonly Func<TaskRowViewModel, Task> _completionChanged;
    private readonly Func<TaskRowViewModel, Task> _deleted;

    public TaskRowViewModel(
        TaskItem task,
        Func<TaskRowViewModel, Task> completionChanged,
        Func<TaskRowViewModel, Task> deleted)
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
    private Task ToggleCompletionAsync()
    {
        return _completionChanged(this);
    }

    [RelayCommand]
    private Task DeleteAsync()
    {
        return _deleted(this);
    }
}
