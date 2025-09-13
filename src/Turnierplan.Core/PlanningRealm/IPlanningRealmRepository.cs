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
        ApplicationsWithTeams = 4,

        All = TournamentClasses | InvitationLinks | ApplicationsWithTeams
    }
}
