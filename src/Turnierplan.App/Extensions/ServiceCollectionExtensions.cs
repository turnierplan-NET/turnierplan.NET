using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.User;
using Turnierplan.Dal.Extensions;
using Turnierplan.PdfRendering.Extensions;
using IdentityOptions = Turnierplan.App.Options.IdentityOptions;

namespace Turnierplan.App.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddTurnierplanMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationInsightsConnectionString = configuration.GetSection("ApplicationInsights").GetValue<string>("ConnectionString");
        var hasApplicationInsights = !string.IsNullOrEmpty(applicationInsightsConnectionString);
        var hasOltpExporter = !string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (!hasApplicationInsights && !hasOltpExporter)
        {
            return;
        }

        var openTelemetryBuilder = services.AddOpenTelemetry();

        openTelemetryBuilder.WithMetrics(metrics =>
        {
            metrics.AddAspNetCoreInstrumentation();
            metrics.AddHttpClientInstrumentation();
            metrics.AddRuntimeInstrumentation();
            metrics.AddNpgsqlInstrumentation();
        });

        openTelemetryBuilder.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation(options =>
            {
                // Exclude health check requests from tracing
                options.Filter = context => !context.Request.Path.StartsWithSegments("/health");
            });
            tracing.AddHttpClientInstrumentation();
            tracing.AddTurnierplanDataAccessLayer();
            tracing.AddTurnierplanDocumentRendering();
        });

        if (hasApplicationInsights)
        {
            openTelemetryBuilder.UseAzureMonitor(opt => opt.ConnectionString = applicationInsightsConnectionString);
        }

        if (hasOltpExporter)
        {
            openTelemetryBuilder.UseOtlpExporter();
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
