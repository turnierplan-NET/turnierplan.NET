using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.App.Security;

internal static class Actions
{
    /// <summary>
    /// Any action that reads or writes role assignments of some entity.
    /// </summary>
    public static readonly Action ReadOrWriteRoleAssignments = new(Role.Owner);

    /// <summary>
    /// Any action that deletes some entity.
    /// </summary>
    public static readonly Action GenericDelete = new(Role.Owner);

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

    internal sealed class Action(params Role[] requiredRoles)
    {
        public bool IsAllowed(IEnumerable<Role> availableRoles)
        {
            return availableRoles.Any(requiredRoles.Contains);
        }
    }
}
