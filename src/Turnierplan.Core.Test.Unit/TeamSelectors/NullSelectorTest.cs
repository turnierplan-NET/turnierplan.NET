using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class NullSelectorTest
{
    [Fact]
    public void NullSelector___GetTargetTeam___Returns_Null()
    {
        // Arrange
        var selector = new NullSelector();

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
