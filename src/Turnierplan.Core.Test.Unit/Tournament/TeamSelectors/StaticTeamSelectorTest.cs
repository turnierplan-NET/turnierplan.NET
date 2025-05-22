using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament.TeamSelectors;

public sealed class StaticTeamSelectorTest : TeamSelectorTestBase
{
    public static readonly TheoryData<int, int?> TeamSelectionTestData = new()
    {
        { -7, null },
        { 0, null },
        { 1, 1 },
        { 7, 7 }
    };

    [Theory]
    [MemberData(nameof(TeamSelectionTestData))]
    public void StaticTeamSelector___Get_Target_Team_Id___Returns_Expected_Result(int targetTeamId, int? expectedTeamId)
    {
        // Arrange
        var selector = new StaticTeamSelector(targetTeamId);

        // Act
        var team = selector.GetTargetTeam(TestTournament);

        // Assert
        team?.Id.Should().Be(expectedTeamId);
    }
}
