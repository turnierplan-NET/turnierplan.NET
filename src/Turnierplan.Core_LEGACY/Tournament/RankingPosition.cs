using System.Diagnostics.CodeAnalysis;

namespace Turnierplan.Core.Tournament;

public sealed record RankingPosition
{
    internal RankingPosition(int position, RankingReason reason, Team? team)
    {
        Position = position;
        Reason = reason;
        IsDefined = team is not null;
        Team = team;
    }

    public int Position { get; }

    public RankingReason Reason { get; }

    [MemberNotNullWhen(true, nameof(Team))]
    public bool IsDefined { get; }

    public Team? Team { get; }
}
