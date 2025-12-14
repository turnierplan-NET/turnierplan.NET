using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Entity;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

internal abstract class RepositoryBaseWithPublicId<TEntity> : RepositoryBase<TEntity, long>, IRepositoryWithPublicId<TEntity, long>
    where TEntity : Entity<long>, IEntityWithPublicId
{
    protected RepositoryBaseWithPublicId(TurnierplanContext context)
        : base(context)
    {
    }

    public virtual Task<TEntity?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id).FirstOrDefaultAsync();
    }
}
