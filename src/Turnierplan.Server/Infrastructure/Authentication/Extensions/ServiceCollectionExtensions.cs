using Microsoft.AspNetCore.Authentication;
using Turnierplan.Server.Infrastructure.Authentication.Handlers;
using Turnierplan.Server.Infrastructure.Authentication.Options;

namespace Turnierplan.Server.Infrastructure.Authentication.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddTurnierplanAuthentication(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.Configure<IdentityOptions>(configuration);

        services.AddAuthentication(AuthenticationSchemes.Session)
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationSchemes.ApiKey, _ => { })
            .AddScheme<IdentityOptions, JwtAuthenticationHandler>(AuthenticationSchemes.Session, _ => { });

        // TODO: Add once ApiKey & User are available
        // services.AddScoped<IPasswordHasher<ApiKey>, PasswordHasher<ApiKey>>();
        // services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddHttpContextAccessor();
        services.AddScoped<PrincipalProvider>();
    }
}
