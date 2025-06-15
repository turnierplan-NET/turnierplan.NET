using Turnierplan.Core.Organization;
using Turnierplan.Core.Tournament;
using Turnierplan.PdfRendering.Configuration;
using Turnierplan.PdfRendering.Renderer;

namespace Turnierplan.PdfRendering.Test.Unit.Renderer;

public sealed class RefereeCardsRendererTest(ITestOutputHelper testOutputHelper) : RendererTestBase<RefereeCardsRenderer>(testOutputHelper)
{
    [Fact]
    public void RefereeCardsRenderer___Render_Referee_Cards___Works_As_Expected()
    {
        for (var i = 0; i < 3; i++)
        {
            var organization = new Organization("Test");
            var tournament = new Tournament(organization, "Test", Visibility.Public);

            var groupA = tournament.AddGroup('A');
            var groupB = tournament.AddGroup('B');

            for (var j = 0; j < 12; j++)
            {
                var team = tournament.AddTeam($"Team {j}");
                tournament.AddGroupParticipant(j % 2 == 0 ? groupA : groupB, team);
            }

            tournament.GenerateMatchPlan(new MatchPlanConfiguration
            {
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = i,
                }
            });

            AssertRender(tournament, new RefereeCardsDocumentConfiguration());
        }
    }
}
