using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Dal.Repositories;

internal sealed class TournamentRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Tournament>(context, context.Tournaments), ITournamentRepository
{
    public override Task<Tournament?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id).Include(x => x.Organization).FirstOrDefaultAsync();
    }

    public async Task<Tournament?> GetByPublicIdAsync(PublicId id, ITournamentRepository.Include include)
    {
        var query = DbSet.Where(x => x.PublicId == id);

        if (include.HasFlag(ITournamentRepository.Include.Teams))
        {
            query = query.Include(x => x.Teams);
        }

        if (include.HasFlag(ITournamentRepository.Include.Groups))
        {
            query = query.Include(x => x.Groups).ThenInclude(x => x.Participants.OrderBy(p => p.Order).ThenBy(p => p.Team.Id));
        }

        if (include.HasFlag(ITournamentRepository.Include.Matches))
        {
            query = query.Include(x => x.Matches);
        }

        if (include.HasFlag(ITournamentRepository.Include.Documents))
        {
            query = query.Include(x => x.Documents);
        }

        if (include.HasFlag(ITournamentRepository.Include.Venue))
        {
            query = query.Include(x => x.Venue);
        }

        if (include.HasFlag(ITournamentRepository.Include.FolderWithTournaments))
        {
            query = query.Include(x => x.Folder).ThenInclude(x => x!.Tournaments);
        }
        else if (include.HasFlag(ITournamentRepository.Include.Folder))
        {
            query = query.Include(x => x.Folder);
        }

        if (include.HasFlag(ITournamentRepository.Include.Images))
        {
            query = query.Include(x => x.OrganizerLogo);
            query = query.Include(x => x.SponsorLogo);
            query = query.Include(x => x.SponsorBanner);
        }

        query = query.Include(x => x.Organization);

        query = query.AsSplitQuery();

        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }
}
