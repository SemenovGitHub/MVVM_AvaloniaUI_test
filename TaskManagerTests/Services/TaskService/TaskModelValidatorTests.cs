using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.TestHelper;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Validation;

namespace TaskManagerTests.Services.TaskService;

public sealed class TaskModelValidatorTests
{
    private static TaskModelValidator CreateTaskModelValidator()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<TaskModelValidator>();

        builder.Populate([]);

        var container = builder.Build();

        return container.Resolve<TaskModelValidator>();
    }

    [Fact]
    public void TaskModelValidator_Positive_Validate_Title()
    {
        var validator = CreateTaskModelValidator();

        var result = validator.TestValidate(CreateTaskModel());

        result.ShouldNotHaveAnyValidationErrors();

        return;

        TaskModel CreateTaskModel()
        {
            var data = TaskData.TaskModel();

            return data;
        }
    }

    [Fact]
    public void TaskModelValidator_Negative_Validate_Long_Title()
    {
        var validator = CreateTaskModelValidator();

        var result = validator.TestValidate(CreateTaskModel());

        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Название задачи не может быть длиннее 100 символов.");

        return;

        TaskModel CreateTaskModel()
        {
            var data = TaskData.TaskModel();

            data.Title =
                "01000000000100000000000000000000lllllllooooooooooooooooonnnnnnggggggggggggggggg00000000011111111000011111111111111111";

            return data;
        }
    }

    [Fact]
    public void TaskModelValidator_Negative_Validate_Empty_Title()
    {
        var validator = CreateTaskModelValidator();

        var result = validator.TestValidate(CreateTaskModel());

        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Название задачи обязательно.");

        return;

        TaskModel CreateTaskModel()
        {
            var data = TaskData.TaskModel();

            data.Title = " ";
            return data;
        }
    }
}