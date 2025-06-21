using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Turnierplan.Core.Exceptions;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Dal.Converters;

internal sealed partial class PrincipalConverter : ValueConverter<Principal, string>
{
    public PrincipalConverter()
        : base(principal => FormatPrincipal(principal), str => ParsePrincipal(str))
    {
    }

    internal static string FormatPrincipal(Principal principal)
    {
        return $"{principal.Kind}:{principal.PrincipalId}";
    }

    internal static Principal ParsePrincipal(string input)
    {
        var match = PrincipalRegex().Match(input);

        if (!match.Success
            || !Enum.TryParse<PrincipalKind>(match.Groups["Kind"].Value, out var kind)
            || !Guid.TryParse(match.Groups["PrincipalId"].Value, out var principalId))
        {
            throw new TurnierplanException("Invalid principal string.");
        }

        return new Principal(kind, principalId);
    }

    [GeneratedRegex(@"^(?<Kind>ApiKey|User):(?<PrincipalId>[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})$")]
    private static partial Regex PrincipalRegex();
}
