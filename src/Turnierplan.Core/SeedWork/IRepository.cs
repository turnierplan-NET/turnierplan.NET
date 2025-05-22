namespace Turnierplan.Core.SeedWork;

public interface IRepository<TEntity, in TIdentifier>
    where TEntity : Entity<TIdentifier>
{
    IUnitOfWork UnitOfWork { get; }

    Task CreateAsync(TEntity entity);

    void Remove(TEntity entity);
}
