using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class GroupDefinitionSelectorTest : TeamSelectorTestBase
{
    [Theory]
    [InlineData(0, "Team 0")]
    [InlineData(1, "Team 1")]
    [InlineData(2, "Team 2")]
    [InlineData(3, "Team 3")]
    public void GroupDefinitionSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(int targetTeamIndex, string expectedTeamName)
    {
        // Arrange
        var selector = new GroupDefinitionSelector(GroupA, targetTeamIndex);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().NotBeNull();
        team.Name.Should().Be(expectedTeamName);
    }

    [Theory]
    [InlineData('A', -1)]
    [InlineData('A', 4)]
    [InlineData('B', 1)]
    public void GroupDefinitionSelector___GetTargetTeam_When_Target_Is_Invalid___Returns_Null(char targetGroupId, int targetTeamIndex)
    {
        // Arrange
        var group = Tournament._groups.Single(x => x.AlphabeticalId == targetGroupId);
        var selector = new GroupDefinitionSelector(group, targetTeamIndex);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
