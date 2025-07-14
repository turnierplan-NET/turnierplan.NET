using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

internal sealed class PlanningRealmRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<PlanningRealm>(context, context.PlanningRealms), IPlanningRealmRepository
{
    public override Task<PlanningRealm?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization).ThenInclude(x => x.RoleAssignments)
            .Include(x => x.RoleAssignments)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    public async Task<PlanningRealm?> GetByPublicIdAsync(PublicId id, IPlanningRealmRepository.Include include)
    {
        var query = DbSet.Where(x => x.PublicId == id);

        // TODO: Add includes

        query = query.Include(x => x.Organization).ThenInclude(x => x.RoleAssignments);
        query = query.Include(x => x.RoleAssignments);

        query = query.AsSplitQuery();

        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }
}
