using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament.TeamSelectors;

public sealed class GroupDefinitionSelectorTest : TeamSelectorTestBase
{
    public static readonly TheoryData<int, int, int?> TeamSelectionTestData = new()
    {
        { 90, 1, null },
        { 100, -1, null },
        { 100, 0, 4 },
        { 100, 1, 5 },
        { 100, 2, 6 },
        { 100, 3, 7 },
        { 100, 4, null }
    };

    [Theory]
    [MemberData(nameof(TeamSelectionTestData))]
    public void GroupDefinitionSelector___Get_Target_Team_Id___Returns_Expected_Result(int targetGroupId, int targetTeamIndex, int? expectedTeamId)
    {
        // Arrange
        var selector = new GroupDefinitionSelector(targetGroupId, targetTeamIndex);

        // Act
        var team = selector.GetTargetTeam(TestTournament);

        // Assert
        team?.Id.Should().Be(expectedTeamId);
    }
}
