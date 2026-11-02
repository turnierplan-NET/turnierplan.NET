using System.Collections.Immutable;

namespace Turnierplan.Core.Tournament.Definitions;

public sealed record GroupMatchDefinition
{
    public GroupMatchDefinition(ImmutableArray<MatchBlock> matchBlocks)
    {
        MatchBlocks = matchBlocks;
    }

    public ImmutableArray<MatchBlock> MatchBlocks { get; }

    public int BlockCount => MatchBlocks.Length;

    public sealed record MatchBlock(ImmutableArray<MatchDefinition> Matches);

    public sealed record MatchDefinition(int TeamIndexA, int TeamIndexB);
}
