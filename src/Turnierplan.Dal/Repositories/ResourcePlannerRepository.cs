using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.Dal.Repositories;

public interface IResourcePlannerRepository : IRepositoryWithPublicId<ResourcePlanner, long>
{
}

internal sealed class ResourcePlannerRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<ResourcePlanner>(context), IResourcePlannerRepository
{
}
