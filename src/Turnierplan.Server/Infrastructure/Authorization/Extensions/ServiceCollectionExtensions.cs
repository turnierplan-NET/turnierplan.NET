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
        var defaultPolicy = new AuthorizationPolicyBuilder()
            .RequireDefaults()
            .Build();

        builder.AddDefaultPolicy(PolicyNames.Default, defaultPolicy);

        builder.AddPolicy(PolicyNames.CreateOrganization, policy =>
        {
            policy.RequireDefaults();
            policy.AddRequirements(new AllowCreateOrganizationRequirement());
        });
    }

    private static AuthorizationPolicyBuilder RequireDefaults(this AuthorizationPolicyBuilder builder)
    {
        // TODO: Should api key be allowed for all policies?
        builder.AuthenticationSchemes = [AuthenticationSchemes.ApiKey, AuthenticationSchemes.Session];

        builder.RequireAuthenticatedUser();
        builder.RequireClaim(ClaimTypes.PrincipalKind);
        builder.RequireClaim(ClaimTypes.PrincipalId);

        return builder;
    }
}
