using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class MatchSelectorTest : TeamSelectorTestBase
{
    [Theory]
    [InlineData(1, MatchSelector.Mode.Winner, "Team 4")]
    [InlineData(1, MatchSelector.Mode.Loser, "Team 5")]
    [InlineData(2, MatchSelector.Mode.Winner, "Team 7")]
    [InlineData(2, MatchSelector.Mode.Loser, "Team 6")]
    public void MatchSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(int targetMatchIndex, MatchSelector.Mode selectionMode, string expectedTeamName)
    {
        // Arrange
        var match = Tournament._matches.Single(x => x.Index == targetMatchIndex);
        var selector = new MatchSelector(match, selectionMode);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().NotBeNull();
        team.Name.Should().Be(expectedTeamName);
    }

    [Theory]
    [InlineData(3, MatchSelector.Mode.Winner)]
    [InlineData(3, MatchSelector.Mode.Loser)]
    [InlineData(4, MatchSelector.Mode.Winner)]
    [InlineData(4, MatchSelector.Mode.Loser)]
    public void MatchSelector___GetTargetTeam_When_Target_Is_Invalid___Returns_Null(int targetMatchIndex, MatchSelector.Mode selectionMode)
    {
        // Arrange
        var match = Tournament._matches.Single(x => x.Index == targetMatchIndex);
        var selector = new MatchSelector(match, selectionMode);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
