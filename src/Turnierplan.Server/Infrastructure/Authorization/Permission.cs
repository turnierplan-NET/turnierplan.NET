using Turnierplan.Server.Domain.RoleAssignments;

namespace Turnierplan.Server.Infrastructure.Authorization;

/// <remarks>
/// The contents of this source file are parsed using regex during the build of the Angular client application. Because
/// of this, the formatting must be consistent with the regular expression such that all actions are read properly.
/// Refer to the 'Client/actions.gen.js' file for implementation details including the regex.
/// </remarks>
internal enum Permission
{
    /// <summary>
    /// Any action that reads or writes role assignments of some entity.
    /// </summary>
    [GrantedByRoles(Role.Owner)]
    ReadOrWriteRoleAssignments,

    /// <summary>
    /// A special kind of delete action which shall require the <see cref="Role.Owner"/> role on the target entity.
    /// </summary>
    [GrantedByRoles(Role.Owner)]
    PrivilegedDelete,

    /// <summary>
    /// A special kind of write action which shall require the <see cref="Role.Owner"/> role on the target entity.
    /// </summary>
    [GrantedByRoles(Role.Owner)]
    PrivilegedWrite,

    /// <summary>
    /// Any action that modifies some entity.
    /// </summary>
    [GrantedByRoles(Role.Owner, Role.Contributor)]
    GenericWrite,

    /// <summary>
    /// Any action that reads information about some entity.
    /// </summary>
    [GrantedByRoles(Role.Owner, Role.Contributor, Role.Reader)]
    GenericRead,

    /// <summary>
    /// Any action that modifies a tournament in a way that is required during "playtime".
    /// </summary>
    [GrantedByRoles(Role.Owner, Role.Contributor, Role.Reporter)]
    TournamentConduct,

    /// <summary>
    /// Any action that adds or modifies applications in a tournament planner.
    /// </summary>
    [GrantedByRoles(Role.Owner, Role.Contributor, Role.ApplicationOfficer)]
    ApplicationsWrite,

    /// <summary>
    /// Any action that queries applications in a tournament planner.
    /// </summary>
    [GrantedByRoles(Role.Owner, Role.Contributor, Role.ApplicationOfficer, Role.ApplicationReader)]
    ApplicationsRead
}
