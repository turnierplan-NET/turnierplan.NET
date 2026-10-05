using System.Security.Claims;

namespace Turnierplan.Server.Infrastructure.Authentication.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static bool HasAllowCreateOrganizationClaim(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.HasClaim(ClaimTypes.AllowCreateOrganization, ClaimValues.True);
    }

    public static bool HasIsAdministratorClaim(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.HasClaim(ClaimTypes.IsAdministrator, ClaimValues.True);
    }
}
