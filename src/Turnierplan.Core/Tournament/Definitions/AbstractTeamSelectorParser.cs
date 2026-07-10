using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Turnierplan.Core.Exceptions;

namespace Turnierplan.Core.Tournament.Definitions;

/// <remarks>
/// The FinalsMatchDefinitions.json uses a modified string representation for the abstract team selectors because the
/// definitions file should be easily readable and the "standard" string representation format is not particularly intuitive.
/// </remarks>
public static partial class AbstractTeamSelectorParser
{
    public static AbstractTeamSelector ParseAbstractTeamSelector(string? source)
    {
        return TryParseAbstractTeamSelector(source, out var result)
            ? result
            : throw new TurnierplanException($"The abstract team selector '{source ?? "<null>"}' is not valid.");
    }

    public static bool TryParseAbstractTeamSelector(string? source, [NotNullWhen(true)] out AbstractTeamSelector? result)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            result = null;
            return false;
        }

        var regex = AbstractTeamSelectorExternalFormatRegex();
        var match = regex.Match(source);

        if (!match.Success)
        {
            result = null;
            return false;
        }

        if (match.Groups["GroupIndex"].Success
            && int.TryParse(match.Groups["GroupIndex"].Value, out var groupIndex)
            && int.TryParse(match.Groups["PlacementRank"].Value, out var placementRank)
            && groupIndex >= 0
            && placementRank >= 1)
        {
            result = new AbstractTeamSelector(false, groupIndex, placementRank, null);
            return true;
        }

        if (match.Groups["OrdinalNumber"].Success
                && int.TryParse(match.Groups["OrdinalNumber"].Value, out var ordinalNumber)
                && int.TryParse(match.Groups["PlacementRank"].Value, out placementRank)
                && ordinalNumber >= 0
                && placementRank >= 1)
        {
            result = new AbstractTeamSelector(true, null, placementRank, ordinalNumber);
            return true;
        }

        result = null;
        return false;
    }

    [GeneratedRegex(@"^(?:(?<PlacementRank>\d+)\.(?<GroupIndex>\d+)|(?<OrdinalNumber>\d)B(?<PlacementRank>\d))$")]
    private static partial Regex AbstractTeamSelectorExternalFormatRegex();
}
