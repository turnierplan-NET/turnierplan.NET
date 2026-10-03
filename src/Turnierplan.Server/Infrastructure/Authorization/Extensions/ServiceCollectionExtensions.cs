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
    }

    private static void AddPolicies(this AuthorizationBuilder builder)
    {
        var defaultPolicy = new AuthorizationPolicyBuilder()
            .RequireDefaults()
            .Build();

        builder.AddDefaultPolicy(PolicyNames.Default, defaultPolicy);
        builder.SetFallbackPolicy(defaultPolicy);

        builder.AddPolicy(PolicyNames.CreateOrganization, policy =>
        {
            policy.RequireDefaults();
            policy.AddRequirements(new AllowCreateOrganizationRequirement());
        });
    }

    private static AuthorizationPolicyBuilder RequireDefaults(this AuthorizationPolicyBuilder builder)
    {
        builder.AuthenticationSchemes = [AuthenticationSchemes.Session]; // TODO: Allow API key for certain policies

        builder.RequireAuthenticatedUser();
        builder.RequireClaim(ClaimTypes.PrincipalKind);
        builder.RequireClaim(ClaimTypes.PrincipalId);

        return builder;
    }
}
