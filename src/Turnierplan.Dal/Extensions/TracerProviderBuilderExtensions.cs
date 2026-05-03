using OpenTelemetry.Trace;

namespace Turnierplan.Dal.Extensions;

public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddTurnierplanDataAccessLayer(this TracerProviderBuilder builder)
    {
        return builder.AddSource("Npgsql"); // TODO: Revert to AddNpgsql() once Npgsql.OpenTelemetry is updated to use a non-vulnerable version of OpenTelemetry.Api
    }
}

