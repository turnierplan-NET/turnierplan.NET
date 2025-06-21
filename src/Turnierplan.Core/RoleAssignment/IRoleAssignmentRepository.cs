using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.RoleAssignment;

public interface IRoleAssignmentRepository<T> : IRepository<RoleAssignment<T>, Guid>
    where T : Entity, IEntityWithRoleAssignments<T>;
