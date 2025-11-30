using Turnierplan.Core.Entity;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

public interface IRepositoryWithPublicId<TEntity, in TIdentifier> : IRepository<TEntity, TIdentifier>
    where TEntity : Entity<TIdentifier>, IEntityWithPublicId
{
    Task<TEntity?> GetByPublicIdAsync(PublicId id);
}
