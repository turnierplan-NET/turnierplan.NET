using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class GroupDefinitionTeamSelectorTest
{
    private readonly Group _group;

    public GroupDefinitionTeamSelectorTest()
    {
        var tournament = new Core.Tournament(string.Empty);
        _group = tournament.AddGroup('A');

        for (var i = 0; i < 4; i++)
        {
            var team = tournament.AddTeam($"Team {i}");
            tournament.AddGroupParticipant(_group, team);
        }
    }

    [Theory]
    [InlineData(0, "Team 0")]
    [InlineData(1, "Team 1")]
    [InlineData(2, "Team 2")]
    [InlineData(3, "Team 3")]
    public void GroupDefinitionTeamSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(int targetTeamIndex, string expectedTeamName)
    {
        // Arrange
        var selector = new GroupDefinitionTeamSelector(_group, targetTeamIndex);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().NotBeNull();
        team.Name.Should().Be(expectedTeamName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void GroupDefinitionTeamSelector___GetTargetTeam_When_Target_Is_Out_Of_Bounds___Returns_Null(int targetTeamIndex)
    {
        // Arrange
        var selector = new GroupDefinitionTeamSelector(_group, targetTeamIndex);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
