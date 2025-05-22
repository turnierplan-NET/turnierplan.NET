using System.Collections.Immutable;

namespace Turnierplan.Core.Tournament.Definitions;

public sealed record GroupMatchDefinition
{
    public GroupMatchDefinition(IEnumerable<MatchBlock> matchBlocks)
    {
        MatchBlocks = matchBlocks.ToList();
    }

    public IReadOnlyList<MatchBlock> MatchBlocks { get; }

    public int BlockCount => MatchBlocks.Count;

    public sealed record MatchBlock(ImmutableArray<MatchDefinition> Matches);

    public sealed record MatchDefinition(int TeamIndexA, int TeamIndexB);
}
