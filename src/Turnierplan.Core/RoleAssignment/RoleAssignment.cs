using Turnierplan.Core.Entity;

namespace Turnierplan.Core.RoleAssignment;

public sealed class RoleAssignment<T> : Entity<Guid>
    where T : Entity<long>, IEntityWithRoleAssignments<T>
{
    internal RoleAssignment(T scope, Role role, Principal principal)
    {
        Id = Guid.NewGuid();
        Scope = scope;
        CreatedAt =  DateTime.UtcNow;
        Role = role;
        Principal = principal;
    }

    internal RoleAssignment(Guid id, DateTime createdAt, Role role, Principal principal)
    {
        Id = id;
        CreatedAt = createdAt;
        Role = role;
        Principal = principal;
    }

    public override Guid Id { get; protected set; }

    public T Scope { get; internal set; } = null!;

    public DateTime CreatedAt { get; }

    public Role Role { get; }

    public Principal Principal { get; }
}
