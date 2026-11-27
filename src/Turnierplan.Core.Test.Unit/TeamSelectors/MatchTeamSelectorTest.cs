using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class MatchTeamSelectorTest
{
    private readonly Team _teamA, _teamB;

    public MatchTeamSelectorTest()
    {
        var tournament = new Core.Tournament(string.Empty);
        _teamA = tournament.AddTeam("Team A");
        _teamB = tournament.AddTeam("Team B");
    }

    [Theory]
    [InlineData(MatchTeamSelector.Mode.Winner, 0, "Team A")]
    [InlineData(MatchTeamSelector.Mode.Loser, 0, "Team B")]
    [InlineData(MatchTeamSelector.Mode.Winner, 2, "Team B")]
    [InlineData(MatchTeamSelector.Mode.Loser, 2, "Team A")]
    public void MatchTeamSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(MatchTeamSelector.Mode selectionMode, int scoreB, string expectedTeamName)
    {
        // Arrange
        var match = new Match(1, new NullTeamSelector(), new NullTeamSelector(), null)
        {
            TeamA = _teamA,
            TeamB = _teamB,
            IsCurrentlyPlaying = false,
            ScoreA = 1,
            ScoreB = scoreB,
            OutcomeType = MatchOutcomeType.Standard
        };
        var selector = new MatchTeamSelector(match, selectionMode);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().NotBeNull();
        team.Name.Should().Be(expectedTeamName);
    }

    [Theory]
    [InlineData(MatchTeamSelector.Mode.Winner)]
    [InlineData(MatchTeamSelector.Mode.Loser)]
    public void MatchTeamSelector___GetTargetTeam_When_Target_Match_Is_Drawn___Returns_Null(MatchTeamSelector.Mode selectionMode)
    {
        // Arrange
        var match = new Match(1, new NullTeamSelector(), new NullTeamSelector(), null)
        {
            TeamA = _teamA,
            TeamB = _teamB,
            IsCurrentlyPlaying = false,
            ScoreA = 1,
            ScoreB = 1,
            OutcomeType = MatchOutcomeType.Standard
        };
        var selector = new MatchTeamSelector(match, selectionMode);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }

    [Theory]
    [InlineData(MatchTeamSelector.Mode.Winner)]
    [InlineData(MatchTeamSelector.Mode.Loser)]
    public void MatchTeamSelector___GetTargetTeam_When_Target_Match_Is_Not_Finished___Returns_Null(MatchTeamSelector.Mode selectionMode)
    {
        // Arrange
        var match = new Match(1, new NullTeamSelector(), new NullTeamSelector(), null)
        {
            TeamA = _teamA,
            TeamB = _teamB
        };
        var selector = new MatchTeamSelector(match, selectionMode);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
