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
        DbSet = null!; // TODO: Find automatically
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
