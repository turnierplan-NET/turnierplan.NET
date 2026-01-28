using System.Diagnostics;

namespace Turnierplan.PdfRendering.Tracing;

internal static class PdfRenderingActivitySource
{
    public const string ActivitySourceName = "TurnierplanPdfRenderer";

    private static readonly ActivitySource __source = new(ActivitySourceName);

    public static Activity? RenderPdfDocument(Type rendererType)
    {
        var activity = __source.StartActivity(ActivityKind.Internal);

        if (activity == null)
        {
            return null;
        }

        activity.SetTag("turnierplan.renderer.type", rendererType.FullName);

        activity.Start();

        return activity;
    }
}
