using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Errors;
using TaskManager.Infrastructure.Persistence.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class, IEntity
{
    public RepositoryBase(TaskDbContext context)
    {
        Context = context;
    }

    protected TaskDbContext Context { get; }

    protected DbSet<TEntity> Entities => Context.Set<TEntity>();

    protected virtual string NotFoundMessage => "Запись не найдена.";

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await Entities
            .AsNoTracking()
            .OrderBy(entity => entity.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Entities.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await Entities.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(
            async () =>
            {
                var entity = await GetByIdAsync(id, cancellationToken);
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                return entity;
            },
            cancellationToken);
    }

    protected async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

        var result = await operation();
        await Context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return result;
    }
}
