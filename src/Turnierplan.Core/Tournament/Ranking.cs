using Turnierplan.Core.Exceptions;

namespace Turnierplan.Core.Tournament;

public sealed class Ranking
{
    private readonly Tournament _tournament;
    private readonly List<RankingPosition> _positions = [];

    internal Ranking(Tournament tournament)
    {
        _tournament = tournament;
    }

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

    internal void Evaluate()
    {
        var positionsTemporary = new List<RankingPosition>();

        // Matches with a defined playoff position (final, 3rd playoff, etc.) are always evaluated immediately
        foreach (var match in _tournament._matches)
        {
            if (!match.PlayoffPosition.HasValue)
            {
                continue;
            }

            positionsTemporary.Add(new RankingPosition(match.PlayoffPosition.Value, match.GetWinningTeam()));
            positionsTemporary.Add(new RankingPosition(match.PlayoffPosition.Value + 1, match.GetLosingTeam()));
        }

        if (_tournament._matches.Any(x => x is { IsGroupMatch: true, IsFinished: false }))
        {
            // If any group match is non-finished, fill the remaining rankings with 'blank spaces'
            var takenPositions = positionsTemporary.Select(x => x.Position);
            var missingPositions = Enumerable.Range(0, _tournament._teams.Count)
                .Except(takenPositions)
                .Select(positions => new RankingPosition(positions, null));

            positionsTemporary.AddRange(missingPositions);
        }
        else
        {
            // All other rankings can only be evaluated if all group matches are finished
            var sections = new List<RankingSection>();

            // Add a ranking section for each finals round in the tournament
            sections.AddRange(_tournament._matches
                .Where(x => x.PlayoffPosition is null && x.FinalsRound is not null)
                .Select(x => x.FinalsRound)
                .Distinct()
                .Select(fr => new RankingSection(_tournament, fr!.Value)));

            // Add a section for all teams that are not qualified for any finals round
            sections.Add(new RankingSection(_tournament));

            // The next ranking position, given all previously defined rankings
            var nextPosition = positionsTemporary.Count > 0
                ? positionsTemporary.Max(x => x.Position) + 1
                : 1;

            // Iterate over all sections, starting from the highest tier, and copy the rankings
            foreach (var section in sections.OrderByDescending(x => x.Tier))
            {
                if (section.IsDefined)
                {
                    var teamsAdded = 0;

                    foreach (var team in section.Teams)
                    {
                        positionsTemporary.Add(new RankingPosition(++nextPosition, team));
                        teamsAdded++;
                    }

                    if (teamsAdded != section.Size)
                    {
                        throw new TurnierplanException("Section contains a number of teams which is different from the specified size!");
                    }
                }
                else
                {
                    for (var i = 0; i < section.Size; i++)
                    {
                        positionsTemporary.Add(new RankingPosition(++nextPosition, null));
                    }
                }
            }
        }

        _positions.Clear();
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
