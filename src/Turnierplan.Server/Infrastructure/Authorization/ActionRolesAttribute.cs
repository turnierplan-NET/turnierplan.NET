using System.Collections.Immutable;
using Turnierplan.Server.Domain.RoleAssignments;

namespace Turnierplan.Server.Infrastructure.Authorization;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class ActionRolesAttribute : Attribute
{
    public ImmutableArray<Role> RequiresAnyOf { get; }

    public ActionRolesAttribute(params Role[] requiresAnyOf)
    {
        if (requiresAnyOf.Length == 0)
        {
            throw new ArgumentException("The required roles array must not be empty.");
        }

        RequiresAnyOf = [.. requiresAnyOf];
    }
}
