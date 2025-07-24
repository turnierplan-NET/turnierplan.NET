namespace Turnierplan.App.Models;

public sealed record InvitationLinkEntryDto
{
    public required long TournamentClassId { get; init; }

    public required bool AllowNewRegistrations { get; init; }

    public required int? MaxTeamsPerRegistration { get; init; }

    public required int NumberOfTeams { get; init; }
}
