using System.Collections.Immutable;

namespace Turnierplan.Core.Tournament.Definitions;

public sealed record FinalsMatchDefinition
{
    public FinalsMatchDefinition(ImmutableArray<MatchDefinition> matches)
    {
        Matches = matches;
        RequiredTeamsPerGroup = Matches.SelectMany(x => new[] { x.TeamA.PlacementRank, x.TeamB.PlacementRank }).Max();
    }

    public ImmutableArray<MatchDefinition> Matches { get; }

    public int RequiredTeamsPerGroup { get; }

    public sealed record MatchDefinition(AbstractTeamSelector TeamA, AbstractTeamSelector TeamB);
}
