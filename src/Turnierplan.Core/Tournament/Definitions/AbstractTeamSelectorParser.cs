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

    internal static AbstractTeamSelector ParseAbstractTeamSelectorFromDefinitionFormat(string source)
    {
        var regex = AbstractTeamSelectorDefinitionFormatRegex();
        var match = regex.Match(source);

        if (!match.Success)
        {
            throw new InvalidOperationException($"Invalid abstract team selector: '{source}'");
        }

        if (match.Groups["GroupRef"].Success)
        {
            if (int.TryParse(match.Groups["PlacementRank"].Value, out var placementRank))
            {
                var groupRef = match.Groups["GroupRef"].Value.Single();

                if (char.IsLetter(groupRef) && placementRank >= 1)
                {
                    var upper = char.ToUpper(groupRef);
                    var groupIndex = upper - 'A';

                    return new AbstractTeamSelector(false, groupIndex, placementRank, null);
                }
            }
        }
        else if (int.TryParse(match.Groups["OrdinalNumber"].Value, out var ordinalNumber)
                 && int.TryParse(match.Groups["PlacementRank"].Value, out var placementRank)
                 && ordinalNumber >= 1
                 && placementRank >= 1)
        {
            // Subtract 1 from ordinal number because in definitions json the referenced rankings are provided on a 1.. range.
            return new AbstractTeamSelector(true, null, placementRank, ordinalNumber - 1);
        }

        throw new InvalidOperationException($"Invalid abstract team selector: '{source}'");
    }

    [GeneratedRegex(@"^(?:(?<PlacementRank>\d+)\.(?<GroupIndex>\d+)|(?<OrdinalNumber>\d)B(?<PlacementRank>\d))$")]
    private static partial Regex AbstractTeamSelectorExternalFormatRegex();

    [GeneratedRegex(@"^(?:(?<GroupRef>[A-Z])(?<PlacementRank>\d)|(?<OrdinalNumber>\d)B(?<PlacementRank>\d))$")]
    private static partial Regex AbstractTeamSelectorDefinitionFormatRegex();
}
