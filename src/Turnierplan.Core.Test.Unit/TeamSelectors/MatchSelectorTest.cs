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
    [InlineData(MatchSelector.Mode.Winner, 0, "Team A")]
    [InlineData(MatchSelector.Mode.Loser, 0, "Team B")]
    [InlineData(MatchSelector.Mode.Winner, 2, "Team B")]
    [InlineData(MatchSelector.Mode.Loser, 2, "Team A")]
    public void MatchSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(MatchSelector.Mode selectionMode, int scoreB, string expectedTeamName)
    {
        // Arrange
        var match = new Match(1, new NullSelector(), new NullSelector(), null)
        {
            TeamA = _teamA,
            TeamB = _teamB,
            IsCurrentlyPlaying = false,
            ScoreA = 1,
            ScoreB = scoreB,
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
    [InlineData(MatchSelector.Mode.Winner)]
    [InlineData(MatchSelector.Mode.Loser)]
    public void MatchSelector___GetTargetTeam_When_Target_Match_Is_Drawn___Returns_Null(MatchSelector.Mode selectionMode)
    {
        // Arrange
        var match = new Match(1, new NullSelector(), new NullSelector(), null)
        {
            TeamA = _teamA,
            TeamB = _teamB,
            IsCurrentlyPlaying = false,
            ScoreA = 1,
            ScoreB = 1,
            OutcomeType = MatchOutcomeType.Standard
        };
        var selector = new MatchSelector(match, selectionMode);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }

    [Theory]
    [InlineData(MatchSelector.Mode.Winner)]
    [InlineData(MatchSelector.Mode.Loser)]
    public void MatchSelector___GetTargetTeam_When_Target_Match_Is_Not_Finished___Returns_Null(MatchSelector.Mode selectionMode)
    {
        // Arrange
        var match = new Match(1, new NullSelector(), new NullSelector(), null)
        {
            TeamA = _teamA,
            TeamB = _teamB
        };
        var selector = new MatchSelector(match, selectionMode);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
