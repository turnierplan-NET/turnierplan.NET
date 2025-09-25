using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Turnierplan.ImageStorage;
using Image = Turnierplan.Core.Image.Image;

namespace Turnierplan.PdfRendering.Extensions;

internal static class QuestPdfContainerExtensions
{
    public static ImageDescriptor Image(this IContainer container, Image image, IImageStorage imageStorage)
    {
        var task = imageStorage.GetImageAsync(image);
        task.Wait();
        var stream = task.Result;
        var descriptor = container.Image(stream);
        stream.Dispose();
        return descriptor;
    }

    /// <remarks>
    /// This code is taken from <see href="https://www.questpdf.com/api-reference/skiasharp-integration.html#helper-script"/>, last viewed on 2025-09-26
    /// </remarks>
    public static void SkiaSharpSvgCanvas(this IContainer container, Action<SKCanvas, Size> drawOnCanvas)
    {
        container.Svg(size =>
        {
            using var stream = new MemoryStream();

            using (var canvas = SKSvgCanvas.Create(new SKRect(0, 0, size.Width, size.Height), stream))
            {
                drawOnCanvas(canvas, size);
            }

            var svgData = stream.ToArray();
            return Encoding.UTF8.GetString(svgData);
        });
    }
}
