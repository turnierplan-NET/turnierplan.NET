namespace Turnierplan.Core.RoleAssignment;

public enum Role
{
    /// <summary>
    /// This role grants all permissions on the target scope including the rights
    /// to delete the entity and modify role assignments.
    /// </summary>
    Owner,

    /// <summary>
    /// This role grants all permissions on the target scope excluding the rights
    /// to delete the entity and modify role assignments.
    /// </summary>
    Contributor,

    /// <summary>
    /// This role grants the permission to view the target entity but not to make modifications.
    /// </summary>
    Reader
}
