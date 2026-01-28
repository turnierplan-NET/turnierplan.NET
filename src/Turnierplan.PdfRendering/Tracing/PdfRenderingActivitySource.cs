using System.Diagnostics;
using Turnierplan.Core.Image;

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

        activity.SetTag("turnierplan.renderer.type", rendererType.Name);

        activity.Start();

        return activity;
    }

    public static Activity? LoadRemoteImage(Activity? parent, Image image, string usage)
    {
        if (parent is null)
        {
            return null;
        }

        var activity = __source.StartActivity(ActivityKind.Internal, parent.Context);

        if (activity == null)
        {
            return null;
        }

        activity.SetTag("turnierplan.image.id", image.PublicId.ToString());
        activity.SetTag("turnierplan.image.type", image.Type.ToString());
        activity.SetTag("turnierplan.image_usage", usage);

        activity.Start();

        return activity;
    }
}
