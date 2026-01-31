using Turnierplan.App.Extensions;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Entity;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Security;

internal interface IAccessValidator
{
    bool IsActionAllowed<T>(IEntityWithRoleAssignments<T> target, Actions.Action action)
        where T : Entity<long>, IEntityWithRoleAssignments<T>;

    void AddRolesToResponseHeader<T>(IEntityWithRoleAssignments<T> target)
        where T : Entity<long>, IEntityWithRoleAssignments<T>;
}

internal sealed class AccessValidator : IAccessValidator
{
    private const string RolesHeaderName = "X-Turnierplan-Roles";
    private readonly HttpContext _httpContext;

    /// <summary>
    /// Keep track of the IDs of all entities whose role assignments have been added to the HTTP response via the
    /// <see cref="AddRolesToResponseHeader"/> method. This is done to avoid duplicates which could previously occur
    /// in some specific endpoints. Since <see cref="AccessValidator"/> is a <see cref="ServiceLifetime.Scoped"/>
    /// service, a single list is sufficient to keep track of IDs added to the <em>current</em> response.
    /// </summary>
    private readonly HashSet<PublicId> _roleHeaderProcessedEntityIds = [];

    public AccessValidator(IHttpContextAccessor contextAccessor)
    {
        _httpContext = contextAccessor.HttpContext ?? throw new InvalidOperationException("Cannot access HttpContext");
    }

    public bool IsActionAllowed<T>(IEntityWithRoleAssignments<T> target, Actions.Action action)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        if (_httpContext.IsCurrentUserAdministrator())
        {
            return true;
        }

        var principal = _httpContext.GetActivePrincipal();

        return IsActionAllowed(target, action, principal);
    }

    public void AddRolesToResponseHeader<T>(IEntityWithRoleAssignments<T> target)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        if (!_roleHeaderProcessedEntityIds.Add(target.PublicId))
        {
            // The AddRolesToResponseHeader() method was previously called on the same entity
            return;
        }

        var availableRoles = new HashSet<Role>();

        if (_httpContext.IsCurrentUserAdministrator())
        {
            availableRoles.Add(Role.Owner);
        }
        else
        {
            var principal = _httpContext.GetActivePrincipal();
            AddAvailableRoles(target, availableRoles, principal);
        }

        if (availableRoles.Count == 0)
        {
            return;
        }

        var targetPublicId = target.PublicId.ToString();
        var rolesList = string.Join('+', availableRoles.Select(x => x.ToString()));
        var rolesHeaderValue = $"{targetPublicId}={rolesList}";

        _httpContext.Response.Headers.Append(RolesHeaderName, rolesHeaderValue);

        if (target is IEntityWithOrganization entityWithOrganization)
        {
            // TODO: Evaluate if this is still necessary after #309 / #324
            // Always add the organization-level roles, too. This is necessary because for example, the tournament
            // page allows the user to upload images. But for uploading images, the authorization check is done
            // against the organization.
            AddRolesToResponseHeader(entityWithOrganization.Organization);
        }
    }

    internal static bool IsActionAllowed<T>(IEntityWithRoleAssignments<T> target, Actions.Action action, Principal principal)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        var activePrincipalRoles = target.RoleAssignments
            .Where(x => x.Principal.Equals(principal))
            .Select(x => x.Role);

        var isAccessAllowed = action.IsAllowed(activePrincipalRoles);

        if (isAccessAllowed)
        {
            return true;
        }

        return target switch
        {
            ApiKey apiKey => IsActionAllowed(apiKey.Organization, action, principal),
            Image image => IsActionAllowed(image.Organization, action, principal),
            Folder folder => IsActionAllowed(folder.Organization, action, principal),
            PlanningRealm planningRealm => IsActionAllowed(planningRealm.Organization, action, principal),
            Tournament tournament => (tournament.Folder is not null && IsActionAllowed(tournament.Folder, action, principal)) || IsActionAllowed(tournament.Organization, action, principal),
            Venue venue => IsActionAllowed(venue.Organization, action, principal),
            _ => false
        };
    }

    internal static void AddAvailableRoles<T>(IEntityWithRoleAssignments<T> target, HashSet<Role> rolesList, Principal principal)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        rolesList.UnionWith(target.RoleAssignments.Where(x => x.Principal.Equals(principal)).Select(x => x.Role));

        switch (target)
        {
            case ApiKey apiKey:
                AddAvailableRoles(apiKey.Organization, rolesList, principal);
                break;
            case Image image:
                AddAvailableRoles(image.Organization, rolesList, principal);
                break;
            case Folder folder:
                AddAvailableRoles(folder.Organization, rolesList, principal);
                break;
            case PlanningRealm planningRealm:
                AddAvailableRoles(planningRealm.Organization, rolesList, principal);
                break;
            case Tournament tournament:
                AddAvailableRoles(tournament.Organization, rolesList, principal);
                if (tournament.Folder is not null)
                {
                    AddAvailableRoles(tournament.Folder, rolesList, principal);
                }
                break;
            case Venue venue:
                AddAvailableRoles(venue.Organization, rolesList, principal);
                break;
        }
    }
}
