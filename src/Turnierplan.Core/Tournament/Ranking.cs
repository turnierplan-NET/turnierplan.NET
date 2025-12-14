using System.Collections;

namespace Turnierplan.Core.Tournament;

public sealed class Ranking : IEnumerable<RankingPosition>
{
    private readonly List<RankingPosition> _positions = [];

    #region Access properties/methods and IEnumerable implementation

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

    #endregion

    #region Ranking evaluation logic

    internal void Evaluate(IReadOnlyCollection<Team> teams, IReadOnlyCollection<Match> matches)
    {
        _positions.Clear();

        // TODO: Implement

        _positions.Sort((a, b) => a.Position - b.Position);
    }

    #endregion
}
