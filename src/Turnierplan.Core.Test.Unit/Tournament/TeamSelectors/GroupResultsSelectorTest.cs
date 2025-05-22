using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament.TeamSelectors;

public sealed class GroupResultsSelectorTest : TeamSelectorTestBase
{
    public static readonly TheoryData<int, int, int?> TeamSelectionTestData = new()
    {
        { 90, 1, null },
        { 100, -1, null },
        { 100, 0, null },
        { 100, 1, 7 },
        { 100, 2, 5 },
        { 100, 3, 6 },
        { 100, 4, 4 },
        { 100, 5, null },
        { 101, 1, null }
    };

    [Theory]
    [MemberData(nameof(TeamSelectionTestData))]
    public void GroupResultsSelector___Get_Target_Team_Id___Returns_Expected_Result(int targetGroupId, int targetGroupPosition, int? expectedTeamId)
    {
        // Arrange
        var selector = new GroupResultsSelector(targetGroupId, targetGroupPosition);

        // Act
        var team = selector.GetTargetTeam(TestTournament);

        // Assert
        team?.Id.Should().Be(expectedTeamId);
    }
}
