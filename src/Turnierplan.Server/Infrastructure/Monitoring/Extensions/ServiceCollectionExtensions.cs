using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Turnierplan.Server.Infrastructure.Monitoring.Extensions;

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
            metrics.AddMeter("Npgsql"); // TODO: Revert to AddNpgsqlInstrumentation() once Npgsql.OpenTelemetry is updated to use a non-vulnerable version of OpenTelemetry.Api
        });

        openTelemetryBuilder.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation(options =>
            {
                // Exclude health check requests from tracing
                options.Filter = context => !context.Request.Path.StartsWithSegments("/health");
            });
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource("Npgsql"); // TODO: Revert to AddNpgsql() once Npgsql.OpenTelemetry is updated to use a non-vulnerable version of OpenTelemetry.Api
            // tracing.AddTurnierplanDocumentRendering(); TODO: Add back once PDF rendering is integrated into new project
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
}
