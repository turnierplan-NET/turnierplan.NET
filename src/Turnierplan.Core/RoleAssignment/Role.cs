namespace Turnierplan.Core.RoleAssignment;

public enum Role
{
    // Note: Don't change enum values (DB serialization)

    #region General Roles

    /// <summary>
    /// This role grants all permissions on the target scope including the rights
    /// to delete the entity and modify role assignments.
    /// </summary>
    Owner = 1000,

    /// <summary>
    /// This role grants all permissions on the target scope excluding the rights
    /// to delete the entity and modify role assignments.
    /// </summary>
    Contributor = 1001,

    /// <summary>
    /// This role grants the permission to view the target entity but not to make modifications.
    /// </summary>
    Reader = 1002,

    #endregion

    #region Tournament Roles

    /// <summary>
    /// This role grants the permission to perform all actions in a tournament which
    /// are necessary while the tournament is played.
    /// </summary>
    Reporter = 2000

    #endregion
}
