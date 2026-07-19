using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.Dal.Repositories;

public interface IResourcePlannerRepository : IRepositoryWithPublicId<ResourcePlanner, long>
{
    Task<ResourcePlanner?> GetByPublicIdAsync(PublicId id, Includes includes);

    [Flags]
    public enum Includes
    {
        None = 0,
        ResourceGroupsWithResources = 1,
        ResourcePlannerViews = 2,

        All = ResourceGroupsWithResources | ResourcePlannerViews
    }
}

internal sealed class ResourcePlannerRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<ResourcePlanner>(context), IResourcePlannerRepository
{
    public async Task<ResourcePlanner?> GetByPublicIdAsync(PublicId id, IResourcePlannerRepository.Includes includes)
    {
        var query = DbSet.Where(x => x.PublicId == id);

        if (includes.HasFlag(IResourcePlannerRepository.Includes.ResourceGroupsWithResources))
        {
            query = query.Include(x => x.ResourceGroups).ThenInclude(x => x.ResourceAssignments).ThenInclude(x => x.Resource);
        }

        if (includes.HasFlag(IResourcePlannerRepository.Includes.ResourcePlannerViews))
        {
            query = query.Include(x => x.ResourcePlannerViews).ThenInclude(x => x.ResourceGroups);
        }

        query = query.Include(x => x.Organization).ThenInclude(x => x.RoleAssignments);
        query = query.Include(x => x.RoleAssignments);

        query = query.AsSplitQuery();

        return await query.FirstOrDefaultAsync();
    }
}
