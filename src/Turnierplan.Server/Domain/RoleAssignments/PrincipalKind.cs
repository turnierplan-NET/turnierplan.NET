namespace Turnierplan.Server.Domain.RoleAssignments;

public enum PrincipalKind
{
    // Note: Don't change enum values (DB serialization)

    ApiKey = 1,
    User = 2
}
