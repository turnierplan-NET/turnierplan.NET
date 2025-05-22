using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.User;
using IdentityOptions = Turnierplan.App.Options.IdentityOptions;

namespace Turnierplan.App.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddTurnierplanSecurity(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.Configure<IdentityOptions>(configuration);

        services.AddScoped<IAccessValidator, AccessValidator>();

        services.AddScoped<IPasswordHasher<ApiKey>, PasswordHasher<ApiKey>>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddSingleton<ISigningKeyProvider, SigningKeyProvider>();

        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationSchemes.AuthenticationSchemeApiKey, _ => { })
            .AddScheme<IdentityOptions, JwtAuthenticationHandler>(AuthenticationSchemes.AuthenticationSchemeSession, _ => { });

        services.AddAuthorization();
    }
}
