using Npgsql;
using OpenTelemetry.Trace;

namespace Turnierplan.Dal.Extensions;

public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddTurnierplanDataAccessLayer(this TracerProviderBuilder builder)
    {
        return builder.AddNpgsql();
    }
}

