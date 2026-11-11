using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class MatchSelectorTest
{
    private readonly Team _teamA, _teamB;

    public MatchSelectorTest()
    {
        var tournament = new Tournament(string.Empty);
        _teamA = tournament.AddTeam("Team A");
        _teamB = tournament.AddTeam("Team B");
    }

    [Theory]
    [InlineData(MatchSelector.Mode.Winner, "Team A")]
    [InlineData(MatchSelector.Mode.Loser, "Team B")]
    public void MatchSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(MatchSelector.Mode selectionMode, string expectedTeamName)
    {
        // Arrange
        var match = new Match(1, new NullSelector(), new NullSelector(), null)
        {
            TeamA = _teamA,
            TeamB = _teamB,
            IsCurrentlyPlaying = false,
            ScoreA = 1,
            ScoreB = 0,
            OutcomeType = MatchOutcomeType.Standard
        };
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
