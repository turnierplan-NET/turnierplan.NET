using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.Extensions;

public static class PrincipalExtensions
{
    public static Principal AsPrincipal(this ApiKey.ApiKey apiKey)
    {
        return new Principal(PrincipalKind.ApiKey, apiKey.Id.ToString());
    }

    public static Principal AsPrincipal(this User.User user)
    {
        return new Principal(PrincipalKind.User, user.Id.ToString());
    }
}
