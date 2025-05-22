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
    /// This code is taken from https://www.questpdf.com/concepts/skia-sharp-integration.html
    /// </remarks>
    public static void SkiaSharpCanvas(this IContainer container, Action<SKCanvas, Size> drawOnCanvas)
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

    /// <remarks>
    /// This code is taken from https://www.questpdf.com/concepts/skia-sharp-integration.html
    /// </remarks>
    public static void SkiaSharpRasterized(this IContainer container, Action<SKCanvas, Size> drawOnCanvas)
    {
        container.Image(payload =>
        {
            using var bitmap = new SKBitmap(payload.ImageSize.Width, payload.ImageSize.Height);

            using (var canvas = new SKCanvas(bitmap))
            {
                var scalingFactor = payload.Dpi / (float)DocumentSettings.DefaultRasterDpi;
                canvas.Scale(scalingFactor);
                drawOnCanvas(canvas, payload.AvailableSpace);
            }

            return bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        });
    }
}
