using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Planning;

public interface IPlanningRealmRepository : IRepositoryWithPublicId<PlanningRealm, long>
{
    Task<PlanningRealm?> GetByPublicIdAsync(PublicId.PublicId id, Include include);

    [Flags]
    public enum Include
    {
        None = 0
    }
}
