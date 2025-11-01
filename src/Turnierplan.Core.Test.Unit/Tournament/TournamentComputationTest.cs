using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament;

public sealed class TournamentComputationTest
{
    [Theory]
    [InlineData(true, true, true, 3, 2, 4, 1, 5, 6)] // Tournament with 2 groups, 6 teams, semi-finals and all ranking matches
    [InlineData(false, false, false, 3, 1, 4, 2, 5, 6)] // Tournament with 2 groups, 6 teams, semi-finals and no ranking matches → All rankings are determined based on the team's group match statistics
    [InlineData(true, false, false, 3, 2, 1, 4, 5, 6)] // Tournament with 2 groups, 6 teams, semi-finals and no ranking matches → Only first two rankings are determined via final match
    [InlineData(false, true, false, 3, 2, 4, 1, 5, 6)] // Tournament with 2 groups, 6 teams, semi-finals and no ranking matches → Only 3rd/4th place are determined via ranking match (not a typical scenario :D)
    public void Tournament___Compute_With_Ranking_Matches___Works_As_Expected(bool playoff1st, bool playoff3rd, bool playoff5th, params int[] expectedRankingsOrder)
    {
        var tournament = TestTournament.Default;

        // Add group(s)
        var groupA = tournament.AddGroup('A');
        var groupB = tournament.AddGroup('B');

        // Add team(s)
        var team1 = tournament.AddTeam("Team 1");
        var team2 = tournament.AddTeam("Team 2");
        var team3 = tournament.AddTeam("Team 3");
        var team4 = tournament.AddTeam("Team 4");
        var team5 = tournament.AddTeam("Team 5");
        var team6 = tournament.AddTeam("Team 6");

        // Assign team(s) to group(s)
        tournament.AddGroupParticipant(groupA, team6);
        tournament.AddGroupParticipant(groupA, team4);
        tournament.AddGroupParticipant(groupA, team1);

        tournament.AddGroupParticipant(groupB, team3);
        tournament.AddGroupParticipant(groupB, team2);
        tournament.AddGroupParticipant(groupB, team5);

        // Add matches
        tournament._matches.Add(new Match(1, 1, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(2, 2, new GroupDefinitionSelector(2, 0), new GroupDefinitionSelector(2, 1), groupB) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(3, 3, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(4, 4, new GroupDefinitionSelector(2, 1), new GroupDefinitionSelector(2, 2), groupB) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(5, 5, new GroupDefinitionSelector(1, 2), new GroupDefinitionSelector(1, 0), groupA) { IsCurrentlyPlaying = false, ScoreA = 3, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(6, 6, new GroupDefinitionSelector(2, 2), new GroupDefinitionSelector(2, 0), groupB) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 4, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(7, 7, new GroupResultsSelector(1, 1), new GroupResultsSelector(2, 2), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(8, 8, new GroupResultsSelector(2, 1), new GroupResultsSelector(1, 2), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 3, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });

        if (playoff5th)
        {
            tournament._matches.Add(new Match(9, 9, new GroupResultsSelector(1, 3), new GroupResultsSelector(2, 3), null, 5) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        }

        if (playoff3rd)
        {
            tournament._matches.Add(new Match(10, 10, new MatchSelector(7, MatchSelector.Mode.Loser), new MatchSelector(8, MatchSelector.Mode.Loser), null, 3) { IsCurrentlyPlaying = false, ScoreA = 3, ScoreB = 4, OutcomeType = MatchOutcomeType.Standard });
        }

        if (playoff1st)
        {
            tournament._matches.Add(new Match(11, 11, new MatchSelector(7, MatchSelector.Mode.Winner), new MatchSelector(8, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.FinalOnly, 1) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        }

        // Compute tournament
        tournament.Compute();

        // Assert group participant results
        groupA.Participants.Single(x => x.Team == team6).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 3,
            ScoreFor = 1,
            ScoreAgainst = 4,
            MatchesWon = 0,
            MatchesDrawn = 1,
            MatchesLost = 1,
            Points = 1
        });

        groupA.Participants.Single(x => x.Team == team4).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 2,
            ScoreFor = 2,
            ScoreAgainst = 3,
            MatchesWon = 0,
            MatchesDrawn = 1,
            MatchesLost = 1,
            Points = 1
        });

        groupA.Participants.Single(x => x.Team == team1).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 1,
            ScoreFor = 5,
            ScoreAgainst = 1,
            MatchesWon = 2,
            MatchesDrawn = 0,
            MatchesLost = 0,
            Points = 6
        });

        groupB.Participants.Single(x => x.Team == team3).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 1,
            ScoreFor = 6,
            ScoreAgainst = 1,
            MatchesWon = 2,
            MatchesDrawn = 0,
            MatchesLost = 0,
            Points = 6
        });

        groupB.Participants.Single(x => x.Team == team2).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 2,
            ScoreFor = 0,
            ScoreAgainst = 2,
            MatchesWon = 0,
            MatchesDrawn = 1,
            MatchesLost = 1,
            Points = 1
        });

        groupB.Participants.Single(x => x.Team == team5).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 3,
            ScoreFor = 1,
            ScoreAgainst = 4,
            MatchesWon = 0,
            MatchesDrawn = 1,
            MatchesLost = 1,
            Points = 1
        });

        // Assert match teams
        tournament._matches.Single(x => x.Id == 1).TeamA.Should().Be(team6);
        tournament._matches.Single(x => x.Id == 1).TeamB.Should().Be(team4);

        tournament._matches.Single(x => x.Id == 2).TeamA.Should().Be(team3);
        tournament._matches.Single(x => x.Id == 2).TeamB.Should().Be(team2);

        tournament._matches.Single(x => x.Id == 3).TeamA.Should().Be(team4);
        tournament._matches.Single(x => x.Id == 3).TeamB.Should().Be(team1);

        tournament._matches.Single(x => x.Id == 4).TeamA.Should().Be(team2);
        tournament._matches.Single(x => x.Id == 4).TeamB.Should().Be(team5);

        tournament._matches.Single(x => x.Id == 5).TeamA.Should().Be(team1);
        tournament._matches.Single(x => x.Id == 5).TeamB.Should().Be(team6);

        tournament._matches.Single(x => x.Id == 6).TeamA.Should().Be(team5);
        tournament._matches.Single(x => x.Id == 6).TeamB.Should().Be(team3);

        tournament._matches.Single(x => x.Id == 7).TeamA.Should().Be(team1);
        tournament._matches.Single(x => x.Id == 7).TeamB.Should().Be(team2);

        tournament._matches.Single(x => x.Id == 8).TeamA.Should().Be(team3);
        tournament._matches.Single(x => x.Id == 8).TeamB.Should().Be(team4);

        if (playoff5th)
        {
            tournament._matches.Single(x => x.Id == 9).TeamA.Should().Be(team6);
            tournament._matches.Single(x => x.Id == 9).TeamB.Should().Be(team5);
        }

        if (playoff3rd)
        {
            tournament._matches.Single(x => x.Id == 10).TeamA.Should().Be(team1);
            tournament._matches.Single(x => x.Id == 10).TeamB.Should().Be(team4);
        }

        if (playoff1st)
        {
            tournament._matches.Single(x => x.Id == 11).TeamA.Should().Be(team2);
            tournament._matches.Single(x => x.Id == 11).TeamB.Should().Be(team3);
        }

        // Assert ranking
        for (var i = 0; i < expectedRankingsOrder.Length; i++)
        {
            var ranking = i + 1;
            var teamName = $"Team {expectedRankingsOrder[i]}";

            tournament._teams.Single(x => x.Name.Equals(teamName)).Ranking.Should().Be(ranking);
        }
    }

    [Fact]
    public void Tournament___Compute_With_Multiple_Group_Phase_Rounds___Works_As_Expected()
    {
        var tournament = TestTournament.Default;

        // Add group(s)
        var groupA = tournament.AddGroup('A');

        // Add team(s)
        var team1 = tournament.AddTeam("Team 1");
        var team2 = tournament.AddTeam("Team 2");
        var team3 = tournament.AddTeam("Team 3");

        // Assign team(s) to group(s)
        tournament.AddGroupParticipant(groupA, team1);
        tournament.AddGroupParticipant(groupA, team2);
        tournament.AddGroupParticipant(groupA, team3);

        // Add matches
        tournament._matches.Add(new Match(1, 1, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(2, 2, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 3, ScoreB = 4, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(3, 3, new GroupDefinitionSelector(1, 2), new GroupDefinitionSelector(1, 0), groupA) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(4, 4, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 0), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(5, 5, new GroupDefinitionSelector(1, 2), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(6, 6, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 3, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });

        // Compute tournament
        tournament.Compute();

        // Assert group participant results
        groupA.Participants.Single(x => x.Team == team1).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 1,
            ScoreFor = 7,
            ScoreAgainst = 3,
            MatchesWon = 2,
            MatchesDrawn = 1,
            MatchesLost = 1,
            Points = 7
        });

        groupA.Participants.Single(x => x.Team == team2).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 2,
            ScoreFor = 8,
            ScoreAgainst = 8,
            MatchesWon = 1,
            MatchesDrawn = 2,
            MatchesLost = 1,
            Points = 5
        });

        groupA.Participants.Single(x => x.Team == team3).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 3,
            ScoreFor = 6,
            ScoreAgainst = 10,
            MatchesWon = 1,
            MatchesDrawn = 1,
            MatchesLost = 2,
            Points = 4
        });

        // Assert match teams
        tournament._matches.Single(x => x.Id == 1).TeamA.Should().Be(team1);
        tournament._matches.Single(x => x.Id == 1).TeamB.Should().Be(team2);

        tournament._matches.Single(x => x.Id == 2).TeamA.Should().Be(team2);
        tournament._matches.Single(x => x.Id == 2).TeamB.Should().Be(team3);

        tournament._matches.Single(x => x.Id == 3).TeamA.Should().Be(team3);
        tournament._matches.Single(x => x.Id == 3).TeamB.Should().Be(team1);

        tournament._matches.Single(x => x.Id == 4).TeamA.Should().Be(team2);
        tournament._matches.Single(x => x.Id == 4).TeamB.Should().Be(team1);

        tournament._matches.Single(x => x.Id == 5).TeamA.Should().Be(team3);
        tournament._matches.Single(x => x.Id == 5).TeamB.Should().Be(team2);

        tournament._matches.Single(x => x.Id == 6).TeamA.Should().Be(team1);
        tournament._matches.Single(x => x.Id == 6).TeamB.Should().Be(team3);
    }

    [Fact]
    public void Tournament___Compute_When_Direct_Comparison_Between_Teams_Is_Required___Works_As_Expected()
    {
        var tournament = TestTournament.Default;

        // Add group(s)
        var groupA = tournament.AddGroup('A');

        // Add team(s)
        var team1 = tournament.AddTeam("Team 1");
        var team2 = tournament.AddTeam("Team 2");
        var team3 = tournament.AddTeam("Team 3");
        var team4 = tournament.AddTeam("Team 4");

        // Assign team(s) to group(s)
        tournament.AddGroupParticipant(groupA, team1);
        tournament.AddGroupParticipant(groupA, team2);
        tournament.AddGroupParticipant(groupA, team3);
        tournament.AddGroupParticipant(groupA, team4);

        // Add matches
        tournament._matches.Add(new Match(1, 1, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(2, 2, new GroupDefinitionSelector(1, 2), new GroupDefinitionSelector(1, 3), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(3, 3, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(4, 4, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 3), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(5, 5, new GroupDefinitionSelector(1, 3), new GroupDefinitionSelector(1, 0), groupA) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 3, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(6, 6, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });

        // Compute tournament
        tournament.Compute();

        // Assert group participant results
        groupA.Participants.Single(x => x.Team == team1).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 1,
            ScoreFor = 6,
            ScoreAgainst = 5,
            MatchesWon = 2,
            MatchesDrawn = 0,
            MatchesLost = 1,
            Points = 6
        });

        groupA.Participants.Single(x => x.Team == team2).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 4,
            ScoreFor = 4,
            ScoreAgainst = 5,
            MatchesWon = 1,
            MatchesDrawn = 0,
            MatchesLost = 2,
            Points = 3
        });

        groupA.Participants.Single(x => x.Team == team3).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 3,
            ScoreFor = 4,
            ScoreAgainst = 5,
            MatchesWon = 1,
            MatchesDrawn = 0,
            MatchesLost = 2,
            Points = 3
        });

        groupA.Participants.Single(x => x.Team == team4).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 2,
            ScoreFor = 6,
            ScoreAgainst = 5,
            MatchesWon = 2,
            MatchesDrawn = 0,
            MatchesLost = 1,
            Points = 6
        });

        // Assert match teams
        tournament._matches.Single(x => x.Id == 1).TeamA.Should().Be(team1);
        tournament._matches.Single(x => x.Id == 1).TeamB.Should().Be(team2);

        tournament._matches.Single(x => x.Id == 2).TeamA.Should().Be(team3);
        tournament._matches.Single(x => x.Id == 2).TeamB.Should().Be(team4);

        tournament._matches.Single(x => x.Id == 3).TeamA.Should().Be(team1);
        tournament._matches.Single(x => x.Id == 3).TeamB.Should().Be(team3);

        tournament._matches.Single(x => x.Id == 4).TeamA.Should().Be(team2);
        tournament._matches.Single(x => x.Id == 4).TeamB.Should().Be(team4);

        tournament._matches.Single(x => x.Id == 5).TeamA.Should().Be(team4);
        tournament._matches.Single(x => x.Id == 5).TeamB.Should().Be(team1);

        tournament._matches.Single(x => x.Id == 6).TeamA.Should().Be(team2);
        tournament._matches.Single(x => x.Id == 6).TeamB.Should().Be(team3);
    }

    [Fact]
    public void Tournament___Compute_When_Comparison_Between_Teams_Using_Priority_Is_Required___Works_As_Expected()
    {
        var tournament = TestTournament.Default;

        // Add group(s)
        var groupA = tournament.AddGroup('A');

        // Add team(s)
        var team1 = tournament.AddTeam("Team 1");
        var team2 = tournament.AddTeam("Team 2");
        var team3 = tournament.AddTeam("Team 3");

        // Assign team(s) to group(s)
        tournament.AddGroupParticipant(groupA, team1, 5);
        tournament.AddGroupParticipant(groupA, team2, 1);
        tournament.AddGroupParticipant(groupA, team3, 2);

        // Add matches
        tournament._matches.Add(new Match(1, 1, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(2, 2, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(3, 3, new GroupDefinitionSelector(1, 2), new GroupDefinitionSelector(1, 0), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });

        // Compute tournament
        tournament.Compute();

        // Assert group participant results
        groupA.Participants.Single(x => x.Team == team1).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 1,
            ScoreFor = 2,
            ScoreAgainst = 2,
            MatchesWon = 0,
            MatchesDrawn = 2,
            MatchesLost = 0,
            Points = 2
        });

        groupA.Participants.Single(x => x.Team == team2).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 3,
            ScoreFor = 2,
            ScoreAgainst = 2,
            MatchesWon = 0,
            MatchesDrawn = 2,
            MatchesLost = 0,
            Points = 2
        });

        groupA.Participants.Single(x => x.Team == team3).Statistics.Should().BeEquivalentTo(new TeamGroupStatistics
        {
            Position = 2,
            ScoreFor = 2,
            ScoreAgainst = 2,
            MatchesWon = 0,
            MatchesDrawn = 2,
            MatchesLost = 0,
            Points = 2
        });

        // Assert match teams
        tournament._matches.Single(x => x.Id == 1).TeamA.Should().Be(team1);
        tournament._matches.Single(x => x.Id == 1).TeamB.Should().Be(team2);

        tournament._matches.Single(x => x.Id == 2).TeamA.Should().Be(team2);
        tournament._matches.Single(x => x.Id == 2).TeamB.Should().Be(team3);

        tournament._matches.Single(x => x.Id == 3).TeamA.Should().Be(team3);
        tournament._matches.Single(x => x.Id == 3).TeamB.Should().Be(team1);
    }

    /// <summary>
    /// This method tests a special case that requires that any teams which are qualified for any finals match are
    /// always ranked above any other team which has not qualified for any finals match even if the latter team has
    /// better group statistics than the qualified team.
    /// </summary>
    /// <remarks>
    /// Note that this test relies on the ordering of teams by their ID in case of equal statistics.
    /// </remarks>
    [Fact]
    public void Tournament___Compute_Qualified_Teams_Are_Always_Ranked_Better_Than_Non_Qualified_Teams_Case_1___Works_As_Expected()
    {
         var tournament = TestTournament.Default;

        // Add group(s)
        var groupA = tournament.AddGroup('A');
        var groupB = tournament.AddGroup('B');

        // Add team(s)
        var team1 = tournament.AddTeam("Team 1");
        var team2 = tournament.AddTeam("Team 2");
        var team3 = tournament.AddTeam("Team 3");
        var team4 = tournament.AddTeam("Team 4");
        var team5 = tournament.AddTeam("Team 5");
        var team6 = tournament.AddTeam("Team 6");
        var team7 = tournament.AddTeam("Team 7");
        var team8 = tournament.AddTeam("Team 8");

        // Assign team(s) to group(s)
        tournament.AddGroupParticipant(groupA, team1);
        tournament.AddGroupParticipant(groupA, team2);
        tournament.AddGroupParticipant(groupA, team3);
        tournament.AddGroupParticipant(groupA, team4);

        tournament.AddGroupParticipant(groupB, team5);
        tournament.AddGroupParticipant(groupB, team6);
        tournament.AddGroupParticipant(groupB, team7);
        tournament.AddGroupParticipant(groupB, team8);

        // Add matches
        tournament._matches.Add(new Match(1, 1, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(2, 2, new GroupDefinitionSelector(1, 2), new GroupDefinitionSelector(1, 3), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(3, 3, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(4, 4, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 3), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(5, 5, new GroupDefinitionSelector(1, 3), new GroupDefinitionSelector(1, 0), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(6, 6, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(7, 7, new GroupDefinitionSelector(2, 0), new GroupDefinitionSelector(2, 1), groupB) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(8, 8, new GroupDefinitionSelector(2, 2), new GroupDefinitionSelector(2, 3), groupB) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(9, 9, new GroupDefinitionSelector(2, 0), new GroupDefinitionSelector(2, 2), groupB) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(10, 10, new GroupDefinitionSelector(2, 1), new GroupDefinitionSelector(2, 3), groupB) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(11, 11, new GroupDefinitionSelector(2, 3), new GroupDefinitionSelector(2, 0), groupB) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(12, 12, new GroupDefinitionSelector(2, 1), new GroupDefinitionSelector(2, 2), groupB) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(13, 13, new GroupResultsSelector(1, 1), new GroupResultsSelector(2, 2), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(14, 14, new GroupResultsSelector(2, 1), new GroupResultsSelector(1, 2), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(15, 15, new MatchSelector(13, MatchSelector.Mode.Winner), new MatchSelector(14, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.FinalOnly, 1) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });

        // Compute tournament
        tournament.Compute();

        // Assert ranking
        tournament._teams.Single(x => x.Name.Equals("Team 2")).Ranking.Should().Be(1);
        tournament._teams.Single(x => x.Name.Equals("Team 1")).Ranking.Should().Be(2);
        tournament._teams.Single(x => x.Name.Equals("Team 5")).Ranking.Should().Be(3);
        tournament._teams.Single(x => x.Name.Equals("Team 6")).Ranking.Should().Be(4);
        tournament._teams.Single(x => x.Name.Equals("Team 3")).Ranking.Should().Be(5);
        tournament._teams.Single(x => x.Name.Equals("Team 4")).Ranking.Should().Be(6);
        tournament._teams.Single(x => x.Name.Equals("Team 7")).Ranking.Should().Be(7);
        tournament._teams.Single(x => x.Name.Equals("Team 8")).Ranking.Should().Be(8);
    }

    /// <summary>
    /// This method tests a special case that requires that any teams which are qualified for a "better" finals match
    /// (in the sense that playing a semi-final is considered "better" than playing "only" a quarter-final) are always
    /// ranked above any other team which has not qualified for any finals or only qualified for "worse" finals
    /// matches even if the latter team has better group statistics than the former team.
    /// </summary>
    /// <remarks>
    /// Note that this test relies on the ordering of teams by their ID in case of equal statistics.
    /// </remarks>
    [Fact]
    public void Tournament___Compute_Qualified_Teams_Are_Always_Ranked_Better_Than_Non_Qualified_Teams_Case_2___Works_As_Expected()
    {
         var tournament = TestTournament.Default;

        // Add group(s)
        var groupA = tournament.AddGroup('A');
        var groupB = tournament.AddGroup('B');
        var groupC = tournament.AddGroup('C');

        // Add team(s)
        var team1 = tournament.AddTeam("Team 1");
        var team2 = tournament.AddTeam("Team 2");
        var team3 = tournament.AddTeam("Team 3");
        var team4 = tournament.AddTeam("Team 4");
        var team5 = tournament.AddTeam("Team 5");
        var team6 = tournament.AddTeam("Team 6");
        var team7 = tournament.AddTeam("Team 7");
        var team8 = tournament.AddTeam("Team 8");
        var team9 = tournament.AddTeam("Team 9");
        var team10 = tournament.AddTeam("Team 10");
        var team11 = tournament.AddTeam("Team 11");
        var team12 = tournament.AddTeam("Team 12");

        // Assign team(s) to group(s)
        tournament.AddGroupParticipant(groupA, team1);
        tournament.AddGroupParticipant(groupA, team2);
        tournament.AddGroupParticipant(groupA, team3);
        tournament.AddGroupParticipant(groupA, team4);

        tournament.AddGroupParticipant(groupB, team5);
        tournament.AddGroupParticipant(groupB, team6);
        tournament.AddGroupParticipant(groupB, team7);
        tournament.AddGroupParticipant(groupB, team8);

        tournament.AddGroupParticipant(groupC, team9);
        tournament.AddGroupParticipant(groupC, team10);
        tournament.AddGroupParticipant(groupC, team11);
        tournament.AddGroupParticipant(groupC, team12);

        // Add matches
        tournament._matches.Add(new Match(1, 1, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(2, 2, new GroupDefinitionSelector(1, 2), new GroupDefinitionSelector(1, 3), groupA) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(3, 3, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(4, 4, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 3), groupA) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(5, 5, new GroupDefinitionSelector(1, 3), new GroupDefinitionSelector(1, 0), groupA) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(6, 6, new GroupDefinitionSelector(1, 1), new GroupDefinitionSelector(1, 2), groupA) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(7, 7, new GroupDefinitionSelector(2, 0), new GroupDefinitionSelector(2, 1), groupB) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(8, 8, new GroupDefinitionSelector(2, 2), new GroupDefinitionSelector(2, 3), groupB) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(9, 9, new GroupDefinitionSelector(2, 0), new GroupDefinitionSelector(2, 2), groupB) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(10, 10, new GroupDefinitionSelector(2, 1), new GroupDefinitionSelector(2, 3), groupB) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(11, 11, new GroupDefinitionSelector(2, 3), new GroupDefinitionSelector(2, 0), groupB) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(12, 12, new GroupDefinitionSelector(2, 1), new GroupDefinitionSelector(2, 2), groupB) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(13, 13, new GroupDefinitionSelector(3, 0), new GroupDefinitionSelector(3, 1), groupC) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(14, 14, new GroupDefinitionSelector(3, 2), new GroupDefinitionSelector(3, 3), groupC) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(15, 15, new GroupDefinitionSelector(3, 0), new GroupDefinitionSelector(3, 2), groupC) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(16, 16, new GroupDefinitionSelector(3, 1), new GroupDefinitionSelector(3, 3), groupC) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(17, 17, new GroupDefinitionSelector(3, 3), new GroupDefinitionSelector(3, 0), groupC) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(18, 18, new GroupDefinitionSelector(3, 1), new GroupDefinitionSelector(3, 2), groupC) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(19, 19, new GroupResultsSelector(1, 1), new GroupResultsNthRankedSelector([groupA.Id, groupB.Id, groupC.Id], 0, 3), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(20, 20, new GroupResultsSelector(2, 1), new GroupResultsNthRankedSelector([groupA.Id, groupB.Id, groupC.Id], 1, 3), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 3, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(21, 21, new GroupResultsSelector(1, 2), new GroupResultsSelector(3, 2), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(22, 22, new GroupResultsSelector(2, 2), new GroupResultsSelector(3, 1), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(23, 23, new MatchSelector(19, MatchSelector.Mode.Winner), new MatchSelector(20, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(24, 24, new MatchSelector(21, MatchSelector.Mode.Winner), new MatchSelector(22, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard });

        tournament._matches.Add(new Match(25, 25, new MatchSelector(23, MatchSelector.Mode.Winner), new MatchSelector(24, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.FinalOnly, 1) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });

        // Compute tournament
        tournament.Compute();

        // Assert ranking from final match: 11 vs 6
        tournament._teams.Single(x => x.Name.Equals("Team 11")).Ranking.Should().Be(1);
        tournament._teams.Single(x => x.Name.Equals("Team 6")).Ranking.Should().Be(2);

        // Assert ranking from semi-final disqualified teams: 2, 7 (compared by group stats)
        tournament._teams.Single(x => x.Name.Equals("Team 7")).Ranking.Should().Be(3);
        tournament._teams.Single(x => x.Name.Equals("Team 2")).Ranking.Should().Be(4);

        // Assert ranking from quarter-final disqualified teams: 1, 5, 10, 9 (compared by group stats)
        tournament._teams.Single(x => x.Name.Equals("Team 9")).Ranking.Should().Be(5);
        tournament._teams.Single(x => x.Name.Equals("Team 10")).Ranking.Should().Be(6);
        tournament._teams.Single(x => x.Name.Equals("Team 5")).Ranking.Should().Be(7);
        tournament._teams.Single(x => x.Name.Equals("Team 1")).Ranking.Should().Be(8);

        // Assert ranking from non-qualified teams: 3, 4, 8, 12
        tournament._teams.Single(x => x.Name.Equals("Team 12")).Ranking.Should().Be(9);
        tournament._teams.Single(x => x.Name.Equals("Team 8")).Ranking.Should().Be(10);
        tournament._teams.Single(x => x.Name.Equals("Team 3")).Ranking.Should().Be(11);
        tournament._teams.Single(x => x.Name.Equals("Team 4")).Ranking.Should().Be(12);
    }

    // TODO: Add tests corresponding to the edge-cases handled by the ranking calculation change
}
