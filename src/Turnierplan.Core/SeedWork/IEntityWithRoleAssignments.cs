using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.SeedWork;

public interface IEntityWithRoleAssignments<T> : IEntityWithPublicId
    where T : Entity, IEntityWithRoleAssignments<T>
{
    string Name { get; }

    IReadOnlyList<RoleAssignment<T>> RoleAssignments { get; }

    RoleAssignment<T> AddRoleAssignment(Role role, Principal principal, string? description = null);

    void RemoveRoleAssignment(RoleAssignment<T> roleAssignment);
}
