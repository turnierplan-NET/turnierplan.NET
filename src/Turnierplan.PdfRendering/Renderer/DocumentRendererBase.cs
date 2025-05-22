using Microsoft.ApplicationInsights;
using QuestPDF.Fluent;
using Turnierplan.Core.Tournament;
using Turnierplan.Localization;
using Turnierplan.PdfRendering.Configuration;

namespace Turnierplan.PdfRendering.Renderer;

public abstract class DocumentRendererBase<T> : IDocumentRenderer
    where T : IDocumentConfiguration
{
    private const string DependencyTypeName = "PdfRendering";

    private readonly TelemetryClient _telemetryClient;

    private protected DocumentRendererBase(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }

    public Type DocumentConfigurationType => typeof(T);

    public void Render(Tournament tournament, IDocumentConfiguration configuration, ILocalization localization, Stream destination)
    {
        if (configuration is not T documentConfiguration)
        {
            throw new InvalidOperationException($"Expected document configuration of type '{typeof(T).Name}', but got {configuration.GetType().FullName}");
        }

        var startTime = DateTimeOffset.UtcNow;

        try
        {
            var document = Render(tournament, documentConfiguration, localization);

            document.GeneratePdf(destination);

            _telemetryClient.TrackDependency(DependencyTypeName, GetType().Name, $"PDF size (bytes): {destination.Position}", startTime, DateTimeOffset.UtcNow - startTime, true);
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex);
            _telemetryClient.TrackDependency(DependencyTypeName, GetType().Name, string.Empty, startTime, DateTimeOffset.UtcNow - startTime, false);

            throw;
        }
    }

    protected abstract Document Render(Tournament tournament, T configuration, ILocalization localization);
}
