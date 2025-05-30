using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Dal.Repositories;

public abstract class RepositoryBaseWithPublicId<TEntity> : RepositoryBase<TEntity, long>, IRepositoryWithPublicId<TEntity, long>
    where TEntity : Entity<long>, IEntityWithPublicId
{
    protected RepositoryBaseWithPublicId(IUnitOfWork unitOfWork, DbSet<TEntity> dbSet)
        : base(unitOfWork, dbSet)
    {
    }

    public virtual Task<TEntity?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id).FirstOrDefaultAsync();
    }
}
