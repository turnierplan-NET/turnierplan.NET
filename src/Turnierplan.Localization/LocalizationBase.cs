using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Localization;

public abstract class LocalizationBase : ILocalization
{
    public abstract string Get(string key);

    public abstract string Get(string key, object arg1);

    public abstract string Get(string key, object arg1, object arg2);

    public abstract string Get(string key, object arg1, object arg2, object arg3);

    public abstract string Get(string key, params object[] args);

    public string LocalizeGroupName(Group group)
    {
        return group.DisplayName ?? Get("Documents.Common.DefaultGroupName", group.AlphabeticalId);
    }

    public string LocalizeMatchDisplayName(Match match)
    {
        if (match.IsGroupMatch)
        {
            return Get("Documents.Common.MatchTypes.GroupMatch");
        }

        if (match.PlayoffPosition == 1)
        {
            return Get("Documents.Common.MatchTypes.Final");
        }

        if (match.PlayoffPosition is not null)
        {
            return Get("Documents.Common.MatchTypes.RankingMatch", match.PlayoffPosition);
        }

        return match.FinalsRound switch
        {
            (int)FinalsRoundOrder.SemiFinals => Get("Documents.Common.MatchTypes.SemiFinal"),
            (int)FinalsRoundOrder.QuarterFinals => Get("Documents.Common.MatchTypes.QuarterFinal"),
            (int)FinalsRoundOrder.EighthFinals => Get("Documents.Common.MatchTypes.EighthFinal"),
            not null => Get("Documents.Common.MatchTypes.XFinal", Math.Pow(2, match.FinalsRound.Value)),
            _ => string.Empty
        };
    }

    public string LocalizeTeamSelector(TeamSelectorBase teamSelector, Tournament tournament)
    {
        Group? group;
        string groupName;

        switch (teamSelector)
        {
            case GroupDefinitionSelector groupDefinitionSelector:
                group = tournament.Groups.FirstOrDefault(x => x.Id == groupDefinitionSelector.TargetGroupId);
                groupName = group is null ? $"{groupDefinitionSelector.TargetGroupId}" : LocalizeGroupName(group);
                return Get("Documents.Common.TeamSelectors.GroupDefinition", groupDefinitionSelector.TargetTeamIndex + 1, groupName);
            case GroupResultsNthRankedSelector groupResultsNthRankedSelector:
                var ordinal = groupResultsNthRankedSelector.OrdinalNumber + 1;
                var ranking = groupResultsNthRankedSelector.PlacementRank;
                var ordinalText = ordinal <= 4
                    ? Get($"Documents.Common.TeamSelectors.NthRanked.Ordinals.{ordinal}")
                    : Get("Documents.Common.TeamSelectors.NthRanked.Ordinals.X", ordinal);
                var rankingText = ranking <= 5
                    ? Get($"Documents.Common.TeamSelectors.NthRanked.Rankings.{ranking}")
                    : Get("Documents.Common.TeamSelectors.NthRanked.Rankings.X", ranking);
                return Get("Documents.Common.TeamSelectors.NthRanked.Combined", ordinalText, rankingText);
            case GroupResultsSelector groupResultsSelector:
                group = tournament.Groups.FirstOrDefault(x => x.Id == groupResultsSelector.TargetGroupId);
                groupName = group is null ? $"{groupResultsSelector.TargetGroupId}" : LocalizeGroupName(group);
                return Get("Documents.Common.TeamSelectors.GroupResults", groupResultsSelector.TargetGroupPosition, groupName);
            case MatchSelector { SelectionMode: MatchSelector.Mode.Winner } winnerMatchSelector:
                return Get("Documents.Common.TeamSelectors.MatchWinner", winnerMatchSelector.TargetMatchIndex);
            case MatchSelector { SelectionMode: MatchSelector.Mode.Loser } loserMatchSelector:
                return Get("Documents.Common.TeamSelectors.MatchLoser", loserMatchSelector.TargetMatchIndex);
            case StaticTeamSelector staticTeamSelector:
                var team = tournament.Teams.FirstOrDefault(x => x.Id == staticTeamSelector.TargetTeamId);
                return team?.Name ?? $"{staticTeamSelector.TargetTeamId}";
            default:
                throw new NotSupportedException($"Team selector of type '{teamSelector.GetType().Name}' cannot be converted to translated string.");
        }
    }
}
