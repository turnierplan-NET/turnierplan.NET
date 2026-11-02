using System.Collections.Immutable;
using Turnierplan.Server.Domain.RoleAssignments;

namespace Turnierplan.Server.Infrastructure.Authorization;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class GrantedByRolesAttribute : Attribute
{
    public ImmutableArray<Role> RequiresAnyOf { get; }

    public GrantedByRolesAttribute(params Role[] anyOf)
    {
        if (anyOf.Length == 0)
        {
            throw new ArgumentException("The required roles array must not be empty.");
        }

        RequiresAnyOf = [.. anyOf];
    }
}
