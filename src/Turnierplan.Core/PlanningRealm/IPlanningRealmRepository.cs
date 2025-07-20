using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public interface IPlanningRealmRepository : IRepositoryWithPublicId<PlanningRealm, long>
{
    Task<PlanningRealm?> GetByPublicIdAsync(PublicId.PublicId id, Include include);

    [Flags]
    public enum Include
    {
        None = 0,
        TournamentClasses = 1,
        InvitationLinks = 2,
        ApplicationsWithTeams = 4,

        All = TournamentClasses | InvitationLinks | ApplicationsWithTeams
    }
}
