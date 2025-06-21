using Turnierplan.App.Extensions;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Security;

internal interface IAccessValidator
{
    bool IsActionAllowed<T>(IEntityWithRoleAssignments<T> target, Actions.Action action)
        where T : Entity, IEntityWithRoleAssignments<T>;
}

internal sealed class AccessValidator : IAccessValidator
{
    private readonly HttpContext _httpContext;

    public AccessValidator(IHttpContextAccessor contextAccessor)
    {
        _httpContext = contextAccessor.HttpContext ?? throw new InvalidOperationException("Cannot access HttpContext");
    }

    public bool IsActionAllowed<T>(IEntityWithRoleAssignments<T> target, Actions.Action action)
        where T : Entity, IEntityWithRoleAssignments<T>
    {
        if (_httpContext.IsCurrentUserAdministrator())
        {
            return true;
        }

        var principal = _httpContext.GetActivePrincipal();

        return IsActionAllowed(target, action, principal);
    }

    internal static bool IsActionAllowed<T>(IEntityWithRoleAssignments<T> target, Actions.Action action, Principal principal)
        where T : Entity, IEntityWithRoleAssignments<T>
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
            Tournament tournament => (tournament.Folder is not null && IsActionAllowed(tournament.Folder, action, principal)) || IsActionAllowed(tournament.Organization, action, principal),
            Venue venue => IsActionAllowed(venue.Organization, action, principal),
            _ => false
        };
    }
}
