using FluentAssertions;

namespace TaskManagerTests.Services.TaskRepository;

public sealed class TaskRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _postgres;

    public TaskRepositoryTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    [Fact]
    public async Task CreatesTask_Positive()
    {
        await using var context = _postgres.CreateContext();

        var repository = new TaskManager.Desktop.Data.Repository.TaskRepository(context);

        var taskEntity = TaskData.TaskEntity();

        taskEntity.Id = Guid.NewGuid();

        await repository.AddAsync(taskEntity, CancellationToken.None);

        var result = await repository.GetByIdAsync(taskEntity.Id, CancellationToken.None);

        result.Title.Should().Be(taskEntity.Title);
        result.IsDeleted.Should().Be(false);
        result.Id.Should().Be(taskEntity.Id);
    }
}