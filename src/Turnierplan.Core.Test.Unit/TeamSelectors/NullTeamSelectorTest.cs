using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class NullTeamSelectorTest
{
    [Fact]
    public void NullTeamSelector___GetTargetTeam___Returns_Null()
    {
        // Arrange
        var selector = new NullTeamSelector();

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
