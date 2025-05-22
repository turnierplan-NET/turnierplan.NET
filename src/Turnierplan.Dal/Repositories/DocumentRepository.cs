using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

internal sealed class DocumentRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Document>(context, context.Documents), IDocumentRepository
{
    public async Task<Document?> GetByPublicIdAsync(PublicId id, bool includeTournamentDetails = false)
    {
        var query = DbSet.Where(x => x.PublicId == id);

        query = query.Include(x => x.Tournament).ThenInclude(x => x.Organization);

        if (includeTournamentDetails)
        {
            query = query.Include(x => x.Tournament.Teams);
            query = query.Include(x => x.Tournament.Groups).ThenInclude(x => x.Participants.OrderBy(p => p.Order).ThenBy(p => p.Team.Id));
            query = query.Include(x => x.Tournament.Matches);
            query = query.Include(x => x.Tournament.Venue);

            query = query.Include(x => x.Tournament.OrganizerLogo);
            query = query.Include(x => x.Tournament.SponsorLogo);
            query = query.Include(x => x.Tournament.SponsorBanner);

            query = query.AsSplitQuery();
        }

        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }
}
