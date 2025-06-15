using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.App.Helpers;

internal static partial class RbacScopeHelper
{
    public static string GetScopeId<T>(this T entity)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        return $"{typeof(T).Name}:{entity.PublicId.ToString()}";
    }

    public static bool TryParseScopeId(string scopeId, [NotNullWhen(true)] out string? objectTypeName, out PublicId targetObjectId)
    {
        var match = ScopeIdRegex().Match(scopeId);

        if (!match.Success || !PublicId.TryParse(match.Groups["ObjectId"].Value, out targetObjectId))
        {
            objectTypeName = null;
            targetObjectId = PublicId.Empty;

            return false;
        }

        objectTypeName = match.Groups["TypeName"].Value;

        return true;
    }

    [GeneratedRegex(@"^(?<TypeName>ApiKey|Folder|Image|Organization|Tournament|Venue):(?<ObjectId>[A-Za-z0-9_-]{11})$")]
    public static partial Regex ScopeIdRegex();
}
