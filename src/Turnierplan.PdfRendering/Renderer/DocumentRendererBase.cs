using System.Diagnostics;
using QuestPDF.Fluent;
using Turnierplan.Core.Tournament;
using Turnierplan.Localization;
using Turnierplan.PdfRendering.Configuration;
using Turnierplan.PdfRendering.Tracing;

namespace Turnierplan.PdfRendering.Renderer;

public abstract class DocumentRendererBase<T> : IDocumentRenderer
    where T : IDocumentConfiguration
{
    private bool _renderCalled;

    private protected DocumentRendererBase()
    {
    }

    public Type DocumentConfigurationType => typeof(T);

    protected Activity? CurrentActivity { get; private set; }

    public bool Render(Tournament tournament, IDocumentConfiguration configuration, ILocalization localization, Stream destination)
    {
        lock (this)
        {
            if (_renderCalled)
            {
                throw new InvalidOperationException("Each instance of document renderer can only be used once.");
            }

            _renderCalled = true;
        }

        if (configuration is not T documentConfiguration)
        {
            throw new InvalidOperationException($"Expected document configuration of type '{typeof(T).Name}', but got {configuration.GetType().FullName}");
        }

        CurrentActivity = PdfRenderingActivitySource.RenderPdfDocument(GetType());

        try
        {
            var document = Render(tournament, documentConfiguration, localization);

            document.GeneratePdf(destination);

            CurrentActivity?.SetTag("turnierplan.renderer.output_size", destination.Position);
            CurrentActivity?.Stop();
        }
        catch (Exception ex)
        {
            CurrentActivity?.AddException(ex);
            CurrentActivity?.Stop();

            return false;
        }
        finally
        {
            try
            {
                CurrentActivity?.Dispose();
            }
            catch
            {
                // Ignored
            }
        }

        return true;
    }

    protected abstract Document Render(Tournament tournament, T configuration, ILocalization localization);
}
