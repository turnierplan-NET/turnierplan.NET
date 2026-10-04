using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Turnierplan.Server.Domain.Users;
using Turnierplan.Server.Infrastructure.Authentication.Handlers;

namespace Turnierplan.Server.Infrastructure.Authentication.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddTurnierplanAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(AuthenticationSchemes.Session)
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationSchemes.ApiKey, _ => { })
            .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(AuthenticationSchemes.Session, _ => { });

        // TODO: Add once ApiKey is available
        // services.AddScoped<IPasswordHasher<ApiKey>, PasswordHasher<ApiKey>>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddHttpContextAccessor();
        services.AddScoped<PrincipalProvider>();
    }
}
