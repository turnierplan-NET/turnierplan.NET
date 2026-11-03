using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

#if false

public sealed class NullSelectorTest : TeamSelectorTestBase
{
    [Fact]
    public void NullSelector___Get_Target_Team_Id___Returns_Null()
    {
        // Arrange
        var selector = new NullSelector();

        // Act
        var team = selector.GetTargetTeam(TestTournament);

        // Assert
        team.Should().BeNull();
    }
}

#endif
