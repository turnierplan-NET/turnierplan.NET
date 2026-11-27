using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class StaticTeamSelectorTest
{
    [Fact]
    public void StaticTeamSelector___GetTargetTeam___Returns_Expected_Result()
    {
        // Arrange
        var setupTeam = new Team("Test", false);
        var selector = new StaticTeamSelector(setupTeam);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeSameAs(setupTeam);
    }
}
