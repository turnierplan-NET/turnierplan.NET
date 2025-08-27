using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.Comparers;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament.Comparers;

public sealed class GroupParticipantComparerTest
{
    private static Team DummyTeam(int teamId, bool outOfCompetition = false)
    {
        return new Team(teamId, null!, "Team")
        {
            OutOfCompetition = outOfCompetition
        };
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GroupParticipantComparer___When_Comparing_Same_Instance___Returns_Expected_Result(bool outOfCompetition)
    {
        // Arrange
        var comparer = CreateTestComparer();
        var team = new GroupParticipant(null!, DummyTeam(1, outOfCompetition), 0, 0);

        // Assert
        comparer.Compare(team, team).Should().Be(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GroupParticipantComparer___When_Comparing_With_Null___Returns_Expected_Result(bool outOfCompetition)
    {
        // Arrange
        var comparer = CreateTestComparer();
        var team = new GroupParticipant(null!, DummyTeam(1, outOfCompetition), 0, 0);

        // Assert
        comparer.Compare(team, null).Should().Be(-1);
        comparer.Compare(null, team).Should().Be(1);
    }

    /// <remarks>In case of equal points, the team with the lower team id is considered 'better'.</remarks>
    [Theory]
    [InlineData(4, 3, -1)]
    [InlineData(5, 3, -1)]
    [InlineData(3, 4, 1)]
    [InlineData(3, 5, 1)]
    [InlineData(3, 3, -1)]
    public void GroupParticipantComparer___Compare_By_Points___Works_As_Expected(int pointsA, int pointsB, int expectedResult)
    {
        // Arrange
        var comparer = CreateTestComparer(TeamComparisonMode.ByPoints);

        var teamX = new GroupParticipant(null!, DummyTeam(1), 0, 0)
        {
            Statistics = new TeamGroupStatistics
            {
                Points = pointsA
            }
        };
        var teamY = new GroupParticipant(null!, DummyTeam(2), 0, 0)
        {
            Statistics = new TeamGroupStatistics
            {
                Points = pointsB
            }
        };

        // Act
        var result = comparer.Compare(teamX, teamY);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <remarks>In case of equal score difference, the team with the lower team id is considered 'better'.</remarks>
    [Theory]
    [InlineData(5, 0, 3, 0, -1)]
    [InlineData(0, 3, 0, 5, -1)]
    [InlineData(3, 0, 5, 0, 1)]
    [InlineData(0, 5, 0, 3, 1)]
    [InlineData(5, 0, 5, 0, -1)]
    [InlineData(0, 5, 0, 5, -1)]
    public void GroupParticipantComparer___Compare_By_Goal_Difference___Works_As_Expected(int scoreForA, int scoreAgainstA, int scoreForB, int scoreAgainstB, int expectedResult)
    {
        // Arrange
        var comparer = CreateTestComparer(TeamComparisonMode.ByScoreDifference);

        var teamX = new GroupParticipant(null!, DummyTeam(1), 0, 0)
        {
            Statistics = new TeamGroupStatistics
            {
                ScoreFor = scoreForA,
                ScoreAgainst = scoreAgainstA
            }
        };
        var teamY = new GroupParticipant(null!, DummyTeam(2), 0, 0)
        {
            Statistics = new TeamGroupStatistics
            {
                ScoreFor = scoreForB,
                ScoreAgainst = scoreAgainstB
            }
        };

        // Act
        var result = comparer.Compare(teamX, teamY);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <remarks>In case of equal points, the team with the lower team id is considered 'better'.</remarks>
    [Theory]
    [InlineData(5, 1, -1)]
    [InlineData(8, 1, -1)]
    [InlineData(1, 5, 1)]
    [InlineData(1, 8, 1)]
    [InlineData(4, 4, -1)]
    public void GroupParticipantComparer___Compare_By_Score___Works_As_Expected(int scoreA, int scoreB, int expectedResult)
    {
        // Arrange
        var comparer = CreateTestComparer(TeamComparisonMode.ByScore);

        var teamX = new GroupParticipant(null!, DummyTeam(1), 0, 0)
        {
            Statistics = new TeamGroupStatistics
            {
                ScoreFor = scoreA
            }
        };
        var teamY = new GroupParticipant(null!, DummyTeam(2), 0, 0)
        {
            Statistics = new TeamGroupStatistics
            {
                ScoreFor = scoreB
            }
        };

        // Act
        var result = comparer.Compare(teamX, teamY);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <remarks>
    /// In case of a draw or when direct comparison can not be determined,
    /// the team with the lower team id is considered 'better'.
    /// </remarks>
    [Theory]
    [InlineData(true, null, null, null, null, -1)]
    [InlineData(false, 1, 2, null, null, -1)]
    [InlineData(false, 1, 2, 3, 0, -1)]
    [InlineData(false, 1, 2, 0, 3, 1)]
    [InlineData(false, 2, 1, 3, 0, 1)]
    [InlineData(false, 2, 1, 0, 3, -1)]
    [InlineData(false, 1, 2, 2, 2, -1)]
    [InlineData(false, 2, 1, 2, 2, -1)]
    public void GroupParticipantComparer___Compare_By_Direct_Comparison___Works_As_Expected(bool returnNoMatches, int? matchTeamIdA, int? matchTeamIdB, int? matchScoreA, int? matchScoreB, int expectedResult)
    {
        // Arrange
        var teams = new Dictionary<int, Team> { { 1, DummyTeam(1) }, { 2, DummyTeam(2) } };
        var group = new Group(1, 'A');
        var matches = new List<Match>();

        if (!returnNoMatches)
        {
            var match = new Match(0, 0, new NullSelector(), new NullSelector(), group)
            {
                TeamA = matchTeamIdA.HasValue ? teams[matchTeamIdA.Value] : null,
                TeamB = matchTeamIdB.HasValue ? teams[matchTeamIdB.Value] : null
            };

            if (matchScoreA.HasValue && matchScoreB.HasValue)
            {
                match.SetOutcome(false, matchScoreA.Value, matchScoreB.Value, MatchOutcomeType.Standard);
            }

            matches.Add(match);
        }

        var comparer = CreateTestComparer(matches, TeamComparisonMode.ByDirectComparison);

        var teamX = new GroupParticipant(group, teams[1], 0, 0);
        var teamY = new GroupParticipant(group, teams[2], 0, 0);

        // Act
        var result = comparer.Compare(teamX, teamY);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <remarks>
    /// In case of a draw or when direct comparison can not be determined,
    /// the team with the lower team id is considered 'better'.
    /// </remarks>
    [Theory]
    [InlineData(1, 2, -1, 1, 2, 2, 1)]
    [InlineData(1, 2, 1, 1, 2, 1, 2)]
    [InlineData(1, 2, 1, 1, 2, 1, 2, 4, 2)]
    [InlineData(1, 2, 1, 1, 2, 2, 3)]
    [InlineData(1, 2, -1, 1, 2, 3, 2)]
    [InlineData(2, 1, -1, 1, 2, 2, 1)]
    [InlineData(2, 1, -1, 1, 2, 1, 2)]
    [InlineData(2, 1, -1, 1, 2, 1, 2, 4, 2)]
    [InlineData(2, 1, -1, 1, 2, 2, 3)]
    [InlineData(2, 1, -1, 1, 2, 3, 2)]
    public void GroupParticipantComparer___Compare_By_Direct_Comparison_With_Multiple_Matches___Works_As_Expected(int matchTeamIdA, int matchTeamIdB, int expectedResult, params int[] matchScore)
    {
        // Arrange
        var teams = new Dictionary<int, Team> { { 1, DummyTeam(1) }, { 2, DummyTeam(2) } };
        var group = new Group(1, 'A');
        var matches = new List<Match>();

        foreach (var i in Enumerable.Range(0, matchScore.Length / 2))
        {
            var match = new Match(i, i, new NullSelector(), new NullSelector(), group)
            {
                TeamA = teams[matchTeamIdA],
                TeamB = teams[matchTeamIdB]
            };

            match.SetOutcome(false, matchScore[i * 2], matchScore[i * 2 + 1], MatchOutcomeType.Standard);
            matches.Add(match);
        }

        var comparer = CreateTestComparer(matches, TeamComparisonMode.ByDirectComparison);

        var teamX = new GroupParticipant(group, teams[1], 0, 0);
        var teamY = new GroupParticipant(group, teams[2], 0, 0);

        // Act
        var result = comparer.Compare(teamX, teamY);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <remarks>In case of equal priority, the team with the lower order is considered 'better'.</remarks>
    [Theory]
    [InlineData(5, 0, -1)]
    [InlineData(0, 5, 1)]
    [InlineData(1, 0, -1)]
    [InlineData(0, 1, 1)]
    [InlineData(0, 0, 1)]
    [InlineData(1, 1, 1)]
    [InlineData(5, 5, 1)]
    [InlineData(-5, -5, 1)]
    public void GroupParticipantComparer___Compare_By_Priority___Works_As_Expected(int priorityA, int priorityB, int expectedResult)
    {
        // Arrange
        var comparer = CreateTestComparer();

        var teamX = new GroupParticipant(null!, DummyTeam(1), 2, priorityA);
        var teamY = new GroupParticipant(null!, DummyTeam(2), 1, priorityB);

        // Act
        var result = comparer.Compare(teamX, teamY);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <remarks>In case of equal order, the team with the lower team id is considered 'better'.</remarks>
    [Theory]
    [InlineData(5, 0, 1)]
    [InlineData(0, 5, -1)]
    [InlineData(1, 0, 1)]
    [InlineData(0, 1, -1)]
    [InlineData(0, 0, -1)]
    [InlineData(1, 1, -1)]
    [InlineData(5, 5, -1)]
    public void GroupParticipantComparer___Compare_By_Order___Works_As_Expected(int orderA, int orderB, int expectedResult)
    {
        // Arrange
        var comparer = CreateTestComparer();

        var teamX = new GroupParticipant(null!, DummyTeam(1), orderA, 0);
        var teamY = new GroupParticipant(null!, DummyTeam(2), orderB, 0);

        // Act
        var result = comparer.Compare(teamX, teamY);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GroupParticipantComparer___When_Comparing_Using_Multiple_Comparing_Modes_And_With_Out_Of_Competition___Works_As_Expected(bool team3OutOfCompetition)
    {
        // Arrange
        var comparer = CreateTestComparer(TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore);

        var teams = new GroupParticipant[]
        {
            new(null!, DummyTeam(8), 0, 0)
            {
                Statistics = new TeamGroupStatistics
                {
                    Points = 8,
                    ScoreFor = 9,
                    ScoreAgainst = 5
                    // GoalDifference = 4
                }
            },
            new(null!, DummyTeam(6), 0, 0)
            {
                Statistics = new TeamGroupStatistics
                {
                    Points = 5,
                    ScoreFor = 7,
                    ScoreAgainst = 4
                    // GoalDifference = 3
                }
            },
            new(null!, DummyTeam(3, team3OutOfCompetition), 0, 0)
            {
                Statistics = new TeamGroupStatistics
                {
                    Points = 5,
                    ScoreFor = 6,
                    ScoreAgainst = 4
                    // GoalDifference = 2
                }
            },
            new(null!, DummyTeam(1), 0, 0)
            {
                Statistics = new TeamGroupStatistics
                {
                    Points = 3,
                    ScoreFor = 2,
                    ScoreAgainst = 3
                    // ScoreDifference = -1
                }
            },
            new(null!, DummyTeam(7), 0, 0)
            {
                Statistics = new TeamGroupStatistics
                {
                    Points = 3,
                    ScoreFor = 1,
                    ScoreAgainst = 2
                    // GoalDifference = -1
                }
            }
        };

        var teamsShuffled = new[] { teams[4], teams[0], teams[1], teams[3], teams[2] };

        // Act
        var sorted = teamsShuffled.Order(comparer);
        var sortedReverse = teamsShuffled.OrderDescending(comparer);

        // Assert
#pragma warning disable CA1861 // Avoid constant arrays as arguments
        if (team3OutOfCompetition)
        {
            // Team with id 3 should be last (or first when reversing)
            sorted.Select(x => x.Team.Id).Should().BeEquivalentTo(
                new[] { 8, 6, 1, 7, 3 },
                options => options.WithStrictOrdering());
            sortedReverse.Select(x => x.Team.Id).Should().BeEquivalentTo(
                new[] { 3, 7, 1, 6, 8 },
                options => options.WithStrictOrdering());
        }
        else
        {
            // Team with id 3 is sorted "normally"
            sorted.Select(x => x.Team.Id).Should().BeEquivalentTo(
                new[] { 8, 6, 3, 1, 7 },
                options => options.WithStrictOrdering());
            sortedReverse.Select(x => x.Team.Id).Should().BeEquivalentTo(
                new[] { 7, 1, 3, 6, 8 },
                options => options.WithStrictOrdering());
        }
#pragma warning restore CA1861
    }

    private static GroupParticipantComparer CreateTestComparer(params TeamComparisonMode[] comparisonModes) => CreateTestComparer([], comparisonModes);

    private static GroupParticipantComparer CreateTestComparer(IEnumerable<Match> matches, params TeamComparisonMode[] comparisonModes)
    {
        var tournament = TestTournament.Default;

        tournament._matches.AddRange(matches);

        tournament.ComputationConfiguration.ComparisonModes.Clear();
        tournament.ComputationConfiguration.ComparisonModes.AddRange(comparisonModes);

        return new GroupParticipantComparer(tournament);
    }
}
