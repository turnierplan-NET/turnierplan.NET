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

        var decidingMatches = tournament._matches.Where(x => x is { IsDecidingMatch: true, PlayoffPosition: null }).ToList();
        List<Team> relevantTeams;

        if (decidingMatches.Count == 0)
        {
            // Tournament contains no deciding matches => all teams must be considered in this section
            relevantTeams = tournament._teams;
        }
        else
        {
            // Tournament contains deciding matches => all teams which do not participate in the first finals round must be considered
            var highestFinalsRound = decidingMatches.Max(x => x.FinalsRound!.Value);
            var matchesInFinalRound = decidingMatches.Where(x => x.FinalsRound == highestFinalsRound).ToList();
            relevantTeams = tournament._teams.Where(team => matchesInFinalRound.None(match => match.IsTeamParticipant(team))).ToList();
        }

        Size = relevantTeams.Count;
        IsDefined = true; // always true because we know that all group matches are finished
        Teams = [..SortTeams(tournament, relevantTeams)];
    }

    /// <remarks>
    /// Creates the section for all teams qualified for a specific round. Note that this constructor will
    /// perform invalid calculations if the tournament contains non-finished group matches!
    /// </remarks>
    public RankingSection(Tournament tournament, int finalsRound)
    {
        Tier = int.MaxValue - finalsRound; // a "higher" finals round corresponds to a lower "tier" of K/O match
        Size = 0; // start at 0 and increment below

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
                .None(x => (x.TeamSelectorA is MatchSelector { SelectionMode: MatchSelector.Mode.Winner } msA && msA.TargetMatchIndex == x.Index)
                    || (x.TeamSelectorB is MatchSelector { SelectionMode: MatchSelector.Mode.Winner } msB && msB.TargetMatchIndex == x.Index));
            var includeLoserInSection = otherMatches
                .None(x => (x.TeamSelectorA is MatchSelector { SelectionMode: MatchSelector.Mode.Loser } msA && msA.TargetMatchIndex == x.Index)
                    || (x.TeamSelectorB is MatchSelector { SelectionMode: MatchSelector.Mode.Loser } msB && msB.TargetMatchIndex == x.Index));

            if (!includeWinnerInSection)
            {
                // This match's winner is not referenced by any other match => This match's winner must be placed in this section.
                // To be honest, this should never happen in a "normal" tournament because a deciding match winner typically
                // qualifies for the next round or for a specific ranking... But rather handle this case than throw some random exception.

                Size++; // always increase size even if the match is not finished => "IsDefined" is determined below

                if (match.IsFinished)
                {
                    relevantTeamsTop.Add(match.GetWinningTeam()!);
                }
            }

            if (!includeLoserInSection)
            {
                // This match's loser is not referenced by any other match => This match's loser must be placed in this section.

                Size++; // always increase size even if the match is not finished => "IsDefined" is determined below

                if (match.IsFinished)
                {
                    relevantTeamsTop.Add(match.GetLosingTeam()!);
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
            Teams = [..SortTeams(tournament, relevantTeamsTop), ..SortTeams(tournament, relevantTeamsBottom)];
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
    /// Sorts a list of teams by their relative group statistics and returns the result.
    /// </summary>
    private static IEnumerable<Team> SortTeams(Tournament tournament, List<Team> teams)
    {
        return teams
            .Select(team =>
            {
                var participations = tournament._groups
                    .Select(group => group._participants.SingleOrDefault(participant => participant.Team == team))
                    .WhereNotNull()
                    .ToList();

                if (participations.Count == 0)
                {
                    return null;
                }

                var source = participations[0];
                var priority = participations.Max(x => x.Priority);
                var combinedGroupStats = new GroupParticipant(source.Group, source.Team, 0, priority)
                {
                    Statistics = participations.Select(x => x.Statistics).Combine()
                };

                return combinedGroupStats;
            })
            .WhereNotNull()
            .Order(tournament._groupParticipantComparer)
            .Select(x => x.Team);
    }
}
