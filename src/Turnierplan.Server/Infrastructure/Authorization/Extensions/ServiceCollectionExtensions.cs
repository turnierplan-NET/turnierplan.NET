using Microsoft.AspNetCore.Authorization;
using Turnierplan.Server.Infrastructure.Authentication;
using Turnierplan.Server.Infrastructure.Authorization.Handlers;
using Turnierplan.Server.Infrastructure.Authorization.Requirements;

namespace Turnierplan.Server.Infrastructure.Authorization.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddTurnierplanAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder().AddPolicies();

        services.AddScoped<IAuthorizationHandler, AllowCreateOrganizationAuthorizationHandler>();

        services.AddHttpContextAccessor();
        services.AddScoped<ResponseRolesStore>();
    }

    private static void AddPolicies(this AuthorizationBuilder builder)
    {
        builder.AddDefaultPolicy(PolicyNames.Default, policy =>
        {
            policy.RequireDefaults();
        });

        builder.AddPolicy(PolicyNames.CreateOrganization, policy =>
        {
            policy.RequireDefaults();
            policy.AddRequirements(new AllowCreateOrganizationRequirement());
        });

        builder.AddDefaultPolicy(PolicyNames.Logout, policy =>
        {
            policy.RequireDefaults(allowApiKey: false);
        });
    }

    // TODO: Should allowApiKey=true be the default?
    private static AuthorizationPolicyBuilder RequireDefaults(this AuthorizationPolicyBuilder builder, bool allowApiKey = true)
    {
        builder.AuthenticationSchemes = allowApiKey
            ? [AuthenticationSchemes.ApiKey, AuthenticationSchemes.Session]
            : [AuthenticationSchemes.Session];

        builder.RequireAuthenticatedUser();
        builder.RequireClaim(ClaimTypes.PrincipalKind);
        builder.RequireClaim(ClaimTypes.PrincipalId);

        return builder;
    }
}
