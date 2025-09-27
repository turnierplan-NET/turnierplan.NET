using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public interface IPlanningRealmRepository : IRepositoryWithPublicId<PlanningRealm, long>
{
    Task<PlanningRealm?> GetByPublicIdAsync(PublicId.PublicId id, Includes includes);

    [Flags]
    public enum Includes
    {
        None = 0,
        TournamentClasses = 1,
        InvitationLinks = 2,
        Applications = 4,
        Labels = 8,

        All = TournamentClasses | InvitationLinks | Applications | Labels,

        ApplicationsWithTeams = 16,
        ApplicationsWithTeamsAndTournamentLinks = 32
    }
}
