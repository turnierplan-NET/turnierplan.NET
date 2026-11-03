using System.Text;
using Turnierplan.Core.TeamSelectors;

namespace Turnierplan.Core.Test.Regression;

internal static class SubjectSerializer
{
    public static void AppendSubject<TSubject>(this StringBuilder builder, TSubject subject)
    {
        switch (subject)
        {
            case Tournament tournament:
                AppendTournament(builder, tournament);
                break;
            default:
                throw new InvalidOperationException($"Subject type '{typeof(TSubject).FullName}' currently not supported for serialization.");
        }
    }

    private static void AppendTournament(StringBuilder builder, Tournament tournament)
    {
        builder.Append(tournament.Name);
        builder.Append(';');
        builder.Append(tournament.Teams.Count);
        builder.Append(';');
        builder.Append(tournament.Groups.Count);
        builder.Append(';');
        builder.Append(tournament.Matches.Count);
        builder.Append(';');
        builder.Append(tournament.RankingOverwrites.Count);
        builder.Append(';');
        builder.Append(tournament.Ranking.Count);
        builder.AppendLine();

        foreach (var team in tournament.Teams)
        {
            builder.Append('T');
            builder.Append(team.Name);
            builder.Append(';');
            builder.Append(team.OutOfCompetition);
            builder.Append(';');
            builder.Append(team.Statistics.ScoreFor);
            builder.Append(';');
            builder.Append(team.Statistics.ScoreAgainst);
            builder.Append(';');
            builder.Append(team.Statistics.MatchesWon);
            builder.Append(';');
            builder.Append(team.Statistics.MatchesDrawn);
            builder.Append(';');
            builder.Append(team.Statistics.MatchesLost);
            builder.Append(';');
            builder.Append(team.Statistics.MatchesPlayed);
            builder.AppendLine();
        }

        foreach (var group in tournament.Groups)
        {
            builder.Append('G');
            builder.Append(group.AlphabeticalId);
            builder.Append(';');
            builder.Append(group.DisplayName);
            builder.Append(';');
            builder.Append(group.Participants.Count);
            builder.AppendLine();

            foreach (var participant in group.Participants)
            {
                builder.Append('>');
                builder.Append(participant.Team.Name);
                builder.Append(';');
                builder.Append(participant.Group.AlphabeticalId);
                builder.Append(';');
                builder.Append(participant.Order);
                builder.Append(';');
                builder.Append(participant.Priority);
                builder.Append(';');
                builder.Append(participant.Statistics.Position);
                builder.Append(';');
                builder.Append(participant.Statistics.ScoreFor);
                builder.Append(';');
                builder.Append(participant.Statistics.ScoreAgainst);
                builder.Append(';');
                builder.Append(participant.Statistics.MatchesWon);
                builder.Append(';');
                builder.Append(participant.Statistics.MatchesDrawn);
                builder.Append(';');
                builder.Append(participant.Statistics.MatchesLost);
                builder.Append(';');
                builder.Append(participant.Statistics.Points);
                builder.Append(';');
                builder.Append(participant.Statistics.ScoreDifference);
                builder.Append(';');
                builder.Append(participant.Statistics.MatchesPlayed);
                builder.AppendLine();
            }
        }

        foreach (var match in tournament.Matches)
        {
            builder.Append('M');
            builder.Append(match.Index);
            builder.Append(';');
            builder.Append(match.Court);
            builder.Append(';');
            builder.Append(match.Kickoff?.Ticks);
            builder.Append(';');
            builder.Append(ConvertTeamSelectorToString(match.TeamSelectorA));
            builder.Append(';');
            builder.Append(ConvertTeamSelectorToString(match.TeamSelectorB));
            builder.Append(';');
            builder.Append(match.TeamA?.Name);
            builder.Append(';');
            builder.Append(match.TeamB?.Name);
            builder.Append(';');
            builder.Append(match.Group?.AlphabeticalId);
            builder.Append(';');
            builder.Append(match.IsGroupMatch);
            builder.Append(';');
            builder.Append(match.IsDecidingMatch);
            builder.Append(';');
            builder.Append(match.PlayoffPosition);
            builder.Append(';');
            builder.Append(match.IsCurrentlyPlaying);
            builder.Append(';');
            builder.Append(match.ScoreA);
            builder.Append(';');
            builder.Append(match.ScoreB);
            builder.Append(';');
            builder.Append(match.OutcomeType);
            builder.Append(';');
            builder.Append(match.IsFinished);
            builder.AppendLine();
        }

        foreach (var overwrite in tournament.RankingOverwrites)
        {
            builder.Append("RO");
            builder.Append(overwrite.PlacementRank);
            builder.Append(';');
            builder.Append(overwrite.HideRanking);
            builder.Append(';');
            builder.Append(overwrite.AssignTeam?.Name);
            builder.AppendLine();
        }

        foreach (var ranking in tournament.Ranking)
        {
            builder.Append('R');
            builder.Append(ranking.Position);
            builder.Append(';');
            builder.Append(ranking.Reason);
            builder.Append(';');
            builder.Append(ranking.IsDefined);
            builder.Append(';');
            builder.Append(ranking.Team?.Name);
            builder.AppendLine();
        }
    }

    private static string ConvertTeamSelectorToString(TeamSelectorBase input)
    {
        switch (input)
        {
            case GroupDefinitionSelector groupDefinitionSelector:
                return $"G{groupDefinitionSelector.TargetGroup.AlphabeticalId}/{groupDefinitionSelector.TargetTeamIndex}";
            case GroupResultsNthRankedSelector groupResultsNthRankedSelector:
                var groupIds = string.Join(",", groupResultsNthRankedSelector.TargetGroups.Select(g => g.AlphabeticalId));
                return $"N{groupIds}/{groupResultsNthRankedSelector.OrdinalNumber}/{groupResultsNthRankedSelector.PlacementRank}";
            case GroupResultsSelector groupResultsSelector:
                return $"R{groupResultsSelector.TargetGroup.AlphabeticalId}/{groupResultsSelector.TargetGroupPosition}";
            case MatchSelector matchSelector:
                return $"{(matchSelector.SelectionMode is MatchSelector.Mode.Winner ? "W" : "L")}{matchSelector.TargetMatch.Index}";
            case StaticTeamSelector staticTeamSelector:
                return $"T{staticTeamSelector.TargetTeam.Name}";
            default:
                return string.Empty;
        }
    }
}
