using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class GroupResultsSelectorTest
{
    private readonly Match _match;
    private readonly Group _group;

    public GroupResultsSelectorTest()
    {
        var tournament = new Tournament(string.Empty);
        _group = tournament.AddGroup('A');

        for (var i = 0; i < 4; i++)
        {
            var team = tournament.AddTeam($"Team {i}");
            tournament.AddGroupParticipant(_group, team);
        }

        _match = new Match(1, new NullSelector(), new NullSelector(), _group)
        {
            Court = 0,
            TeamA = tournament._teams[0],
            TeamB = tournament._teams[1],
            IsCurrentlyPlaying = false,
            ScoreA = 1,
            ScoreB = 0,
            OutcomeType = MatchOutcomeType.Standard
        };
        tournament._matches.Add(_match);

        _group.Participants[0].Statistics = new TeamGroupStatistics { Position = 4, Points = 0 };
        _group.Participants[1].Statistics = new TeamGroupStatistics { Position = 2, Points = 6 };
        _group.Participants[2].Statistics = new TeamGroupStatistics { Position = 3, Points = 3 };
        _group.Participants[3].Statistics = new TeamGroupStatistics { Position = 1, Points = 9 };
    }

    [Theory]
    [InlineData(1, "Team 3")]
    [InlineData(2, "Team 1")]
    [InlineData(3, "Team 2")]
    [InlineData(4, "Team 0")]
    public void GroupResultsSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(int targetGroupPosition, string expectedTeamName)
    {
        // Arrange
        var selector = new GroupResultsSelector(_group, targetGroupPosition);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().NotBeNull();
        team.Name.Should().Be(expectedTeamName);
    }

    [Fact]
    public void GroupResultsSelector___GetTargetTeam_When_Match_Is_Currently_Playing___Returns_Null()
    {
        // Arrange
        _match.ResetOutcome();
        var selector = new GroupResultsSelector(_group, 1);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    public void GroupResultsSelector___GetTargetTeam_When_Target_Is_Out_Of_Bounds___Returns_Null(int targetGroupPosition)
    {
        // Arrange
        var selector = new GroupResultsSelector(_group, targetGroupPosition);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }
}
