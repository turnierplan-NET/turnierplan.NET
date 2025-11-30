using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Entity;
using Turnierplan.Dal.UnitOfWork;

namespace Turnierplan.Dal.Repositories;

internal abstract class RepositoryBase<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier>
    where TEntity : Entity<TIdentifier>
{
    protected RepositoryBase(TurnierplanContext context)
    {
        UnitOfWork = context;

        var properties = typeof(TurnierplanContext)
            .GetProperties()
            .Where(x => x.PropertyType == typeof(DbSet<TEntity>))
            .ToList();

        if (properties.Count != 1)
        {
            throw new InvalidOperationException($"The '{nameof(TurnierplanContext)}' must have exactly one property of type '{nameof(DbSet<>)}<{typeof(TEntity).Name}>', but {properties.Count} where found.'");
        }

        DbSet = properties.Single().GetValue(context) as DbSet<TEntity> ?? throw new InvalidOperationException($"Failed to get '{nameof(DbSet<>)}<{typeof(TEntity).Name}>' from '{nameof(TurnierplanContext)}' via reflection.");
    }

    public IUnitOfWork UnitOfWork { get; }

    protected DbSet<TEntity> DbSet { get; }

    public virtual async Task CreateAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}
