using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.SeedWork;

public interface IEntityWithRoleAssignments<T> : IEntityWithPublicId
    where T : Entity<long>, IEntityWithRoleAssignments<T>
{
    string Name { get; }

    IReadOnlyList<RoleAssignment<T>> RoleAssignments { get; }

    RoleAssignment<T> AddRoleAssignment(Role role, Principal principal);

    void RemoveRoleAssignment(RoleAssignment<T> roleAssignment);
}
