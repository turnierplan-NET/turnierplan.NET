using Turnierplan.App.Extensions;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;

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

        var activePrincipalRoles = target.RoleAssignments
            .Where(x => x.Principal.Equals(activePrincipal))
            .Select(x => x.Role);

        return action.IsAllowed(activePrincipalRoles);
    }
}
