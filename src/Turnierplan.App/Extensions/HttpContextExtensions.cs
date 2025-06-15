using Turnierplan.App.Security;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.App.Extensions;

internal static class HttpContextExtensions
{
    public static Guid GetCurrentUserIdOrThrow(this HttpContext context)
    {
        var claimValue = context.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.UserId))?.Value;

        if (string.IsNullOrWhiteSpace(claimValue) || !Guid.TryParse(claimValue, out var userId))
        {
            throw new InvalidOperationException("Could not get current user ID from HttpContext.");
        }

        return userId;
    }

    public static bool IsCurrentUserAdministrator(this HttpContext context)
    {
        var claimValue = context.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Administrator))?.Value;

        return !string.IsNullOrWhiteSpace(claimValue) && claimValue.Equals("true");
    }

    public static Principal GetActivePrincipal(this HttpContext context)
    {
        foreach (var claim in context.User.Claims)
        {
            if (claim.Type.Equals(ClaimTypes.ApiKeyId))
            {
                return new Principal(PrincipalKind.ApiKey, claim.Value);
            }

            if (claim.Type.Equals(ClaimTypes.UserId))
            {
                return new Principal(PrincipalKind.User, claim.Value);
            }
        }

        throw new InvalidOperationException("Could not determine active principal.");
    }

}
