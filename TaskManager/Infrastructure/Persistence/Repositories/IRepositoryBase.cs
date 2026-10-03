using TaskManager.Infrastructure.Persistence.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public interface IRepositoryBase<TEntity>
    where TEntity : class, IEntity
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);

    Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
