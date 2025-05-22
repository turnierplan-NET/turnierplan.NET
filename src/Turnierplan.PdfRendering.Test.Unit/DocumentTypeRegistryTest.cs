using Turnierplan.Core.Document;
using Turnierplan.PdfRendering.Configuration;

namespace Turnierplan.PdfRendering.Test.Unit;

public sealed class DocumentTypeRegistryTest
{
    private readonly DocumentTypeRegistry _registry = new();

    [Theory]
    [InlineData(DocumentType.MatchPlan, typeof(MatchPlanDocumentConfiguration))]
    [InlineData(DocumentType.Receipts, typeof(ReceiptsDocumentConfiguration))]
    [InlineData(DocumentType.RefereeCards, typeof(RefereeCardsDocumentConfiguration))]
    public void DocumentTypeRegistry___TryGetDocumentConfigurationType___Works_As_Expected(DocumentType type, Type expectedClrType)
    {
        _registry.TryGetDocumentConfigurationType(type, out var actualClrType).Should().BeTrue();
        actualClrType.Should().Be(expectedClrType);
    }

    [Fact]
    public void DocumentTypeRegistry___Contains_Expected_Types()
    {
        _registry.GetAvailableDocumentTypes().Should().BeEquivalentTo([
            DocumentType.MatchPlan,
            DocumentType.Receipts,
            DocumentType.RefereeCards
        ]);
    }
}
