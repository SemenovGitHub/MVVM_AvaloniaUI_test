using FluentValidation;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Validation;

public sealed class TaskModelValidator : AbstractValidator<TaskModel>
{
    private const int ConstMaxTitleLength = 100;

    public TaskModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название задачи обязательно.")
            .MaximumLength(ConstMaxTitleLength)
            .WithMessage($"Название задачи не может быть длиннее {ConstMaxTitleLength} символов.");
    }
}