using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class InvitationLinkEntry : Entity<long>
{
    internal InvitationLinkEntry(long id, int? maxTeamsPerRegistration, bool allowNewRegistrations)
    {
        Id = id;
        MaxTeamsPerRegistration = maxTeamsPerRegistration;
        AllowNewRegistrations = allowNewRegistrations;
    }

    internal InvitationLinkEntry(TournamentClass tournamentClass)
    {
        Id = 0;
        Class = tournamentClass;
        MaxTeamsPerRegistration = null;
        AllowNewRegistrations = true;
    }

    public override long Id { get; protected set; }

    public TournamentClass Class { get; internal set; } = null!;

    public int? MaxTeamsPerRegistration { get; set; }

    public bool AllowNewRegistrations { get; set; }
}
