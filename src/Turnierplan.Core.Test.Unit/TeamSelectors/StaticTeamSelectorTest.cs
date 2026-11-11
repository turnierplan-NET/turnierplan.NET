using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class StaticTeamSelectorTest : TeamSelectorTestBase
{
    [Fact]
    public void StaticTeamSelector___GetTargetTeam___Returns_Expected_Result()
    {
        // Arrange
        var selector = new StaticTeamSelector(Teams[1]);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeSameAs(Teams[1]);
    }
}
