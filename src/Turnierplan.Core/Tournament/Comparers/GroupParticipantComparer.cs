using Turnierplan.Core.Exceptions;

namespace Turnierplan.Core.Tournament.Comparers;

internal sealed class GroupParticipantComparer : IComparer<GroupParticipant>
{
    private readonly Tournament _tournament;

    public GroupParticipantComparer(Tournament tournament)
    {
        _tournament = tournament;
    }

    public int Compare(GroupParticipant? x, GroupParticipant? y)
    {
        if (Equals(x, y))
        {
            return 0;
        }

        if (x is not null && y is not null)
        {
            return CompareParticipants(x, y);
        }

        return x is null ? 1 : -1;
    }

    private int CompareParticipants(GroupParticipant x, GroupParticipant y)
    {
        if (x.Team.OutOfCompetition && !y.Team.OutOfCompetition)
        {
            return 1;
        }

        if (!x.Team.OutOfCompetition && y.Team.OutOfCompetition)
        {
            return -1;
        }

        foreach (var mode in _tournament.ComputationConfiguration.ComparisonModes)
        {
            var diff = Math.Sign(CompareParticipants(x, y, mode));

            if (diff != 0)
            {
                return diff;
            }
        }

        if (x.Priority != y.Priority)
        {
            return Math.Sign(y.Priority - x.Priority);
        }

        if (x.Order != y.Order)
        {
            return Math.Sign(x.Order - y.Order);
        }

        // As a fallback, compare the team ids to guarantee a deterministic result.
        return x.Team.Id - y.Team.Id;
    }

    private int CompareParticipants(GroupParticipant x, GroupParticipant y, TeamComparisonMode mode)
    {
        return mode switch
        {
            TeamComparisonMode.ByPoints => y.Statistics.Points - x.Statistics.Points,
            TeamComparisonMode.ByScoreDifference => y.Statistics.ScoreDifference - x.Statistics.ScoreDifference,
            TeamComparisonMode.ByScore => y.Statistics.ScoreFor - x.Statistics.ScoreFor,
            TeamComparisonMode.ByDirectComparison => CompareByDirectComparison(x, y),
            _ => throw new TurnierplanException($"Invalid comparison mode specified: {mode}")
        };
    }

    private int CompareByDirectComparison(GroupParticipant x, GroupParticipant y)
    {
        if (x.Group.Id != y.Group.Id)
        {
            return 0;
        }

        var matches = _tournament._matches
            .Where(match => match.AreBothTeamsParticipant(x.Team, y.Team) && match.Group == x.Group && match.IsFinished)
            .ToList();

        if (matches.Count == 0)
        {
            return 0;
        }

        var winsTeamX = 0;
        var winsTeamY = 0;
        var scoreDifferenceTeamX = 0;

        foreach (var match in matches)
        {
            if (!match.AreBothTeamsParticipant(x.Team, y.Team))
            {
                throw new TurnierplanException("Group context returned direct comparison match with non-matching team ids.");
            }

            var winningTeam = match.GetWinningTeam();

            if (winningTeam == x.Team)
            {
                winsTeamX++;
                scoreDifferenceTeamX += match.GetScoreDifference() ?? 0;
            }
            else if (winningTeam == y.Team)
            {
                winsTeamY++;
                scoreDifferenceTeamX -= match.GetScoreDifference() ?? 0;
            }
        }

        if (winsTeamX > winsTeamY)
        {
            return -1;
        }

        if (winsTeamY > winsTeamX)
        {
            return 1;
        }

        return -Math.Sign(scoreDifferenceTeamX);
    }
}
