using FluentValidation;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Validation;

public sealed class TaskValidator : AbstractValidator<TaskModel>
{
    private const int ConstMaxTitleLength = 100;
    
    public TaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название задачи обязательно.")
            .MaximumLength(ConstMaxTitleLength)
            .WithMessage("Название задачи не может быть длиннее 100 символов.");
    }
}
