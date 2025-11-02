using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Turnierplan.Core.Tournament;

public sealed class Ranking : IEnumerable<RankingPosition>
{
    private readonly List<RankingPosition> _positions = [];

    public IReadOnlyList<RankingPosition> Positions => _positions.AsReadOnly();

    public RankingPosition GetEntry(int position)
    {
        return _positions.Single(x => x.Position == position);
    }

    public IEnumerator<RankingPosition> GetEnumerator()
    {
        return _positions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // More complex logic will be introduced with issues #2 and #247

    internal void Reset()
    {
        _positions.Clear();
    }

    internal void AddRanking(int position, Team team)
    {
        _positions.Add(new RankingPosition(position, team));
    }

    internal void FinalizeRanking()
    {
        // TODO: Improve how this works when implementing #2 / #247

        var maxPosition = _positions.Max(x => x.Position);

        for (var i = 1; i <= maxPosition; i++)
        {
            if (!_positions.Any(x => x.Position == i))
            {
                _positions.Add(new RankingPosition(i, null));
            }
        }

        _positions.Sort((a, b) => a.Position - b.Position);
    }
}

public sealed record RankingPosition
{
    public RankingPosition(int position, Team? team)
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
