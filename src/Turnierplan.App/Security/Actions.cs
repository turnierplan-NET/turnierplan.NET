using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.App.Security;

/// <remarks>
/// The contents of this source file are parsed using regex during the build of the Angular client application. Because
/// of this, the formatting must be consistent with the regular expression such that all actions are read properly.
/// Refer to the 'Client/actions.gen.js' file for implementation details including the regex.
/// </remarks>
internal static class Actions
{
    /// <summary>
    /// Any action that reads or writes role assignments of some entity.
    /// </summary>
    public static readonly Action ReadOrWriteRoleAssignments = new(Role.Owner);

    /// <summary>
    /// A special kind of delete action which shall require the <see cref="Role.Owner"/> role on the target entity.
    /// </summary>
    public static readonly Action PrivilegedDelete = new(Role.Owner);

    /// <summary>
    /// A special kind of write action which shall require the <see cref="Role.Owner"/> role on the target entity.
    /// </summary>
    public static readonly Action PrivilegedWrite = new(Role.Owner);

    /// <summary>
    /// Any action that modifies some entity.
    /// </summary>
    public static readonly Action GenericWrite = new(Role.Owner, Role.Contributor);

    /// <summary>
    /// Any action that reads information about some entity.
    /// </summary>
    public static readonly Action GenericRead = new(Role.Owner, Role.Contributor, Role.Reader);

    /// <summary>
    /// Any action that modifies a tournament in a way that is required during "playtime".
    /// </summary>
    public static readonly Action TournamentConduct = new(Role.Owner, Role.Contributor, Role.Reporter);

    /// <summary>
    /// Any action that adds or modifies applications in a planning realm.
    /// </summary>
    public static readonly Action ManageApplications = new(Role.Owner, Role.Contributor, Role.ApplicationOfficer);
    // TODO Rename -> ApplicationWrite

    /// <summary>
    /// Any action that queries applications in a planning realm.
    /// </summary>
    public static readonly Action ApplicationsRead = new(Role.Owner, Role.Contributor, Role.ApplicationOfficer, Role.ApplicationReader);

    internal sealed class Action(params Role[] requiredRoles)
    {
        public bool IsAllowed(IEnumerable<Role> availableRoles)
        {
            return availableRoles.Any(requiredRoles.Contains);
        }
    }
}
