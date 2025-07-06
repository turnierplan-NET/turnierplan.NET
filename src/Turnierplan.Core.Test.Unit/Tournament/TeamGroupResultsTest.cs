using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Unit.Tournament;

public sealed class TeamGroupStatisticsTest
{
    public static readonly TheoryData<(int GoalsFor, int GoalsAgainst)[], TeamGroupStatistics> AddMatchOutcomeTestData = new()
    {
        {
            [
                (GoalsFor: 2, GoalsAgainst: 3),
                (GoalsFor: 1, GoalsAgainst: 1),
                (GoalsFor: 4, GoalsAgainst: 0)
            ],
            new TeamGroupStatistics
            {
                ScoreFor = 2 + 1 + 4,
                ScoreAgainst = 3 + 1 + 0,
                MatchesWon = 1,
                MatchesDrawn = 1,
                MatchesLost = 1,
                Points = 4
            }
        },
        {
            [
                (GoalsFor: 1, GoalsAgainst: 0),
                (GoalsFor: 2, GoalsAgainst: 2)
            ],
            new TeamGroupStatistics
            {
                ScoreFor = 1 + 2,
                ScoreAgainst = 0 + 2,
                MatchesWon = 1,
                MatchesDrawn = 1,
                MatchesLost = 0,
                Points = 4
            }
        },
        {
            [
                (GoalsFor: 1, GoalsAgainst: 1),
                (GoalsFor: 2, GoalsAgainst: 0),
                (GoalsFor: 1, GoalsAgainst: 3),
                (GoalsFor: 2, GoalsAgainst: 2)
            ],
            new TeamGroupStatistics
            {
                ScoreFor = 1 + 2 + 1 + 2,
                ScoreAgainst = 1 + 0 + 3 + 2,
                MatchesWon = 1,
                MatchesDrawn = 2,
                MatchesLost = 1,
                Points = 5
            }
        }
    };

    public static readonly TheoryData<(int GoalsFor, int GoalsAgainst)[], TeamGroupStatistics> AddMatchOutcomeWhenHigherScoreLosesTestData = new()
    {
        {
            [
                (GoalsFor: 2, GoalsAgainst: 3),
                (GoalsFor: 1, GoalsAgainst: 1),
                (GoalsFor: 4, GoalsAgainst: 0)
            ],
            new TeamGroupStatistics
            {
                ScoreFor = 2 + 1 + 4,
                ScoreAgainst = 3 + 1 + 0,
                MatchesWon = 1,
                MatchesDrawn = 1,
                MatchesLost = 1,
                Points = 4
            }
        },
        {
            [
                (GoalsFor: 1, GoalsAgainst: 0),
                (GoalsFor: 2, GoalsAgainst: 2)
            ],
            new TeamGroupStatistics
            {
                ScoreFor = 1 + 2,
                ScoreAgainst = 0 + 2,
                MatchesWon = 0,
                MatchesDrawn = 1,
                MatchesLost = 1,
                Points = 1
            }
        },
        {
            [
                (GoalsFor: 1, GoalsAgainst: 1),
                (GoalsFor: 2, GoalsAgainst: 0),
                (GoalsFor: 1, GoalsAgainst: 3),
                (GoalsFor: 2, GoalsAgainst: 2)
            ],
            new TeamGroupStatistics
            {
                ScoreFor = 1 + 2 + 1 + 2,
                ScoreAgainst = 1 + 0 + 3 + 2,
                MatchesWon = 1,
                MatchesDrawn = 2,
                MatchesLost = 1,
                Points = 5
            }
        }
    };

    [Theory]
    [InlineData(7, 3, 4)]
    [InlineData(3, 7, -4)]
    [InlineData(5, 5, 0)]
    public void TeamGroupResults___Goal_Difference___Returns_Correct_Result(int goalsFor, int goalsAgainst, int expectedGoalDifference)
    {
        // Arrange
        var results = new TeamGroupStatistics { ScoreFor = goalsFor, ScoreAgainst = goalsAgainst };

        // Assert
        results.ScoreDifference.Should().Be(expectedGoalDifference);
    }

    [Theory]
    [InlineData(1, 2, 1, 4)]
    [InlineData(3, 0, 2, 5)]
    public void TeamGroupResults___Matches_Played___Returns_Correct_Result(int matchesWon, int matchesDrawn, int matchesLost, int expectedMatchesPlayed)
    {
        // Arrange
        var results = new TeamGroupStatistics { MatchesWon = matchesWon, MatchesDrawn = matchesDrawn, MatchesLost = matchesLost };

        // Assert
        results.MatchesPlayed.Should().Be(expectedMatchesPlayed);
    }

    [Theory]
    [MemberData(nameof(AddMatchOutcomeTestData))]
    public void TeamGroupResults___Add_Match_Outcome___Works_As_Expected((int GoalsFor, int GoalsAgainst)[] matchOutcomes, TeamGroupStatistics expectedResults)
    {
        // Arrange
        var results = new TeamGroupStatistics();
        var configuration = new ComputationConfiguration();

        // Act
        foreach (var matchOutcome in matchOutcomes)
        {
            results.AddMatchOutcome(matchOutcome.GoalsFor, matchOutcome.GoalsAgainst, configuration);
        }

        // Assert
        results.Should().BeEquivalentTo(expectedResults);
    }

    [Theory]
    [MemberData(nameof(AddMatchOutcomeTestData))]
    public void TeamGroupResults___Add_Match_Outcome_With_Custom_Point_Counts___Works_As_Expected((int GoalsFor, int GoalsAgainst)[] matchOutcomes, TeamGroupStatistics expectedResults)
    {
        // Arrange
        var results = new TeamGroupStatistics();
        var configuration = new ComputationConfiguration { MatchDrawnPoints = 2, MatchWonPoints = 5 };

        // Act
        foreach (var matchOutcome in matchOutcomes)
        {
            results.AddMatchOutcome(matchOutcome.GoalsFor, matchOutcome.GoalsAgainst, configuration);
        }

        // Assert
        results.Should().BeEquivalentTo(expectedResults with
        {
            Points = expectedResults.MatchesDrawn * 2 + expectedResults.MatchesWon * 5
        });
    }

    [Theory]
    [MemberData(nameof(AddMatchOutcomeWhenHigherScoreLosesTestData))]
    public void TeamGroupResults___Add_Match_Outcome_With_HigherScoreLoses_Enabled___Works_As_Expected((int GoalsFor, int GoalsAgainst)[] matchOutcomes, TeamGroupStatistics expectedResults)
    {
        // Arrange
        var results = new TeamGroupStatistics();
        var configuration = new ComputationConfiguration { HigherScoreLoses = true };

        // Act
        foreach (var matchOutcome in matchOutcomes)
        {
            results.AddMatchOutcome(matchOutcome.GoalsFor, matchOutcome.GoalsAgainst, configuration);
        }

        // Assert
        results.Should().BeEquivalentTo(expectedResults);
    }
}
