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
        where T : Entity<long>, IEntityWithRoleAssignments<T>;
}

internal sealed class AccessValidator : IAccessValidator
{
    private readonly HttpContext _httpContext;

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

        Principal? activePrincipal = null;

        foreach (var claim in _httpContext.User.Claims)
        {
            if (claim.Type.Equals(ClaimTypes.ApiKeyId))
            {
                activePrincipal = new Principal(PrincipalKind.ApiKey, claim.Value);
            }
            else if (claim.Type.Equals(ClaimTypes.UserId))
            {
                activePrincipal = new Principal(PrincipalKind.User, claim.Value);
            }
        }

        if (activePrincipal is null)
        {
            throw new InvalidOperationException("Could not determine active principal.");
        }

        return IsActionAllowed(target, action, activePrincipal);
    }

    private static bool IsActionAllowed<T>(IEntityWithRoleAssignments<T> target, Actions.Action action, Principal activePrincipal)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        var activePrincipalRoles = target.RoleAssignments
            .Where(x => x.Principal.Equals(activePrincipal))
            .Select(x => x.Role);

        var isAccessAllowed = action.IsAllowed(activePrincipalRoles);

        if (isAccessAllowed)
        {
            return true;
        }

        return target switch
        {
            ApiKey apiKey => IsActionAllowed(apiKey.Organization, action, activePrincipal),
            Image image => IsActionAllowed(image.Organization, action, activePrincipal),
            Folder folder => IsActionAllowed(folder.Organization, action, activePrincipal),
            Tournament tournament => (tournament.Folder is not null && IsActionAllowed(tournament.Folder, action, activePrincipal)) || IsActionAllowed(tournament.Organization, action, activePrincipal),
            Venue venue => IsActionAllowed(venue.Organization, action, activePrincipal),
            _ => false
        };
    }
}
