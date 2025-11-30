using Turnierplan.Core.Entity;
using Turnierplan.Dal.UnitOfWork;

namespace Turnierplan.Dal.Repositories;

public interface IRepository<in TEntity, in TIdentifier>
    where TEntity : Entity<TIdentifier>
{
    IUnitOfWork UnitOfWork { get; }

    Task CreateAsync(TEntity entity);

    void Remove(TEntity entity);
}
