using System.Diagnostics;
using Turnierplan.Core.Image;

namespace Turnierplan.PdfRendering.Extensions;

internal static class ActivityExtensions
{
    public static void AddRemoteImageEvent(this Activity? activity, Image image, string usage)
    {
        activity?.AddEvent(new ActivityEvent("LoadRemoteImage", tags: new ActivityTagsCollection
        {
            { "turnierplan.image.id", image.PublicId.ToString() },
            { "turnierplan.image.type", image.Type.ToString() },
            { "turnierplan.image_usage", usage }
        }));
    }
}
