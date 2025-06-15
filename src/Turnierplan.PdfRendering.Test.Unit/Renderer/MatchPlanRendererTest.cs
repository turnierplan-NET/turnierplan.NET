using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.PdfRendering.Configuration;
using Turnierplan.PdfRendering.Renderer;

namespace Turnierplan.PdfRendering.Test.Unit.Renderer;

public sealed class MatchPlanRendererTest(ITestOutputHelper testOutputHelper) : RendererTestBase<MatchPlanRenderer>(testOutputHelper)
{
    private static readonly ScheduleConfig __scheduleConfig = new()
    {
        FirstMatchKickoff = new DateTime(2024, 1, 1, 12, 0, 0),
        GroupPhaseNumberOfCourts = 1,
        GroupPhasePlayTime = 10.Minutes(),
        GroupPhasePauseTime = 1.Minutes(),
        PauseBetweenGroupAndFinalsPhase = 0.Minutes(),
        FinalsPhaseNumberOfCourts = 1,
        FinalsPhasePlayTime = 15.Minutes(),
        FinalsPhasePauseTime = 2.Minutes()
    };

    /// <remarks>The test data is taken from <c>MatchPlanGeneratorTestBase</c> (only a small subset is used).</remarks>
    public static readonly TheoryData<int[], MatchPlanConfiguration> TestData = new()
    {
        {
            // Single group with 3 teams, no finals round
            [3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            }
        },
        {
            // Two group with 4 and 3 teams respectively, alternating match ordering and no finals round
            [4, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            }
        },
        {
            // Single group with 4 teams, semi-finals, third place playoff and final match
            [4],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.SemiFinals,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = true,
                    AdditionalPlayoffs = null
                }
            }
        },
        {
            // Two groups with 5 teams each, quarter-finals, semi-finals, fifth place playoff, third place playoff and final match
            // Played on a two courts in group phase and three courts in finals phase (as opposed to the test case below)
            [5, 5],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig with { GroupPhaseNumberOfCourts = 2, FinalsPhaseNumberOfCourts = 3 },
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.QuarterFinals,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = true,
                    AdditionalPlayoffs =
                    [
                        new AdditionalPlayoffConfig(5, "5.0", "5.1")
                    ]
                }
            }
        }
    };

    /// <remarks>Most of this code is taken from <c>MatchPlanGeneratorTestBase</c>.</remarks>
    [Theory]
    [MemberData(nameof(TestData))]
    public void MatchPlanRenderer___Render_Match_Plan___Works_As_Expected(int[] teamsPerGroup, MatchPlanConfiguration configuration)
    {
        var organization = new Organization("Test");
        var tournament = new Tournament(organization, "Test", Visibility.Public);

        for (var i = 0; i < teamsPerGroup.Length; i++)
        {
            var group = tournament.AddGroup((char)('A' + i));

            for (var j = 0; j < teamsPerGroup[i]; j++)
            {
                var team = tournament.AddTeam($"Team {j}");
                tournament.AddGroupParticipant(group, team);
            }
        }

        tournament.GenerateMatchPlan(configuration);

        AssertRender(tournament, new MatchPlanDocumentConfiguration());
    }

    [Fact]
    public void MatchPlanRenderer___GetPublicTournamentUrl___Returns_Correct_Result()
    {
        var tournamentId = new PublicId(12544539674150040574);
        var tournamentUrl = GetRenderer().GetPublicTournamentUrl(tournamentId);

        tournamentUrl.Should().Be("https://turnierplan-renderer-test/tournament?id=K4XKF6sgfP_");
    }
}
