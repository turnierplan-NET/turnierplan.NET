using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.SeedWork;

public interface IEntityWithRoleAssignments<T>
    where T : Entity<long>, IEntityWithRoleAssignments<T>
{
    RoleAssignment<T> AddRoleAssignment(Role role, Principal principal, string? description = null);
}
