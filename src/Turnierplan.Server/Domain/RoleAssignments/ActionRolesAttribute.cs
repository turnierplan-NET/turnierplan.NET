using System.Collections.Immutable;

namespace Turnierplan.Server.Domain.RoleAssignments;

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
