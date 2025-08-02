using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

internal sealed class InvitationLinkRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<InvitationLink>(context, context.InvitationLinks), IInvitationLinkRepository
{
    public override Task<InvitationLink?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.PrimaryLogo)
            .Include(x => x.SecondaryLogo)
            .Include(x => x.Entries)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }
}
