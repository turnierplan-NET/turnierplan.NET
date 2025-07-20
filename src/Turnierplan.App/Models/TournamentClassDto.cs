namespace Turnierplan.App.Models;

public sealed record TournamentClassDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public required int? MaxTeamCount { get; init; }
}
