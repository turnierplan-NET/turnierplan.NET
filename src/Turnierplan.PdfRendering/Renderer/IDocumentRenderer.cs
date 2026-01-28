using Turnierplan.Core.Tournament;
using Turnierplan.Localization;
using Turnierplan.PdfRendering.Configuration;

namespace Turnierplan.PdfRendering.Renderer;

public interface IDocumentRenderer
{
    Type DocumentConfigurationType { get; }

    bool Render(Tournament tournament, IDocumentConfiguration configuration, ILocalization localization, Stream destination);
}
