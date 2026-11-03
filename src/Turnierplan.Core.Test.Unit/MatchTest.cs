using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit;

public sealed class MatchTest
{
    private static readonly Team[] __testTeams;
    private static readonly Match[] __testMatches;

    static MatchTest()
    {
        var group = new Group('A');

        __testMatches = new Match[5];
        __testTeams = [..Enumerable.Range(0, 10).Select(x => new Team($"Test {x}", outOfCompetition: false))];

        __testMatches[0] = new Match(1, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            TeamA = __testTeams[0],
            TeamB = __testTeams[1]
        };

        __testMatches[1] = new Match(2, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            IsCurrentlyPlaying = false,
            ScoreA = 2,
            ScoreB = 0,
            OutcomeType = MatchOutcomeType.Standard,
            TeamA = __testTeams[2],
            TeamB = __testTeams[3]
        };

        __testMatches[2] = new Match(3, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            IsCurrentlyPlaying = false,
            ScoreA = 1,
            ScoreB = 4,
            OutcomeType = MatchOutcomeType.Standard,
            TeamA = __testTeams[4],
            TeamB = __testTeams[5]
        };

        __testMatches[3] = new Match(4, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            IsCurrentlyPlaying = false,
            ScoreA = 3,
            ScoreB = 3,
            OutcomeType = MatchOutcomeType.Standard,
            TeamA = __testTeams[6],
            TeamB = __testTeams[7]
        };

        __testMatches[4] = new Match(5, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            IsCurrentlyPlaying = true,
            ScoreA = 2,
            ScoreB = 3,
            OutcomeType = MatchOutcomeType.Standard,
            TeamA = __testTeams[8],
            TeamB = __testTeams[9]
        };
    }

    [Fact]
    public void Match___Is_Finished___Returns_Correct_Result()
    {
        __testMatches[0].IsFinished.Should().BeFalse();
        __testMatches[1].IsFinished.Should().BeTrue();
        __testMatches[2].IsFinished.Should().BeTrue();
        __testMatches[3].IsFinished.Should().BeTrue();
        __testMatches[4].IsFinished.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    public void Match___Is_Team_Participant___Returns_Correct_Result(int teamId, bool expectedResult)
    {
        // Assert
        __testMatches[0].IsTeamParticipant(__testTeams[teamId]).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(0, 1, true)]
    [InlineData(1, 0, true)]
    [InlineData(2, 1, false)]
    [InlineData(0, 2, false)]
    [InlineData(2, 3, false)]
    public void Match___Are_Both_Teams_Participant___Returns_Correct_Result(int teamA, int teamB, bool expectedResult)
    {
        var teamIdA = __testTeams[teamA];
        var teamIdB = __testTeams[teamB];

        // Assert
        __testMatches[0].AreBothTeamsParticipant(teamIdA, teamIdB).Should().Be(expectedResult);
    }

    [Fact]
    public void Match___Get_Winning_Team___Returns_Correct_Result()
    {
        __testMatches[0].GetWinningTeam().Should().BeNull();
        __testMatches[1].GetWinningTeam().Should().Be(__testTeams[2]);
        __testMatches[2].GetWinningTeam().Should().Be(__testTeams[5]);
        __testMatches[3].GetWinningTeam().Should().BeNull();
        __testMatches[4].GetWinningTeam().Should().BeNull();
    }

    [Fact]
    public void Match___Get_Losing_Team___Returns_Correct_Result()
    {
        __testMatches[0].GetLosingTeam().Should().BeNull();
        __testMatches[1].GetLosingTeam().Should().Be(__testTeams[3]);
        __testMatches[2].GetLosingTeam().Should().Be(__testTeams[4]);
        __testMatches[3].GetLosingTeam().Should().BeNull();
        __testMatches[4].GetLosingTeam().Should().BeNull();
    }

    [Fact]
    public void Match___Get_Goals_Difference___Returns_Correct_Result()
    {
        __testMatches[0].GetScoreDifference().Should().BeNull();
        __testMatches[1].GetScoreDifference().Should().Be(2);
        __testMatches[2].GetScoreDifference().Should().Be(3);
        __testMatches[3].GetScoreDifference().Should().Be(0);
        __testMatches[4].GetScoreDifference().Should().Be(1);
    }
}
