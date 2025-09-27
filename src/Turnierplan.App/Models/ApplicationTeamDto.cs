namespace Turnierplan.App.Models;

public sealed record ApplicationTeamDto
{
    public required long Id { get; init; }

    public required long TournamentClassId { get; init; }

    public required string Name { get; init; }

    public required long[] LabelIds { get; init; }

    public required ApplicationTeamLinkedTournamentDto? LinkedTournament { get; init; }
}
