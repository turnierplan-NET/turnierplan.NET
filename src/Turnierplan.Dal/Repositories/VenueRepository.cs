using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Venue;

namespace Turnierplan.Dal.Repositories;

internal sealed class VenueRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Venue>(context, context.Venues), IVenueRepository
{
    public override Task<Venue?> GetByPublicIdAsync(PublicId id)
    {
        // Always include organization to allow for checking authorization via its owner
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization)
            .FirstOrDefaultAsync();
    }
}
