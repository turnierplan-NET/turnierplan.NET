using Turnierplan.PdfRendering.Configuration;

namespace Turnierplan.PdfRendering.Test.Unit.Configuration;

public sealed class ReceiptsDocumentConfigurationTest
{
    [Theory]
    [InlineData(1, 10, 1, 10)]
    [InlineData(1, 10, 1, 10, 2, 8)]
    [InlineData(2, 8, 1, 10, 2, 8)]
    [InlineData(1, 10, 1, 10, 3, 8)]
    [InlineData(2, 10, 1, 10, 3, 8)]
    [InlineData(3, 8, 1, 10, 3, 8)]
    [InlineData(1, 10, 1, 10, 3, 8, 5, 6)]
    [InlineData(2, 10, 1, 10, 3, 8, 5, 6)]
    [InlineData(3, 8, 1, 10, 3, 8, 5, 6)]
    [InlineData(4, 8, 1, 10, 3, 8, 5, 6)]
    [InlineData(5, 6, 1, 10, 3, 8, 5, 6)]
    [InlineData(6, 6, 1, 10, 3, 8, 5, 6)]
    [InlineData(7, 6, 1, 10, 3, 8, 5, 6)]
    public void ReceiptsDocumentConfiguration___GetAmountEntryForTeamCount___Works_As_Expected(int teamCount, double expectedAmount, params int[] configuration)
    {
        var receiptsConfiguration = new ReceiptsDocumentConfiguration();

        for (var i = 0; i < configuration.Length; i += 2)
        {
            receiptsConfiguration.Amounts[configuration[i]] = new ReceiptsDocumentConfiguration.AmountEntry
            {
                Amount = configuration[i + 1]
            };
        }

        var result = receiptsConfiguration.GetAmountEntryForTeamCount(teamCount);
        result.Amount.Should().Be(expectedAmount);
    }
}
