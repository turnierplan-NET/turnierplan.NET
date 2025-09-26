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

    public async Task<PlanningRealm?> GetByPublicIdAsync(PublicId id, IPlanningRealmRepository.Includes includes)
    {
        var query = DbSet.Where(x => x.PublicId == id);

        if (includes.HasFlag(IPlanningRealmRepository.Includes.TournamentClasses))
        {
            query = query.Include(x => x.TournamentClasses);
        }

        if (includes.HasFlag(IPlanningRealmRepository.Includes.InvitationLinks))
        {
            query = query.Include(x => x.InvitationLinks).ThenInclude(x => x.PrimaryLogo);
            query = query.Include(x => x.InvitationLinks).ThenInclude(x => x.SecondaryLogo);
            query = query.Include(x => x.InvitationLinks).ThenInclude(x => x.Entries);
        }

        if (includes.HasFlag(IPlanningRealmRepository.Includes.Applications))
        {
            query = query.Include(x => x.Applications).ThenInclude(x => x.SourceLink);
        }

        if (includes.HasFlag(IPlanningRealmRepository.Includes.Labels))
        {
            query = query.Include(x => x.Labels);
        }

        if (includes.HasFlag(IPlanningRealmRepository.Includes.ApplicationsWithTeams))
        {
            query = query.Include(x => x.Applications).ThenInclude(x => x.SourceLink);
            query = query.Include(x => x.Applications).ThenInclude(x => x.Teams).ThenInclude(x => x.Class);
            query = query.Include(x => x.Applications).ThenInclude(x => x.Teams).ThenInclude(x => x.Labels).ThenInclude(x => x.Name);
            query = query.Include(x => x.Applications).ThenInclude(x => x.Teams).ThenInclude(x => x.TeamLink);
        }

        if (includes.HasFlag(IPlanningRealmRepository.Includes.ApplicationsWithTeamsAndTournamentLinks))
        {
            query = query.Include(x => x.Applications).ThenInclude(x => x.SourceLink);
            query = query.Include(x => x.Applications).ThenInclude(x => x.Teams).ThenInclude(x => x.Class);
            query = query.Include(x => x.Applications).ThenInclude(x => x.Teams).ThenInclude(x => x.Labels).ThenInclude(x => x.Name);
            query = query.Include(x => x.Applications).ThenInclude(x => x.Teams).ThenInclude(x => x.TeamLink).ThenInclude(x => x!.Team).ThenInclude(x => x.Tournament).ThenInclude(x => x.Folder);
        }

        query = query.Include(x => x.Organization).ThenInclude(x => x.RoleAssignments);
        query = query.Include(x => x.RoleAssignments);

        query = query.AsSplitQuery();

        return await query.FirstOrDefaultAsync();
    }
}
