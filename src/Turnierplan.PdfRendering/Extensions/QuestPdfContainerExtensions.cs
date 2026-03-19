using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Turnierplan.ImageStorage;
using Image = Turnierplan.Core.Image.Image;

namespace Turnierplan.PdfRendering.Extensions;

internal static class QuestPdfContainerExtensions
{
    /// <summary>
    /// Displays an image at the current position with the specified size. This method overwrites properties of the specified
    /// <paramref name="container"/> so you should always call this method with a fresh container. Most importantly, the
    /// <see cref="ElementExtensions.Unconstrained"/> method is called to allow for the desired positioning.
    /// </summary>
    public static void LogoImage(this IContainer container, Image image, IImageStorage imageStorage, float size, Unit sizeUnit, bool alignRight = false)
    {
        if (alignRight)
        {
            float translateX;

            if (image.Height > image.Width)
            {
                var imageDisplayWidth = (float)image.Width / image.Height * size;
                translateX = -imageDisplayWidth;
            }
            else
            {
                translateX = -size;
            }

            container = container.AlignRight().TranslateX(translateX, sizeUnit);
        }

        float translateY;

        if (image.Width > image.Height)
        {
            var imageDisplayHeight = (float)image.Height / image.Width * size;
            translateY = 0.5f * (size - imageDisplayHeight);
        }
        else
        {
            translateY = 0.0f;
        }

        container.TranslateY(translateY, sizeUnit).Unconstrained().Width(size, sizeUnit).Height(size, sizeUnit).Image(image, imageStorage);
    }

    /// <summary>
    /// Pulls the specified image from the configured image storage and creates a QuestPDF image with the retrieved image data.
    /// </summary>
    public static void Image(this IContainer container, Image image, IImageStorage imageStorage)
    {
        // Waiting for the task to complete is not ideal. However, attempting to use async
        // inside the QuestPDF document structure is probably a much bigger nightmare...
        var task = imageStorage.GetImageAsync(image);
        task.Wait();

        var stream = task.Result;
        container.Image(stream);

        // Dispose the stream (at this point, QuestPDF has read the stream content into an internal buffer)
        stream.Dispose();
    }

    /// <remarks>
    /// This code is taken from <see href="https://www.questpdf.com/api-reference/skiasharp-integration.html#helper-script"/>, last viewed on 2026-03-01
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
