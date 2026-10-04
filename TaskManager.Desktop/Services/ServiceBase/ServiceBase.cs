using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Desktop.Data.Entities;
using TaskManager.Desktop.Data.Repository.RepositoryBase;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Services;

public abstract class ServiceBase<TModel, TEntity, TRepository> : IServiceBase<TModel>
    where TModel : class, IModel
    where TEntity : class, IEntity
    where TRepository : IRepositoryBase<TEntity>
{
    protected ServiceBase(
        TRepository repository,
        IValidator<TModel> validator,
        IMapper mapper,
        ILogger logger)
    {
        Repository = repository;
        Validator = validator;
        Mapper = mapper;
        Logger = logger;
    }

    protected TRepository Repository { get; }

    protected IValidator<TModel> Validator { get; }

    protected IMapper Mapper { get; }

    protected ILogger Logger { get; }

    public async Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await Repository.GetAllAsync(cancellationToken);
        return Mapper.Map<IReadOnlyList<TModel>>(entities);
    }

    public async Task<TModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        return Mapper.Map<TModel>(entity);
    }

    public virtual async Task<TModel> CreateAsync(TModel model, CancellationToken cancellationToken)
    {
        model.Id = Guid.NewGuid();
        model.CreatedAt = DateTime.UtcNow;

        await Validator.ValidateAndThrowAsync(model, cancellationToken);

        var entity = Mapper.Map<TEntity>(model);
        await Repository.AddAsync(entity, cancellationToken);

        Logger.LogInformation("Создана запись {EntityType} {EntityId}", typeof(TEntity).Name, entity.Id);
        return Mapper.Map<TModel>(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await Repository.DeleteAsync(id, cancellationToken);

        Logger.LogInformation(
            "Запись {EntityType} {EntityId} скрыта мягким удалением",
            typeof(TEntity).Name,
            id);
    }
}
