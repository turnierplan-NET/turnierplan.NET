using Turnierplan.App.Security;

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
}
