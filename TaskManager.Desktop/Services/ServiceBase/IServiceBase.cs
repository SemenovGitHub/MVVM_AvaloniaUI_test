using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Services;

public interface IServiceBase<TModel>
    where TModel : class, IBusinessModel
{
    Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken cancellationToken);

    Task<TModel> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TModel> CreateAsync(TModel model, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
