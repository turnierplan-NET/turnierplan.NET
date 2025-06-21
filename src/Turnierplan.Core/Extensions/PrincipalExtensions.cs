using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.Extensions;

// TODO: Add a new field "PrincipalId" for ApiKey & User | OR Make the Id of APikey also a Guid
//       This is especially important for ApiKeys, so that the incrementally generated id is not "leaked"
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
