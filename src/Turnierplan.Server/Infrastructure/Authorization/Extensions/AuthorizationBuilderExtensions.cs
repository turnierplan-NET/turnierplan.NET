using Microsoft.AspNetCore.Authorization;

namespace Turnierplan.Server.Infrastructure.Authorization.Extensions;

internal static class AuthorizationBuilderExtensions
{
    public static void AddTurnierplanPolicies(this AuthorizationBuilder builder)
    {
        builder.AddPolicy(PolicyNames.CreateOrganization, policy =>
        {
            policy.RequireAssertion(_ => true);
        });
    }
}
