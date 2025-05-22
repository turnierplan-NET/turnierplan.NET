using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament.TeamSelectors;

public sealed class MatchSelectorTest : TeamSelectorTestBase
{
    public static readonly TheoryData<int, MatchSelector.Mode, int?> TeamSelectionTestData = new()
    {
        { 1, MatchSelector.Mode.Winner, 4 },
        { 1, MatchSelector.Mode.Loser, 5 },
        { 2, MatchSelector.Mode.Winner, 7 },
        { 2, MatchSelector.Mode.Loser, 6 },
        { 3, MatchSelector.Mode.Winner, null },
        { 3, MatchSelector.Mode.Loser, null },
        { 4, MatchSelector.Mode.Winner, null },
        { 4, MatchSelector.Mode.Loser, null },
        { 5, MatchSelector.Mode.Winner, null },
        { 5, MatchSelector.Mode.Loser, null }
    };

    [Theory]
    [MemberData(nameof(TeamSelectionTestData))]
    public void MatchSelector___Get_Target_Team_Id___Returns_Expected_Result(int targetMatchIndex, MatchSelector.Mode selectionMode, int? expectedTeamId)
    {
        // Arrange
        var selector = new MatchSelector(targetMatchIndex, selectionMode);

        // Act
        var team = selector.GetTargetTeam(TestTournament);

        // Assert
        team?.Id.Should().Be(expectedTeamId);
    }
}
