using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Tournament;

/// <summary>
/// Internal class which is only used during the evaluation of the ranking.
/// </summary>
internal sealed class RankingSection
{
    /// <remarks>
    /// Creates the section for all nonqualified teams. Note that this constructor will perform invalid
    /// calculations if the tournament contains non-finished group matches!
    /// </remarks>
    public RankingSection(Tournament tournament)
    {
        Tier = int.MinValue; // this section is always at the bottom of the ranking

        var decidingMatches = tournament._matches.Where(x => x is { IsDecidingMatch: true }).ToList();
        List<Team> relevantTeams;

        if (decidingMatches.Count == 0)
        {
            // Tournament contains no deciding matches => all teams must be considered in this section
            relevantTeams = tournament._teams;
        }
        else
        {
            // Tournament contains deciding matches => all teams which do not participate in the first finals round must be considered
            var decidingMatchesWithFinalsRound = decidingMatches.Where(x => x.FinalsRound.HasValue).ToList();
            var highestFinalsRound = decidingMatchesWithFinalsRound.Count == 0 ? int.MaxValue : decidingMatchesWithFinalsRound.Max(x => x.FinalsRound!.Value);
            var matchesInFinalRound = decidingMatches.Where(x => x.PlayoffPosition.HasValue || x.FinalsRound == highestFinalsRound).ToList();
            relevantTeams = tournament._teams.Where(team => matchesInFinalRound.None(match => match.IsTeamParticipant(team))).ToList();
        }

        Size = relevantTeams.Count;
        IsDefined = true; // always true because we know that all group matches are finished
        Teams = [..SortTeams(tournament, relevantTeams, null)];
    }

    /// <remarks>
    /// Creates the section for all teams qualified for a specific round. Note that this constructor will
    /// perform invalid calculations if the tournament contains non-finished group matches!
    /// </remarks>
    public RankingSection(Tournament tournament, int finalsRound)
    {
        Tier = int.MaxValue - finalsRound; // a "higher" finals round corresponds to a lower "tier" of K/O match
        Size = 0; // start at 0 and increment below

        var relevantMatches = new HashSet<Match>();
        var relevantTeamsTop = new List<Team>();
        var relevantTeamsBottom = new List<Team>();

        foreach (var match in tournament._matches)
        {
            if (match.FinalsRound != finalsRound)
            {
                continue; // wrong finals round :/
            }

            // Check whether any other match in the tournament references the current match's winner/loser
            var otherMatches = tournament._matches.Where(x => x.IsDecidingMatch && x.FinalsRound != finalsRound).ToList();
            var includeWinnerInSection = otherMatches
                .None(x => (x.TeamSelectorA is MatchSelector { SelectionMode: MatchSelector.Mode.Winner } msA && msA.TargetMatchIndex == match.Index)
                    || (x.TeamSelectorB is MatchSelector { SelectionMode: MatchSelector.Mode.Winner } msB && msB.TargetMatchIndex == match.Index));
            var includeLoserInSection = otherMatches
                .None(x => (x.TeamSelectorA is MatchSelector { SelectionMode: MatchSelector.Mode.Loser } msA && msA.TargetMatchIndex == match.Index)
                    || (x.TeamSelectorB is MatchSelector { SelectionMode: MatchSelector.Mode.Loser } msB && msB.TargetMatchIndex == match.Index));

            if (includeWinnerInSection)
            {
                // This match's winner is not referenced by any other match => This match's winner must be placed in this section.
                // To be honest, this should never happen in a "normal" tournament because a deciding match winner typically
                // qualifies for the next round or for a specific ranking... But rather handle this case than throw some random exception.

                Size++; // always increase size even if the match is not finished => "IsDefined" is determined below

                if (match.IsFinished)
                {
                    relevantTeamsTop.Add(match.GetWinningTeam()!);
                    relevantMatches.Add(match);
                }
            }

            if (includeLoserInSection)
            {
                // This match's loser is not referenced by any other match => This match's loser must be placed in this section.

                Size++; // always increase size even if the match is not finished => "IsDefined" is determined below

                if (match.IsFinished)
                {
                    relevantTeamsTop.Add(match.GetLosingTeam()!);
                    relevantMatches.Add(match);
                }
            }
        }

        var totalRelevantTeamsCount = relevantTeamsTop.Count + relevantTeamsBottom.Count;

        if (totalRelevantTeamsCount < Size)
        {
            // not all team "slots" are defined
            IsDefined = false;
            Teams = null;
        }
        else if (totalRelevantTeamsCount == Size)
        {
            // all team "slots" are filled => the teams can be compared and ranked
            IsDefined = true;
            Teams = [..SortTeams(tournament, relevantTeamsTop, relevantMatches), ..SortTeams(tournament, relevantTeamsBottom, relevantMatches)];
        }
        else
        {
            // something went wrong :(
            throw new TurnierplanException("Illegal state detected while evaluating ranking section.");
        }
    }

    /// <summary>
    /// The relative weight of this section when comparing to other sections where a higher value represents a better ranking.
    /// </summary>
    public int Tier { get; }

    /// <summary>
    /// The size of this section measured in number of teams.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Whether all positions in this section can be evaluated based on the currently available match outcomes.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Teams))]
    public bool IsDefined { get; }

    /// <summary>
    /// The teams in this section, ordered from best to worst or <c>null</c> if this section is not defined.
    /// </summary>
    public ImmutableArray<Team>? Teams { get; }

    /// <summary>
    /// <para>
    /// Sorts a list of teams by the following criteria:
    /// </para>
    /// <para>
    /// If <paramref name="sortByMatches"/> is specified, the teams will primarily be ordered based on their success
    /// in the matches included in the set.
    /// </para>
    /// <para>
    /// If <paramref name="sortByMatches"/> is not specified <b>or</b> two teams cannot be ordered based on the match
    /// outcomes, they will be ordered based on their group statistics. The underlying <see cref="TeamComparer"/>
    /// always ensures a determinstic ordering: As a last resort, it uses the team's IDs.
    /// </para>
    /// </summary>
    private static IEnumerable<Team> SortTeams(Tournament tournament, List<Team> teams, HashSet<Match>? sortByMatches)
    {
        var comparer = new TeamComparer(tournament);

        return teams
            .Select(team =>
            {
                var participations = tournament._groups
                    .Select(group => group._participants.SingleOrDefault(participant => participant.Team == team))
                    .WhereNotNull()
                    .ToList();

                TeamGroupStatistics? sortByMatchesStatistics = null;

                if (sortByMatches is not null)
                {
                    sortByMatchesStatistics = new TeamGroupStatistics();

                    foreach (var match in sortByMatches)
                    {
                        if (!match.IsTeamParticipant(team))
                        {
                            continue;
                        }

                        var scoreFor = (match.TeamA == team ? match.ScoreA : match.ScoreB)!.Value;
                        var scoreAgainst = (match.TeamA == team ? match.ScoreB : match.ScoreA)!.Value;

                        sortByMatchesStatistics.AddMatchOutcome(scoreFor, scoreAgainst, tournament.ComputationConfiguration);
                    }
                }

                if (participations.Count == 0)
                {
                    return new TemporaryTeam
                    {
                        Team = team,
                        Statistics = new TeamGroupStatistics(),
                        SortByMatchesStatistics = sortByMatchesStatistics
                    };
                }

                return new TemporaryTeam
                {
                    Team = team,
                    Priority = participations.Max(x => x.Priority),
                    Statistics = participations.Select(x => x.Statistics).Combine(),
                    SortByMatchesStatistics = sortByMatchesStatistics
                };
            })
            .Order(comparer)
            .Select(x => x.Team);
    }

    private sealed record TemporaryTeam : IComparableTeam
    {
        public required Team Team { get; init; }

        public int Order => 0;

        public int Priority { get; init; }

        public required TeamGroupStatistics Statistics { get; init; }

        public required TeamGroupStatistics? SortByMatchesStatistics { get; init; }

        public bool HasAssociatedGroup => false;

        public Group? AssociatedGroup => null;
    }

    private sealed class TeamComparer(Tournament tournament) : IComparer<TemporaryTeam>
    {
        public int Compare(TemporaryTeam? x, TemporaryTeam? y)
        {
            if (Equals(x, y))
            {
                return 0;
            }

            if (x is not null && y is not null)
            {
                return CompareTeams(x, y);
            }

            return x is null ? 1 : -1;
        }

        private int CompareTeams(TemporaryTeam x, TemporaryTeam y)
        {
            if (x.SortByMatchesStatistics is not null && y.SortByMatchesStatistics is not null)
            {
                foreach (var mode in tournament.ComputationConfiguration.ComparisonModes)
                {
                    var diff = mode switch
                    {
                        TeamComparisonMode.ByPoints => y.SortByMatchesStatistics.Points - x.SortByMatchesStatistics.Points,
                        TeamComparisonMode.ByScoreDifference => y.SortByMatchesStatistics.ScoreDifference - x.SortByMatchesStatistics.ScoreDifference,
                        TeamComparisonMode.ByScore => y.SortByMatchesStatistics.ScoreFor - x.SortByMatchesStatistics.ScoreFor,
                        TeamComparisonMode.ByDirectComparison => 0, // Direct comparison is ignored in this case
                        _ => throw new TurnierplanException($"Invalid comparison mode specified: {mode}")
                    };

                    if (diff != 0)
                    {
                        return Math.Sign(diff);
                    }
                }
            }

            // In any other case, we fall through to a comparison based on the group matches
            return tournament._teamComparer.Compare(x, y);
        }
    }
}
