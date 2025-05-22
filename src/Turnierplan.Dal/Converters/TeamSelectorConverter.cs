using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Dal.Converters;

public sealed partial class TeamSelectorConverter : ValueConverter<TeamSelectorBase, string>
{
    internal TeamSelectorConverter()
        : base(ts => ConvertTeamSelectorToString(ts), str => ConvertStringToTeamSelector(str))
    {
    }

    public static string ConvertTeamSelectorToString(TeamSelectorBase input)
    {
        switch (input)
        {
            case GroupDefinitionSelector groupDefinitionSelector:
                return $"G{groupDefinitionSelector.TargetGroupId}/{groupDefinitionSelector.TargetTeamIndex}";
            case GroupResultsNthRankedSelector groupResultsNthRankedSelector:
                var groupIds = string.Join(",", groupResultsNthRankedSelector.TargetGroupIds);
                return $"N{groupIds}/{groupResultsNthRankedSelector.OrdinalNumber}/{groupResultsNthRankedSelector.PlacementRank}";
            case GroupResultsSelector groupResultsSelector:
                return $"R{groupResultsSelector.TargetGroupId}/{groupResultsSelector.TargetGroupPosition}";
            case MatchSelector matchSelector:
                return $"{(matchSelector.SelectionMode is MatchSelector.Mode.Winner ? "W" : "L")}{matchSelector.TargetMatchIndex}";
            case StaticTeamSelector staticTeamSelector:
                return $"T{staticTeamSelector.TargetTeamId}";
            default:
                return string.Empty;
        }
    }

    public static TeamSelectorBase ConvertStringToTeamSelector(string input)
    {
        if (input.Length == 0)
        {
            throw new TurnierplanException("Invalid team selector string.");
        }

        Match match;
        int targetGroupId;

        switch (input[0])
        {
            case 'G':
                match = GroupDefinitionSelectorPattern().Match(input);

                if (match.Success
                        && int.TryParse(match.Groups["TargetGroupId"].Value, out targetGroupId)
                        && int.TryParse(match.Groups["TargetTeamIndex"].Value, out var targetTeamIndex))
                {
                    return new GroupDefinitionSelector(targetGroupId, targetTeamIndex);
                }

                break;
            case 'N':
                match = GroupResultsNthRankedSelectorPattern().Match(input);

                if (match.Success
                        && int.TryParse(match.Groups["OrdinalNumber"].Value, out var ordinalNumber)
                        && int.TryParse(match.Groups["PlacementRank"].Value, out var placementRank))
                {
                    var targetGroupIds = match.Groups["TargetGroupIds"].Value.Split(',');
                    var numericGroupIds = new int[targetGroupIds.Length];
                    var isInvalid = false;

                    for (var i = 0; i < targetGroupIds.Length; i++)
                    {
                        if (int.TryParse(targetGroupIds[i], out var groupId))
                        {
                            numericGroupIds[i] = groupId;
                        }
                        else
                        {
                            isInvalid = true;
                            break;
                        }
                    }

                    if (isInvalid)
                    {
                        break;
                    }

                    return new GroupResultsNthRankedSelector(numericGroupIds, ordinalNumber, placementRank);
                }

                break;
            case 'R':
                match = GroupResultsSelectorPattern().Match(input);

                if (match.Success
                        && int.TryParse(match.Groups["TargetGroupId"].Value, out targetGroupId)
                        && int.TryParse(match.Groups["TargetGroupPosition"].Value, out var targetGroupPosition))
                {
                    return new GroupResultsSelector(targetGroupId, targetGroupPosition);
                }

                break;
            case 'W':
            case 'L':
                match = MatchSelectorPattern().Match(input);

                if (match.Success && int.TryParse(match.Groups["TargetMatchIndex"].Value, out var targetMatchIndex))
                {
                    var mode = match.Groups["Mode"].Value == "W" ? MatchSelector.Mode.Winner : MatchSelector.Mode.Loser;

                    return new MatchSelector(targetMatchIndex, mode);
                }

                break;
            case 'T':
                match = StaticTeamSelectorPattern().Match(input);

                if (match.Success && int.TryParse(match.Groups["TargetTeamId"].Value, out var targetTeamId))
                {
                    return new StaticTeamSelector(targetTeamId);
                }

                break;
        }

        throw new TurnierplanException($"Malformed team selector: '{input}'");
    }

    [GeneratedRegex(@"^G(?<TargetGroupId>\d+)/(?<TargetTeamIndex>\d+)$")]
    private static partial Regex GroupDefinitionSelectorPattern();

    [GeneratedRegex(@"^N(?<TargetGroupIds>\d+(?:,\d+)*)/(?<OrdinalNumber>\d+)/(?<PlacementRank>\d+)$")]
    private static partial Regex GroupResultsNthRankedSelectorPattern();

    [GeneratedRegex(@"^R(?<TargetGroupId>\d+)/(?<TargetGroupPosition>\d+)$")]
    private static partial Regex GroupResultsSelectorPattern();

    [GeneratedRegex(@"^(?<Mode>[WL])(?<TargetMatchIndex>\d+)$")]
    private static partial Regex MatchSelectorPattern();

    [GeneratedRegex(@"^T(?<TargetTeamId>\d+)$")]
    private static partial Regex StaticTeamSelectorPattern();
}
