using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Venue;

namespace Turnierplan.Dal.Repositories;

internal sealed class VenueRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Venue, long>(context, context.Venues), IVenueRepository
{
    public override Task<Venue?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization).ThenInclude(x => x.RoleAssignments)
            .Include(x => x.RoleAssignments)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }
}
