using System.Diagnostics.CodeAnalysis;
using Turnierplan.Core.Entity;
using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Tournament;

public sealed class Match : Entity<int>
{
    internal Match(int id, int index, TeamSelectorBase teamSelectorA, TeamSelectorBase teamSelectorB, Group? group)
    {
        Id = id;
        Index = index;
        Court = 0;
        Kickoff = null;
        TeamSelectorA = teamSelectorA;
        TeamSelectorB = teamSelectorB;
        TeamA = null;
        TeamB = null;
        Group = group;
        FinalsRound = null;
        PlayoffPosition = null;
        ScoreA = null;
        ScoreB = null;
        OutcomeType = null;
    }

    internal Match(int id, int index, TeamSelectorBase teamSelectorA, TeamSelectorBase teamSelectorB, int? finalsRound, int? playoffPosition)
    {
        Id = id;
        Index = index;
        Court = 0;
        Kickoff = null;
        TeamSelectorA = teamSelectorA;
        TeamSelectorB = teamSelectorB;
        TeamA = null;
        TeamB = null;
        Group = null;
        FinalsRound = finalsRound;
        PlayoffPosition = playoffPosition;
        ScoreA = null;
        ScoreB = null;
        OutcomeType = null;
    }

    public override int Id { get; protected set; }

    public int Index { get; internal set; }

    public short Court { get; set; }

    public DateTime? Kickoff { get; set; }

    public TeamSelectorBase TeamSelectorA { get; internal set; }

    public TeamSelectorBase TeamSelectorB { get; internal set; }

    public Team? TeamA { get; internal set; }

    public Team? TeamB { get; internal set; }

    [MemberNotNullWhen(true, nameof(IsGroupMatch))]
    public Group? Group { get; internal set; }

    public int? FinalsRound { get; }

    public int? PlayoffPosition { get; }

    public bool? IsCurrentlyPlaying { get; internal set; }

    public int? ScoreA { get; internal set; }

    public int? ScoreB { get; internal set; }

    public MatchOutcomeType? OutcomeType { get; internal set; }

    public bool IsGroupMatch => Group is not null;

    public bool IsDecidingMatch => FinalsRound is not null || PlayoffPosition is not null;

    public bool IsFinished => IsCurrentlyPlaying == false && ScoreA is not null && ScoreB is not null && OutcomeType is not null;

    public void SetOutcome(bool isCurrentlyPlaying, int scoreA, int scoreB, MatchOutcomeType outcomeType)
    {
        if (!isCurrentlyPlaying && IsDecidingMatch && scoreA == scoreB)
        {
            throw new TurnierplanException("Deciding matches cannot end with a draw.");
        }

        IsCurrentlyPlaying = isCurrentlyPlaying;
        ScoreA = scoreA;
        ScoreB = scoreB;
        OutcomeType = outcomeType;
    }

    public void ResetOutcome()
    {
        IsCurrentlyPlaying = null;
        ScoreA = null;
        ScoreB = null;
        OutcomeType = null;
    }

    public bool IsTeamParticipant(Team team)
    {
        return TeamA == team || TeamB == team;
    }

    public bool AreBothTeamsParticipant(Team teamA, Team teamB)
    {
        return IsTeamParticipant(teamA) && IsTeamParticipant(teamB);
    }

    public Team? GetWinningTeam()
    {
        if (IsCurrentlyPlaying == true || ScoreA is null || ScoreB is null || TeamA is null || TeamB is null || TeamA == TeamB)
        {
            return null;
        }

        if (ScoreA > ScoreB)
        {
            return TeamA;
        }

        if (ScoreB > ScoreA)
        {
            return TeamB;
        }

        return null;
    }

    public Team? GetLosingTeam()
    {
        if (IsCurrentlyPlaying == true || ScoreA is null || ScoreB is null || TeamA is null || TeamB is null || TeamA == TeamB)
        {
            return null;
        }

        if (ScoreA > ScoreB)
        {
            return TeamB;
        }

        if (ScoreB > ScoreA)
        {
            return TeamA;
        }

        return null;
    }

    public int? GetScoreDifference()
    {
        if (ScoreA is null || ScoreB is null)
        {
            return null;
        }

        return Math.Abs(ScoreA.Value - ScoreB.Value);
    }
}
