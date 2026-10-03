namespace TaskManager.Domain.Models;

public interface IBusinessModel
{
    Guid Id { get; set; }

    DateTime CreatedAt { get; set; }
}
