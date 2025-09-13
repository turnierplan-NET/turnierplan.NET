using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Dal.Repositories;

public abstract class RepositoryBase<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier>
    where TEntity : Entity<TIdentifier>
{
    protected RepositoryBase(IUnitOfWork unitOfWork, DbSet<TEntity> dbSet)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(dbSet);

        UnitOfWork = unitOfWork;
        DbSet = dbSet;
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
