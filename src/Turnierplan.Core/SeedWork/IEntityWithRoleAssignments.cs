using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.SeedWork;

public interface IEntityWithRoleAssignments<T>
    where T : Entity<long>, IEntityWithRoleAssignments<T>
{
    IReadOnlyList<RoleAssignment<T>> RoleAssignments { get; }

    RoleAssignment<T> AddRoleAssignment(Role role, Principal principal, string? description = null);
}
