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

    private static string FormatPrincipal(Principal principal)
    {
        return $"{principal.Kind}:{principal.ObjectId}";
    }

    private static Principal ParsePrincipal(string input)
    {
        var match = PrincipalRegex().Match(input);

        if (!match.Success || !Enum.TryParse<PrincipalKind>(match.Groups["Kind"].Value, out var kind))
        {
            throw new TurnierplanException("Invalid principal string.");
        }

        return new Principal(kind, match.Groups["ObjectId"].Value);
    }

    [GeneratedRegex("^(?:(?<Kind>ApiKey):(?<ObjectId>\\d+))|(?:(?<Kind>User):(?<ObjectId>[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}))$")]
    private static partial Regex PrincipalRegex();
}
