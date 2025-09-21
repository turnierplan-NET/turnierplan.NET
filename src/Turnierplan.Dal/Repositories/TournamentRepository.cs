using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Dal.Repositories;

internal sealed class TournamentRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Tournament>(context, context.Tournaments), ITournamentRepository
{
    public override Task<Tournament?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization).ThenInclude(x => x.RoleAssignments)
            .Include(x => x.Folder).ThenInclude(x => x!.RoleAssignments)
            .Include(x => x.RoleAssignments)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    public async Task<Tournament?> GetByPublicIdAsync(PublicId id, ITournamentRepository.Includes includes)
    {
        var query = DbSet.Where(x => x.PublicId == id);

        if (includes.HasFlag(ITournamentRepository.Includes.Teams))
        {
            query = query.Include(x => x.Teams).ThenInclude(x => x.TeamLink);
        }

        if (includes.HasFlag(ITournamentRepository.Includes.TeamsWithLinks))
        {
            query = query.Include(x => x.Teams).ThenInclude(x => x.TeamLink).ThenInclude(x => x!.ApplicationTeam).ThenInclude(x => x.Application).ThenInclude(x => x.PlanningRealm);
            query = query.Include(x => x.Teams).ThenInclude(x => x.TeamLink).ThenInclude(x => x!.ApplicationTeam).ThenInclude(x => x.Class);
        }

        if (includes.HasFlag(ITournamentRepository.Includes.Groups))
        {
            query = query.Include(x => x.Groups).ThenInclude(x => x.Participants.OrderBy(p => p.Order).ThenBy(p => p.Team.Id));
        }

        if (includes.HasFlag(ITournamentRepository.Includes.Matches))
        {
            query = query.Include(x => x.Matches);
        }

        if (includes.HasFlag(ITournamentRepository.Includes.Documents))
        {
            query = query.Include(x => x.Documents);
        }

        if (includes.HasFlag(ITournamentRepository.Includes.Venue))
        {
            query = query.Include(x => x.Venue);
        }

        if (includes.HasFlag(ITournamentRepository.Includes.FolderWithTournaments))
        {
            query = query.Include(x => x.Folder).ThenInclude(x => x!.Tournaments);
        }

        if (includes.HasFlag(ITournamentRepository.Includes.Images))
        {
            query = query.Include(x => x.PrimaryLogo);
            query = query.Include(x => x.SecondaryLogo);
            query = query.Include(x => x.BannerImage);
        }

        query = query.Include(x => x.Organization).ThenInclude(x => x.RoleAssignments);
        query = query.Include(x => x.Folder).ThenInclude(x => x!.RoleAssignments);
        query = query.Include(x => x.RoleAssignments);

        query = query.AsSplitQuery();

        return await query.FirstOrDefaultAsync();
    }
}
