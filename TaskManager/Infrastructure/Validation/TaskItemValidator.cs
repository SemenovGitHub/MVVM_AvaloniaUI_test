using FluentValidation;
using TaskManager.Domain.Models;

namespace TaskManager.Infrastructure.Validation;

public sealed class TaskItemValidator : AbstractValidator<TaskItem>
{
    public TaskItemValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty()
            .WithMessage("Название задачи обязательно.")
            .MaximumLength(100)
            .WithMessage("Название задачи не может быть длиннее 100 символов.");
    }
}
