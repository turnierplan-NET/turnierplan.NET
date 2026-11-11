using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class GroupResultsSelectorTest : TeamSelectorTestBase
{
    [Theory]
    [InlineData(1, "Team 3")]
    [InlineData(2, "Team 1")]
    [InlineData(3, "Team 2")]
    [InlineData(4, "Team 0")]
    public void GroupResultsSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(int targetGroupPosition, string expectedTeamName)
    {
        // Arrange
        var selector = new GroupResultsSelector(GroupA, targetGroupPosition);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().NotBeNull();
        team.Name.Should().Be(expectedTeamName);
    }

    [Fact]
    public void GroupResultsSelector___GetTargetTeam_When_Match_Is_Currently_Playing___Returns_Null()
    {
        // Arrange
        Tournament._matches.First(x => x.Group == GroupA).ResetOutcome();
        var selector = new GroupResultsSelector(GroupA, 1);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }

    [Theory]
    [InlineData('A', -1)]
    [InlineData('A', 0)]
    [InlineData('A', 5)]
    [InlineData('B', 1)]
    public void GroupResultsSelector___GetTargetTeam_When_Target_Is_Invalid___Returns_Null(char targetGroupId, int targetGroupPosition)
    {
        // Arrange
        var group = Tournament._groups.Single(x => x.AlphabeticalId == targetGroupId);
        var selector = new GroupResultsSelector(group, targetGroupPosition);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
