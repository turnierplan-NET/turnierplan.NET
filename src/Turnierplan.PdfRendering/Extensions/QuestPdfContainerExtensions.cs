using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Turnierplan.ImageStorage;
using Image = Turnierplan.Core.Image.Image;

namespace Turnierplan.PdfRendering.Extensions;

internal static class QuestPdfContainerExtensions
{
    extension(IContainer container)
    {
        public void Image(Image image, IImageStorage imageStorage)
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
        /// This code is taken from <see href="https://www.questpdf.com/api-reference/skiasharp-integration.html#helper-script"/>, last viewed on 2025-09-26
        /// </remarks>
        public void SkiaSharpSvgCanvas(Action<SKCanvas, Size> drawOnCanvas)
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
}
