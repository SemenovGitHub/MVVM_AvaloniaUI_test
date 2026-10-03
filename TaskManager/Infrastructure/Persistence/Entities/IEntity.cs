namespace TaskManager.Infrastructure.Persistence.Entities;

public interface IEntity
{
    Guid Id { get; set; }

    DateTime CreatedAt { get; set; }

    bool IsDeleted { get; set; }

    DateTime? DeletedAt { get; set; }
}
