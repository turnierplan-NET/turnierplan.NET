using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.RoleAssignment;

public sealed class RoleAssignment<T> : Entity<Guid>
    where T : Entity<long>, IEntityWithRoleAssignments<T>
{
    internal RoleAssignment(T scope, Role role, Principal principal, string? description = null)
    {
        Id = Guid.NewGuid();
        Scope = scope;
        CreatedAt =  DateTime.UtcNow;
        Role = role;
        Principal = principal;
        Description = description ?? string.Empty;
    }

    internal RoleAssignment(Guid id, DateTime createdAt, Role role, Principal principal, string description)
    {
        Id = id;
        CreatedAt = createdAt;
        Role = role;
        Principal = principal;
        Description = description;
    }

    public override Guid Id { get; protected set; }

    public T Scope { get; internal set; } = null!;

    public DateTime CreatedAt { get; }

    public Role Role { get; }

    public Principal Principal { get; }

    public string Description { get; set; }
}
