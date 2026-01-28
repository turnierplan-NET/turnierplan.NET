using OpenTelemetry.Trace;
using Turnierplan.PdfRendering.Tracing;

namespace Turnierplan.PdfRendering.Extensions;

public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddTurnierplanPdfRenderer(this TracerProviderBuilder builder)
    {
        return builder.AddSource(PdfRenderingActivitySource.ActivitySourceName);
    }
}

