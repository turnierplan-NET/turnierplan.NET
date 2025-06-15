using Turnierplan.Core.Organization;
using Turnierplan.Core.Tournament;
using Turnierplan.PdfRendering.Configuration;
using Turnierplan.PdfRendering.Renderer;

namespace Turnierplan.PdfRendering.Test.Unit.Renderer;

public sealed class ReceiptsRendererTest(ITestOutputHelper testOutputHelper) : RendererTestBase<ReceiptsRenderer>(testOutputHelper)
{
    public static readonly TheoryData<string[], bool, (string Name, int Count)[]> CombineTeamsTestData = new()
    {
        {
            ["Test"],
            true,
            [("Test", 1)]
        },
        {
            ["Test 1"],
            true,
            [("Test 1", 1)]
        },
        {
            ["Test 1", "Test 2"],
            true,
            [("Test", 2)]
        },
        {
            ["Test I", "Test II", "Test III", "Test IV"],
            true,
            [("Test", 4)]
        },
        {
            ["Test 415", "Test 489", "Test 123", "Test 4568"],
            true,
            [("Test", 3), ("Test 4568", 1)]
        },
        {
            ["Test 1", "Test 2", "Test I", "Test IV"],
            true,
            [("Test", 4)]
        },
        {
            ["Test 1", "Test I", "Test 2I"],
            true,
            [("Test", 2), ("Test 2I", 1)]
        },
        {
            ["123"],
            true,
            [("123", 1)]
        },
        {
            [" 123"],
            true,
            [("123", 1)]
        },
        {
            ["123 123"],
            true,
            [("123 123", 1)]
        },
        {
            ["Test 1", "Test 2", "Test I", "Test IV"],
            false,
            [("Test 1", 1), ("Test 2", 1), ("Test I", 1), ("Test IV", 1)]
        }
    };

    [Fact]
    public void ReceiptsRenderer___Render_Receipts___Works_As_Expected()
    {
        for (var i = 4; i <= 16; i += 4)
        {
            var organization = new Organization("Test");
            var tournament = new Tournament(organization, "Test", Visibility.Public);

            for (var j = 0; j < i; j++)
            {
                tournament.AddTeam($"Test {j}");
            }

            AssertRender(tournament, new ReceiptsDocumentConfiguration());
        }
    }

    [Theory]
    [MemberData(nameof(CombineTeamsTestData))]
    public void ReceiptsRenderer___GenerateReceipts___Returns_Correct_Result(string[] tournamentTeamNames, bool combineSimilar, (string Name, int Count)[] expectedCombinedTeams)
    {
        var organization = new Organization("Test");
        var tournament = new Tournament(organization, "Test", Visibility.Public);

        foreach (var teamName in tournamentTeamNames)
        {
            tournament.AddTeam(teamName);
        }

        var configuration = new ReceiptsDocumentConfiguration
        {
            CombineSimilarTeams = combineSimilar
        };

        var receipts = ReceiptsRenderer.GenerateReceipts(tournament, configuration);

        receipts.Should().BeEquivalentTo(expectedCombinedTeams.Select(data => new ReceiptsRenderer.ReceiptEntry
        {
            TeamName = data.Name,
            TeamCount = data.Count,
            Amount = 10 * data.Count
        }));
    }
}
