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
        PrincipalKind? kind = null;
        Guid? principalId = null;

        foreach (var claim in context.User.Claims)
        {
            switch (claim.Type)
            {
                case ClaimTypes.PrincipalKind:
                    kind = Enum.Parse<PrincipalKind>(claim.Value);
                    break;
                case ClaimTypes.PrincipalId:
                    principalId = Guid.Parse(claim.Value);
                    break;
            }
        }

        if (kind.HasValue && principalId.HasValue)
        {
            return new Principal(kind.Value, principalId.Value);
        }

        throw new InvalidOperationException("Could not determine active principal.");
    }

}
