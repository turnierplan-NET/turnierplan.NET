using Turnierplan.Core.Exceptions;

namespace Turnierplan.Core.Tournament;

public sealed class Ranking
{
    private readonly List<RankingPosition> _positions = [];

    public bool IsEvaluated { get; private set; }

    public IReadOnlyList<RankingPosition> Positions
    {
        get
        {
            ThrowIfNotEvaluated();
            return _positions.AsReadOnly();
        }
    }

    public RankingPosition GetEntry(int position)
    {
        ThrowIfNotEvaluated();
        return _positions.Single(x => x.Position == position);
    }

    internal void Evaluate(IReadOnlyCollection<Team> teams, IReadOnlyCollection<Match> matches)
    {
        _positions.Clear();

        // TODO: Implement new algorithm and insert into the temporary list
        var positionsTemporary = new List<RankingPosition>();

        _positions.AddRange(positionsTemporary
            .OrderBy(x => x.Position)
            .ThenBy(x => x.Team?.Name));
        IsEvaluated = true;
    }

    private void ThrowIfNotEvaluated()
    {
        if (!IsEvaluated)
        {
            throw new TurnierplanException("The ranking cannot be accessed because it has not been evaluated yet.");
        }
    }
}
