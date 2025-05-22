using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament;

public sealed class MatchTest
{
    private static readonly Team[] __testTeams;
    private static readonly Match[] __testMatches;

    static MatchTest()
    {
        var group = new Group(1, 'A');

        __testMatches = new Match[5];
        __testTeams = Enumerable.Range(1, 10).Select(x => new Team(x, "Test")).ToArray();

        __testMatches[0] = new Match(1, 1, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            TeamA = __testTeams[0],
            TeamB = __testTeams[1]
        };

        __testMatches[1] = new Match(2, 2, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            IsCurrentlyPlaying = false,
            ScoreA = 2,
            ScoreB = 0,
            OutcomeType = MatchOutcomeType.Standard,
            TeamA = __testTeams[2],
            TeamB = __testTeams[3]
        };

        __testMatches[2] = new Match(3, 3, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            IsCurrentlyPlaying = false,
            ScoreA = 1,
            ScoreB = 4,
            OutcomeType = MatchOutcomeType.Standard,
            TeamA = __testTeams[4],
            TeamB = __testTeams[5]
        };

        __testMatches[3] = new Match(4, 4, new NullSelector(), new NullSelector(), group)
        {
            Court = 0,
            IsCurrentlyPlaying = false,
            ScoreA = 3,
            ScoreB = 3,
            OutcomeType = MatchOutcomeType.Standard,
            TeamA = __testTeams[6],
            TeamB = __testTeams[7]
        };

        __testMatches[4] = new Match(5, 5, new NullSelector(), new NullSelector(), group)
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
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, false)]
    public void Match___Is_Team_Participant___Returns_Correct_Result(int teamId, bool expectedResult)
    {
        // Assert
        __testMatches[0].IsTeamParticipant(__testTeams.Single(x => x.Id == teamId)).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(1, 2, true)]
    [InlineData(2, 1, true)]
    [InlineData(3, 2, false)]
    [InlineData(1, 3, false)]
    [InlineData(3, 4, false)]
    public void Match___Are_Both_Teams_Participant___Returns_Correct_Result(int teamA, int teamB, bool expectedResult)
    {
        var teamIdA = __testTeams.Single(x => x.Id == teamA);
        var teamIdB = __testTeams.Single(x => x.Id == teamB);

        // Assert
        __testMatches[0].AreBothTeamsParticipant(teamIdA, teamIdB).Should().Be(expectedResult);
    }

    [Fact]
    public void Match___Get_Winning_Team___Returns_Correct_Result()
    {
        __testMatches[0].GetWinningTeam().Should().BeNull();
        __testMatches[1].GetWinningTeam()?.Id.Should().Be(3);
        __testMatches[2].GetWinningTeam()?.Id.Should().Be(6);
        __testMatches[3].GetWinningTeam().Should().BeNull();
        __testMatches[4].GetWinningTeam().Should().BeNull();
    }

    [Fact]
    public void Match___Get_Losing_Team___Returns_Correct_Result()
    {
        __testMatches[0].GetLosingTeam().Should().BeNull();
        __testMatches[1].GetLosingTeam()?.Id.Should().Be(4);
        __testMatches[2].GetLosingTeam()?.Id.Should().Be(5);
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
