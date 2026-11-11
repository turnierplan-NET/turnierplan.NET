using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class MatchSelectorTest : TeamSelectorTestBase
{
    public static readonly TheoryData<int, MatchSelector.Mode, string?> TeamSelectionTestData = new()
    {
        { 1, MatchSelector.Mode.Winner, "Team 4" },
        { 1, MatchSelector.Mode.Loser, "Team 5" },
        { 2, MatchSelector.Mode.Winner, "Team 7" },
        { 2, MatchSelector.Mode.Loser, "Team 6" },
        { 3, MatchSelector.Mode.Winner, null },
        { 3, MatchSelector.Mode.Loser, null },
        { 4, MatchSelector.Mode.Winner, null },
        { 4, MatchSelector.Mode.Loser, null }
    };

    [Theory]
    [MemberData(nameof(TeamSelectionTestData))]
    public void MatchSelector___GetTargetTeam___Returns_Expected_Result(int targetMatchIndex, MatchSelector.Mode selectionMode, string? expectedTeamName)
    {
        // Arrange
        var match = Tournament._matches.Single(x => x.Index == targetMatchIndex);
        var selector = new MatchSelector(match, selectionMode);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team?.Name.Should().Be(expectedTeamName);
    }
}
