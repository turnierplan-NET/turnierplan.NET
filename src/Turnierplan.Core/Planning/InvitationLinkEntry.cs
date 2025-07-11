using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Planning;

public sealed class InvitationLinkEntry : Entity<long>
{
    internal InvitationLinkEntry(long id, int? maxTeamsPerRegistration, bool allowNewRegistrations)
    {
        Id = id;
        MaxTeamsPerRegistration = maxTeamsPerRegistration;
        AllowNewRegistrations = allowNewRegistrations;
    }

    public override long Id { get; protected set; }

    public TournamentClass Class { get; internal set; } = null!;

    public int? MaxTeamsPerRegistration { get; set; }

    public bool AllowNewRegistrations { get; set; }
}
