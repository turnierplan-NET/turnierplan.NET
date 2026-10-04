namespace Turnierplan.Server.Domain.RoleAssignments;

internal sealed class RoleAssignment<T> : EntityBase<long>
    where T : EntityBase<long>
{
    public RoleAssignment(T scope, Role role, PrincipalKind principalKind, Guid principalId)
    {
        Id = 0;
        CreatedAt = DateTime.UtcNow;
        Scope = scope;
        Role = role;
        PrincipalKind = principalKind;
        PrincipalId = principalId;
    }

    public RoleAssignment(long id, DateTime createdAt, Role role, PrincipalKind principalKind, Guid principalId)
    {
        Id = id;
        CreatedAt = createdAt;
        Role = role;
        PrincipalKind = principalKind;
        PrincipalId = principalId;
    }

    public override long Id { get; protected set; }

    public DateTime CreatedAt { get; }

    public T Scope { get; internal set; } = null!;

    public Role Role { get; }

    public PrincipalKind PrincipalKind { get; }

    public Guid PrincipalId { get; }
}
