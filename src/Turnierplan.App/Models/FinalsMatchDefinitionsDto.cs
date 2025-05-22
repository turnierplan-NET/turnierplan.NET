namespace Turnierplan.App.Models;

public sealed record FinalsMatchDefinitionDto
{
    public required int GroupCount { get; init; }

    public required int MatchCount { get; init; }

    public required int RequiredTeamsPerGroup { get; init; }
}
