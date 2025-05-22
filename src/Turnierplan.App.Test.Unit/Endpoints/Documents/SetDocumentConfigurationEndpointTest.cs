using FluentValidation.TestHelper;
using Turnierplan.App.Endpoints.Documents;
using Turnierplan.PdfRendering.Configuration;

namespace Turnierplan.App.Test.Unit.Endpoints.Documents;

public sealed class SetDocumentConfigurationEndpointTest
{
    [Fact]
    public void UpdateMatchPlanDocumentConfigCommand___With_Ranking_Table_But_Hide_Outcomes___Is_Invalid()
    {
        var configuration = new MatchPlanDocumentConfiguration
        {
            OrganizerNameOverride = null,
            TournamentNameOverride = null,
            VenueOverride = null,
            DateFormat = MatchPlanDateFormat.NoDate,
            Outcomes = MatchPlanOutcomes.HideOutcomeStructures,
            IncludeQrCode = false,
            IncludeRankingTable = true
        };

        var result = SetMatchPlanDocumentConfigurationEndpoint.MatchPlanDocumentConfigurationValidator.Instance.TestValidate(configuration);

        result.ShouldHaveValidationErrorFor(x => x.IncludeRankingTable)
            .WithErrorMessage("IncludeRankingTable must be false if Outcomes is HideOutcomeStructures.")
            .Only();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void UpdateReceiptsDocumentConfigCommand___Without_Amount_Entry_For_1_Team___Is_Invalid(bool containsAny)
    {
        var configuration = new ReceiptsDocumentConfiguration
        {
            Amounts = containsAny
                ? new Dictionary<int, ReceiptsDocumentConfiguration.AmountEntry> { { 2, new ReceiptsDocumentConfiguration.AmountEntry { Amount = 12 } } }
                : [],
            Currency = "\u20AC",
            SignatureLocation = string.Empty,
            CombineSimilarTeams = false
        };

        var result = SetReceiptsDocumentConfigurationEndpoint.ReceiptsDocumentConfigurationValidator.Instance.TestValidate(configuration);

        result.ShouldHaveValidationErrorFor(x => x.Amounts)
            .WithErrorMessage("An amount entry for 1 team must always be specified.")
            .Only();
    }

    [Fact]
    public void UpdateReceiptsDocumentConfigCommand___With_Too_Many_Amount_Entries___Is_Invalid()
    {
        var configuration = new ReceiptsDocumentConfiguration
        {
            Amounts = new Dictionary<int, ReceiptsDocumentConfiguration.AmountEntry>
            {
                { 1, new ReceiptsDocumentConfiguration.AmountEntry { Amount = 12 } },
                { 2, new ReceiptsDocumentConfiguration.AmountEntry { Amount = 11 } },
                { 3, new ReceiptsDocumentConfiguration.AmountEntry { Amount = 10 } },
                { 4, new ReceiptsDocumentConfiguration.AmountEntry { Amount = 9 } },
                { 5, new ReceiptsDocumentConfiguration.AmountEntry { Amount = 8 } }
            },
            Currency = "\u20AC",
            SignatureLocation = string.Empty,
            CombineSimilarTeams = false
        };

        var result = SetReceiptsDocumentConfigurationEndpoint.ReceiptsDocumentConfigurationValidator.Instance.TestValidate(configuration);

        result.ShouldHaveValidationErrorFor(x => x.Amounts.Count)
            .WithErrorMessage("The number of amount entries must be at most 4.")
            .Only();
    }
}
