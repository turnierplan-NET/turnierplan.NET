namespace Turnierplan.Core.SeedWork;

public interface IRepositoryWithPublicId<TEntity, in TIdentifier> : IRepository<TEntity, TIdentifier>
    where TEntity : Entity<TIdentifier>, IEntityWithPublicId
{
    Task<TEntity?> GetByPublicIdAsync(PublicId.PublicId id);
}
