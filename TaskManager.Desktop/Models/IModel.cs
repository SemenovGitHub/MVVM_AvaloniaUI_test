namespace TaskManager.Desktop.Models;

public interface IModel
{
    Guid Id { get; set; }

    DateTime CreatedAt { get; set; }
}
