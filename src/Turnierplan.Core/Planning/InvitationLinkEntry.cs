using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Planning;

public sealed class InvitationLinkEntry : Entity<int>
{
    internal InvitationLinkEntry(int id, int? maxTeamsPerRegistration, bool allowNewRegistrations)
    {
        Id = id;
        MaxTeamsPerRegistration = maxTeamsPerRegistration;
        AllowNewRegistrations = allowNewRegistrations;
    }

    public override int Id { get; protected set; }

    public TournamentClass Class { get; internal set; } = null!;

    public int? MaxTeamsPerRegistration { get; set; }

    public bool AllowNewRegistrations { get; set; }
}
