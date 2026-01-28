using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.User;
using IdentityOptions = Turnierplan.App.Options.IdentityOptions;

namespace Turnierplan.App.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddTurnierplanMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("ApplicationInsights").GetValue<string>("ConnectionString");

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddNpgsql();
                })
                .UseAzureMonitor(opt => opt.ConnectionString = connectionString);

            // TODO: REmove! (temporary so app starts)
            services.AddSingleton<TelemetryClient>(sp=>(TelemetryClient)null!);
        }
    }

    public static void AddTurnierplanSecurity(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.Configure<IdentityOptions>(configuration);

        services.AddHttpContextAccessor();
        services.AddScoped<IAccessValidator, AccessValidator>();

        services.AddScoped<IPasswordHasher<ApiKey>, PasswordHasher<ApiKey>>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddSingleton<ISigningKeyProvider, SigningKeyProvider>();

        services.AddAuthentication(AuthenticationSchemes.AuthenticationSchemeSession)
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationSchemes.AuthenticationSchemeApiKey, _ => { })
            .AddScheme<IdentityOptions, JwtAuthenticationHandler>(AuthenticationSchemes.AuthenticationSchemeSession, _ => { });

        services.AddAuthorization();
    }
}
