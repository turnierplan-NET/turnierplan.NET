using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Turnierplan.Localization;

namespace Turnierplan.PdfRendering.Extensions;

internal static class QuestPdfDocumentExtensions
{
    public static Document WithTurnierplanMetadata(this Document document, ILocalization localization)
    {
        return document.WithMetadata(new DocumentMetadata
        {
            Title = null,
            Author = null,
            Subject = null,
            Keywords = null,
            Creator = localization.Get("Documents.Metadata.Creator"),
            Producer = localization.Get("Documents.Metadata.Producer")
        });
    }
}
