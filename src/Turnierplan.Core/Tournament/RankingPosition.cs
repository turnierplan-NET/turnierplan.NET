using System.Diagnostics.CodeAnalysis;

namespace Turnierplan.Core.Tournament;

public sealed record RankingPosition
{
    internal RankingPosition(int position, Team? team)
    {
        Position = position;
        IsDefined = team is not null;
        Team = team;
    }

    public int Position { get; }

    [MemberNotNullWhen(true, nameof(Team))]
    public bool IsDefined { get; }

    public Team? Team { get; }
}
