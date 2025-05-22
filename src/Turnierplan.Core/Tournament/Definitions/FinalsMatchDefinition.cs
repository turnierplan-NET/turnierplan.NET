namespace Turnierplan.Core.Tournament.Definitions;

public sealed record FinalsMatchDefinition
{
    public FinalsMatchDefinition(IEnumerable<MatchDefinition> matches)
    {
        Matches = matches.ToList();
        RequiredTeamsPerGroup = Matches.SelectMany(x => new[] { x.TeamA.PlacementRank, x.TeamB.PlacementRank }).Max();
    }

    public IReadOnlyList<MatchDefinition> Matches { get; }

    public int RequiredTeamsPerGroup { get; }

    public sealed record MatchDefinition(AbstractTeamSelector TeamA, AbstractTeamSelector TeamB);
}
