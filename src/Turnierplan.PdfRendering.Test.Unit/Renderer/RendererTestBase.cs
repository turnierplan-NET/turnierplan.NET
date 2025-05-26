using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using Turnierplan.Core.Tournament;
using Turnierplan.ImageStorage;
using Turnierplan.Localization;
using Turnierplan.Localization.Extensions;
using Turnierplan.PdfRendering.Configuration;
using Turnierplan.PdfRendering.Extensions;
using Turnierplan.PdfRendering.Renderer;
using Image = Turnierplan.Core.Image.Image;

namespace Turnierplan.PdfRendering.Test.Unit.Renderer;

public abstract partial class RendererTestBase<TRenderer>
    where TRenderer : IDocumentRenderer
{
    // ReSharper disable StaticMemberInGenericType
    private static readonly string[] __languageCodes = ["de"];
    private static readonly IServiceProvider __serviceProvider;
    // ReSharper restore StaticMemberInGenericType

    static RendererTestBase()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(new TelemetryClient(new TelemetryConfiguration()));
        serviceCollection.AddTurnierplanLocalization();
        serviceCollection.AddTurnierplanDocumentRendering("https://turnierplan-renderer-test");
        serviceCollection.AddSingleton<IImageStorage, TestImageStorage>();

        __serviceProvider = serviceCollection.BuildServiceProvider();
    }

    private readonly ITestOutputHelper _testOutputHelper;

    protected RendererTestBase(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    protected TRenderer GetRenderer()
    {
        return (TRenderer)__serviceProvider.GetRequiredService<IEnumerable<IDocumentRenderer>>().Single(x => x.GetType() == typeof(TRenderer));
    }

    protected void AssertRender(Tournament tournament, IDocumentConfiguration configuration)
    {
        foreach(var languageCode in __languageCodes)
        {
            __serviceProvider.GetRequiredService<ILocalizationProvider>().TryGetLocalization(languageCode, out var localization).Should().BeTrue();

            using var stream = new MemoryStream();
            GetRenderer().Render(tournament, configuration, new LocalizationWrapper(localization!), stream);

            var pdfData = stream.ToArray();

#if DEBUG
            SaveGeneratedPdf(configuration.GetType().Name, languageCode, pdfData);
#endif

            pdfData.Should().HaveCountGreaterThan(7);

            Encoding.ASCII.GetString(pdfData, 0, 8).Should().MatchRegex(PdfHeaderRegex());
        }
    }

    private void SaveGeneratedPdf(string name, string languageCode, byte[] pdfData)
    {
        const string outputDirectory = "pdf-output";
        Directory.CreateDirectory(outputDirectory);

        var outputFileName = $"{outputDirectory}/{name}_{languageCode}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss.fff}_{Guid.NewGuid().ToString()[..4]}.pdf";
        using var output = new FileStream(outputFileName, FileMode.Create);
        output.Write(pdfData);

        _testOutputHelper.WriteLine("PDF data written to: {0}", Path.GetFullPath(outputFileName));
    }

    [GeneratedRegex(@"^%PDF-\d\.\d")]
    private static partial Regex PdfHeaderRegex();

    private sealed class LocalizationWrapper(ILocalization inner) : LocalizationBase
    {
        public override string Get(string key) => GetSafe(key, () => inner.Get(key));

        public override string Get(string key, object arg0) => GetSafe(key, () => inner.Get(key, arg0));

        public override string Get(string key, object arg0, object arg1) => GetSafe(key, () => inner.Get(key, arg0, arg1));

        public override string Get(string key, object arg0, object arg1, object arg2) => GetSafe(key, () => inner.Get(key, arg0, arg1, arg2));

        public override string Get(string key, params object[] args) => GetSafe(key, () => inner.Get(key, args));

        private static string GetSafe(string key, Func<string> get)
        {
            var result = get();
            result.Should().NotBe(key, "that indicates a missing entry in the translation file.");
            return result;
        }
    }

    private sealed class TestImageStorage : IImageStorage
    {
        public string GetFullImageUrl(Image image) => throw new NotSupportedException();

        public Task<bool> SaveImageAsync(Image image, MemoryStream imageData) => throw new NotSupportedException();

        public Task<Stream> GetImageAsync(Image image) => throw new NotSupportedException();

        public Task<bool> DeleteImageAsync(Image image) => throw new NotSupportedException();
    }
}
