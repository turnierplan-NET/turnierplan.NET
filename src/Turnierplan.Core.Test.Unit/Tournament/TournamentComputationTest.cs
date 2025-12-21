using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament;

public sealed class TournamentComputationTest
{
    [Theory]
    [InlineData(true, true, true, false, 3, 2, 4, 1, 5, 6)] // Tournament with 2 groups, 6 teams, semi-finals and all ranking matches
    [InlineData(false, false, false, false, 3, 2, 1, 4, 5, 6)] // Tournament with 2 groups, 6 teams, semi-finals and no ranking matches → First four ranking are defined by the outcomes of semi-finals, remaining rankings are determined based on the team's group match statistics
    [InlineData(true, false, false, false, 3, 2, 1, 4, 5, 6)] // Tournament with 2 groups, 6 teams, semi-finals and no ranking matches → Only first two rankings are determined via final match
    [InlineData(false, true, false, true, /* 1st & 2nd are "empty" */ 4, 1, 3, 2, 5, 6)] // Tournament with 2 groups, 6 teams, semi-finals and no ranking matches → Only 3rd/4th place are determined via ranking match (not a typical scenario :D)
    public void Tournament___Compute_With_Ranking_Matches___Works_As_Expected(bool playoff1st, bool playoff3rd, bool playoff5th, bool firstAndSecondPlaceSkipped, params int[] expectedRankingsOrder)
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

        tournament._matches.Add(new Match(7, 7, new GroupResultsSelector(1, 1), new GroupResultsSelector(2, 2), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard }); // Team 1 vs Team 2
        tournament._matches.Add(new Match(8, 8, new GroupResultsSelector(2, 1), new GroupResultsSelector(1, 2), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 3, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard }); // Team 3 vs Team 4

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
        tournament._ranking.Count.Should().Be(expectedRankingsOrder.Length);
        for (var i = 0; i < expectedRankingsOrder.Length; i++)
        {
            var teamName = $"Team {expectedRankingsOrder[i]}";

            var team = tournament._teams.Single(x => x.Name.Equals(teamName));
            var position = firstAndSecondPlaceSkipped ? i + 3 : i + 1;
            var ranking = tournament._ranking.Single(x => x.Position == position);
            ranking.IsDefined.Should().BeTrue();
            ranking.Team.Should().Be(team);
        }

        // Assert team statistics (the numbers represent: <group phase> + <semi-finals if applicable> + <playoff if applicable>)
        team1.Statistics.ScoreFor.Should().Be(5 + 1 + (playoff3rd ? 3 : 0));
        team1.Statistics.ScoreAgainst.Should().Be(1 + 2 + (playoff3rd ? 4 : 0));
        team1.Statistics.MatchesWon.Should().Be(2 + 0);
        team1.Statistics.MatchesDrawn.Should().Be(0 + 0);
        team1.Statistics.MatchesLost.Should().Be(0 + 1 + (playoff3rd ? 1 : 0));
        team1.Statistics.MatchesPlayed.Should().Be(2 + 1 + (playoff3rd ? 1 : 0));

        team2.Statistics.ScoreFor.Should().Be(0 + 2);
        team2.Statistics.ScoreAgainst.Should().Be(2 + 1 + (playoff1st ? 1 : 0));
        team2.Statistics.MatchesWon.Should().Be(0 + 1);
        team2.Statistics.MatchesDrawn.Should().Be(1 + 0);
        team2.Statistics.MatchesLost.Should().Be(1 + 0 + (playoff1st ? 1 : 0));
        team2.Statistics.MatchesPlayed.Should().Be(2 + 1 + (playoff1st ? 1 : 0));

        team3.Statistics.ScoreFor.Should().Be(6 + 3 + (playoff1st ? 1 : 0));
        team3.Statistics.ScoreAgainst.Should().Be(1 + 1);
        team3.Statistics.MatchesWon.Should().Be(2 + 1 + (playoff1st ? 1 : 0));
        team3.Statistics.MatchesDrawn.Should().Be(0 + 0);
        team3.Statistics.MatchesLost.Should().Be(0 + 0);
        team3.Statistics.MatchesPlayed.Should().Be(2 + 1 + (playoff1st ? 1 : 0));

        team4.Statistics.ScoreFor.Should().Be(2 + 1 + (playoff3rd ? 4 : 0));
        team4.Statistics.ScoreAgainst.Should().Be(3 + 3 + (playoff3rd ? 3 : 0));
        team4.Statistics.MatchesWon.Should().Be(0 + 0 + (playoff3rd ? 1 : 0));
        team4.Statistics.MatchesDrawn.Should().Be(1 + 0);
        team4.Statistics.MatchesLost.Should().Be(1 + 1);
        team4.Statistics.MatchesPlayed.Should().Be(2 + 1 + (playoff3rd ? 1 : 0));

        team5.Statistics.ScoreFor.Should().Be(1 + (playoff5th ? 2 : 0));
        team5.Statistics.ScoreAgainst.Should().Be(4 + (playoff5th ? 1 : 0));
        team5.Statistics.MatchesWon.Should().Be(0 + (playoff5th ? 1 : 0));
        team5.Statistics.MatchesDrawn.Should().Be(1);
        team5.Statistics.MatchesLost.Should().Be(1);
        team5.Statistics.MatchesPlayed.Should().Be(2 + (playoff5th ? 1 : 0));

        team6.Statistics.ScoreFor.Should().Be(1 + (playoff5th ? 1 : 0));
        team6.Statistics.ScoreAgainst.Should().Be(4 + (playoff5th ? 2 : 0));
        team6.Statistics.MatchesWon.Should().Be(0);
        team6.Statistics.MatchesDrawn.Should().Be(1);
        team6.Statistics.MatchesLost.Should().Be(1 + (playoff5th ? 1 : 0));
        team6.Statistics.MatchesPlayed.Should().Be(2 + (playoff5th ? 1 : 0));
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

        // Assert team statistics
        team1.Statistics.ScoreFor.Should().Be(6);
        team1.Statistics.ScoreAgainst.Should().Be(5);
        team1.Statistics.MatchesWon.Should().Be(2);
        team1.Statistics.MatchesDrawn.Should().Be(0);
        team1.Statistics.MatchesLost.Should().Be(1);

        team2.Statistics.ScoreFor.Should().Be(4);
        team2.Statistics.ScoreAgainst.Should().Be(5);
        team2.Statistics.MatchesWon.Should().Be(1);
        team2.Statistics.MatchesDrawn.Should().Be(0);
        team2.Statistics.MatchesLost.Should().Be(2);

        team3.Statistics.ScoreFor.Should().Be(4);
        team3.Statistics.ScoreAgainst.Should().Be(5);
        team3.Statistics.MatchesWon.Should().Be(1);
        team3.Statistics.MatchesDrawn.Should().Be(0);
        team3.Statistics.MatchesLost.Should().Be(2);

        team4.Statistics.ScoreFor.Should().Be(6);
        team4.Statistics.ScoreAgainst.Should().Be(5);
        team4.Statistics.MatchesWon.Should().Be(2);
        team4.Statistics.MatchesDrawn.Should().Be(0);
        team4.Statistics.MatchesLost.Should().Be(1);
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

        // Assert team statistics
        team1.Statistics.ScoreFor.Should().Be(2);
        team1.Statistics.ScoreAgainst.Should().Be(2);
        team1.Statistics.MatchesWon.Should().Be(0);
        team1.Statistics.MatchesDrawn.Should().Be(2);
        team1.Statistics.MatchesLost.Should().Be(0);

        team2.Statistics.ScoreFor.Should().Be(2);
        team2.Statistics.ScoreAgainst.Should().Be(2);
        team2.Statistics.MatchesWon.Should().Be(0);
        team2.Statistics.MatchesDrawn.Should().Be(2);
        team2.Statistics.MatchesLost.Should().Be(0);

        team3.Statistics.ScoreFor.Should().Be(2);
        team3.Statistics.ScoreAgainst.Should().Be(2);
        team3.Statistics.MatchesWon.Should().Be(0);
        team3.Statistics.MatchesDrawn.Should().Be(2);
        team3.Statistics.MatchesLost.Should().Be(0);
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
        tournament._ranking.Count.Should().Be(8);
        tournament._ranking.Single(x => x.Position == 1).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 2")));
        tournament._ranking.Single(x => x.Position == 1).Reason.Should().Be(RankingReason.WinnerOfFinal);
        tournament._ranking.Single(x => x.Position == 2).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 1")));
        tournament._ranking.Single(x => x.Position == 2).Reason.Should().Be(RankingReason.LoserOfFinal);
        tournament._ranking.Single(x => x.Position == 3).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 5")));
        tournament._ranking.Single(x => x.Position == 3).Reason.Should().Be(RankingReason.QualifiedForSemiFinals);
        tournament._ranking.Single(x => x.Position == 4).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 6")));
        tournament._ranking.Single(x => x.Position == 4).Reason.Should().Be(RankingReason.QualifiedForSemiFinals);
        tournament._ranking.Single(x => x.Position == 5).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 3")));
        tournament._ranking.Single(x => x.Position == 5).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
        tournament._ranking.Single(x => x.Position == 6).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 4")));
        tournament._ranking.Single(x => x.Position == 6).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
        tournament._ranking.Single(x => x.Position == 7).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 7")));
        tournament._ranking.Single(x => x.Position == 7).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
        tournament._ranking.Single(x => x.Position == 8).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 8")));
        tournament._ranking.Single(x => x.Position == 8).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
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

        tournament._matches.Add(new Match(19, 19, new GroupResultsSelector(1, 1), new GroupResultsNthRankedSelector([groupA.Id, groupB.Id, groupC.Id], 0, 3), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard }); // Team 1 vs Team 11
        tournament._matches.Add(new Match(20, 20, new GroupResultsSelector(2, 1), new GroupResultsNthRankedSelector([groupA.Id, groupB.Id, groupC.Id], 1, 3), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 3, OutcomeType = MatchOutcomeType.Standard }); // Team 5 vs Team 7
        tournament._matches.Add(new Match(21, 21, new GroupResultsSelector(1, 2), new GroupResultsSelector(3, 2), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard }); // Team 2 vs Team 10
        tournament._matches.Add(new Match(22, 22, new GroupResultsSelector(2, 2), new GroupResultsSelector(3, 1), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false, ScoreA = 1, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard }); // Team 6 vs Team 9

        tournament._matches.Add(new Match(23, 23, new MatchSelector(19, MatchSelector.Mode.Winner), new MatchSelector(20, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard }); // Team 11 vs Team 7
        tournament._matches.Add(new Match(24, 24, new MatchSelector(21, MatchSelector.Mode.Winner), new MatchSelector(22, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false, ScoreA = 0, ScoreB = 1, OutcomeType = MatchOutcomeType.Standard }); // Team 2 vs Team 6

        tournament._matches.Add(new Match(25, 25, new MatchSelector(23, MatchSelector.Mode.Winner), new MatchSelector(24, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.FinalOnly, 1) { IsCurrentlyPlaying = false, ScoreA = 2, ScoreB = 0, OutcomeType = MatchOutcomeType.Standard });

        // Compute tournament
        tournament.Compute();

        // Assert ranking from final match: 11 vs 6
        tournament._ranking.Single(x => x.Position == 1).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 11")));
        tournament._ranking.Single(x => x.Position == 1).Reason.Should().Be(RankingReason.WinnerOfFinal);
        tournament._ranking.Single(x => x.Position == 2).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 6")));
        tournament._ranking.Single(x => x.Position == 2).Reason.Should().Be(RankingReason.LoserOfFinal);

        // Assert ranking from semi-final disqualified teams: 2, 7 (compared by their respective success in the semi-final matches)
        tournament._ranking.Single(x => x.Position == 3).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 2")));
        tournament._ranking.Single(x => x.Position == 3).Reason.Should().Be(RankingReason.QualifiedForSemiFinals);
        tournament._ranking.Single(x => x.Position == 4).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 7")));
        tournament._ranking.Single(x => x.Position == 4).Reason.Should().Be(RankingReason.QualifiedForSemiFinals);

        // Assert ranking from quarter-final disqualified teams: 1, 5, 10, 9 (compared by their respective success in the quarter-final matches)
        tournament._ranking.Single(x => x.Position == 5).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 9"))); // 9 and 1 both lost 0:1 so they are compared by their group stats
        tournament._ranking.Single(x => x.Position == 5).Reason.Should().Be(RankingReason.QualifiedForQuarterFinals);
        tournament._ranking.Single(x => x.Position == 6).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 1")));
        tournament._ranking.Single(x => x.Position == 6).Reason.Should().Be(RankingReason.QualifiedForQuarterFinals);
        tournament._ranking.Single(x => x.Position == 7).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 10")));
        tournament._ranking.Single(x => x.Position == 7).Reason.Should().Be(RankingReason.QualifiedForQuarterFinals);
        tournament._ranking.Single(x => x.Position == 8).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 5")));
        tournament._ranking.Single(x => x.Position == 8).Reason.Should().Be(RankingReason.QualifiedForQuarterFinals);

        // Assert ranking from non-qualified teams: 3, 4, 8, 12
        tournament._ranking.Single(x => x.Position == 9).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 12")));
        tournament._ranking.Single(x => x.Position == 9).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
        tournament._ranking.Single(x => x.Position == 10).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 8")));
        tournament._ranking.Single(x => x.Position == 10).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
        tournament._ranking.Single(x => x.Position == 11).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 3")));
        tournament._ranking.Single(x => x.Position == 11).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
        tournament._ranking.Single(x => x.Position == 12).Team.Should().Be(tournament._teams.Single(x => x.Name.Equals("Team 4")));
        tournament._ranking.Single(x => x.Position == 12).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
    }

    [Fact]
    public void Tournament___Compute___Matches_With_Special_Scoring_Do_Not_Count_Towards_Team_Statistics_Score()
    {
        var tournament = TestTournament.Default;

        // Add group
        var groupA = tournament.AddGroup('A');

        // Add teams
        var team1 = tournament.AddTeam("Team 1");
        var team2 = tournament.AddTeam("Team 2");

        // Assign teams to groups
        tournament.AddGroupParticipant(groupA, team1);
        tournament.AddGroupParticipant(groupA, team2);

        // Add matches
        tournament._matches.Add(new Match(1, 1, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 4, ScoreB = 2, OutcomeType = MatchOutcomeType.Standard });
        tournament._matches.Add(new Match(2, 2, new GroupDefinitionSelector(1, 0), new GroupDefinitionSelector(1, 1), groupA) { IsCurrentlyPlaying = false, ScoreA = 3, ScoreB = 0, OutcomeType = MatchOutcomeType.SpecialScoring });

        // Compute tournament
        tournament.Compute();

        // Assert team statistics
        team1.Statistics.ScoreFor.Should().Be(4);
        team1.Statistics.ScoreAgainst.Should().Be(2);
        team1.Statistics.MatchesWon.Should().Be(2);
        team1.Statistics.MatchesDrawn.Should().Be(0);
        team1.Statistics.MatchesLost.Should().Be(0);
        team1.Statistics.MatchesPlayed.Should().Be(2);

        team2.Statistics.ScoreFor.Should().Be(2);
        team2.Statistics.ScoreAgainst.Should().Be(4);
        team2.Statistics.MatchesWon.Should().Be(0);
        team2.Statistics.MatchesDrawn.Should().Be(0);
        team2.Statistics.MatchesLost.Should().Be(2);
        team2.Statistics.MatchesPlayed.Should().Be(2);
    }

    /// <remarks>
    /// This test uses the same tournament structure, matches and match outcomes as:<br/>
    /// <see cref="Tournament___Compute_Qualified_Teams_Are_Always_Ranked_Better_Than_Non_Qualified_Teams_Case_2___Works_As_Expected"/><br/>
    /// Therefore, the final ranking is also equivalent!
    /// </remarks>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Tournament___Compute_Rankings_Are_Always_Evaluated_As_Soon_As_Possible___Works_As_Expected(bool with3rdPlacePlayoff)
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

        // Add matches WITHOUT setting the outcomes after match 17
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
        tournament._matches.Add(new Match(18, 18, new GroupDefinitionSelector(3, 1), new GroupDefinitionSelector(3, 2), groupC) { IsCurrentlyPlaying = false });

        tournament._matches.Add(new Match(19, 19, new GroupResultsSelector(1, 1), new GroupResultsNthRankedSelector([groupA.Id, groupB.Id, groupC.Id], 0, 3), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false }); // Team 1 vs Team 11
        tournament._matches.Add(new Match(20, 20, new GroupResultsSelector(2, 1), new GroupResultsNthRankedSelector([groupA.Id, groupB.Id, groupC.Id], 1, 3), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false }); // Team 5 vs Team 7
        tournament._matches.Add(new Match(21, 21, new GroupResultsSelector(1, 2), new GroupResultsSelector(3, 2), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false }); // Team 2 vs Team 10
        tournament._matches.Add(new Match(22, 22, new GroupResultsSelector(2, 2), new GroupResultsSelector(3, 1), (int)FinalsRoundOrder.QuarterFinals, null) { IsCurrentlyPlaying = false }); // Team 6 vs Team 9

        tournament._matches.Add(new Match(23, 23, new MatchSelector(19, MatchSelector.Mode.Winner), new MatchSelector(20, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false }); // Team 11 vs Team 7
        tournament._matches.Add(new Match(24, 24, new MatchSelector(21, MatchSelector.Mode.Winner), new MatchSelector(22, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.SemiFinals, null) { IsCurrentlyPlaying = false }); // Team 2 vs Team 6

        if (with3rdPlacePlayoff)
        {
            tournament._matches.Add(new Match(25, 25, new MatchSelector(23, MatchSelector.Mode.Loser), new MatchSelector(24, MatchSelector.Mode.Loser), null, 3) { IsCurrentlyPlaying = false }); // Team 7 vs Team 2
            tournament._matches.Add(new Match(26, 26, new MatchSelector(23, MatchSelector.Mode.Winner), new MatchSelector(24, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.FinalOnly, 1) { IsCurrentlyPlaying = false }); // Team 11 vs Team 6
        }
        else
        {
            tournament._matches.Add(new Match(25, 25, new MatchSelector(23, MatchSelector.Mode.Winner), new MatchSelector(24, MatchSelector.Mode.Winner), (int)FinalsRoundOrder.FinalOnly, 1) { IsCurrentlyPlaying = false }); // Team 11 vs Team 6
        }

        // For now, only all group matches except one have an outcome => no rankings yet
        tournament.Compute();
        AssertRankingTeams([null, null, null, null, null, null, null, null, null, null, null, null]);
        AssertRankingReasons();

        // Set final group match => The final four rankings can now be calculated
        tournament._matches.Single(x => x.Index == 18).SetOutcome(false, 2, 2, MatchOutcomeType.Standard);
        tournament.Compute();
        AssertRankingTeams([null, null, null, null, null, null, null, null, team12, team8, team3, team4]);
        AssertRankingReasons();

        // Set three of the four quarter-finals => Nothing should change
        tournament._matches.Single(x => x.Index == 19).SetOutcome(false, 0, 1, MatchOutcomeType.Standard);
        tournament._matches.Single(x => x.Index == 20).SetOutcome(false, 0, 3, MatchOutcomeType.Standard);
        tournament._matches.Single(x => x.Index == 21).SetOutcome(false, 2, 0, MatchOutcomeType.Standard);
        tournament.Compute();
        AssertRankingTeams([null, null, null, null, null, null, null, null, team12, team8, team3, team4]);
        AssertRankingReasons();

        // Set the final quarter-final => Four new rankings appear (the four losers of the quarter-finals)
        tournament._matches.Single(x => x.Index == 22).SetOutcome(false, 1, 0, MatchOutcomeType.Standard);
        tournament.Compute();
        AssertRankingTeams([null, null, null, null, team9, team1, team10, team5, team12, team8, team3, team4]);
        AssertRankingReasons();

        // Set the first semi-final => Nothing should change
        tournament._matches.Single(x => x.Index == 23).SetOutcome(false, 2, 0, MatchOutcomeType.Standard);
        tournament.Compute();
        AssertRankingTeams([null, null, null, null, team9, team1, team10, team5, team12, team8, team3, team4]);
        AssertRankingReasons();

        if (with3rdPlacePlayoff)
        {
            // Set the second semi-final => Nothing changes because the final four rankings each are set by the last two matches
            tournament._matches.Single(x => x.Index == 24).SetOutcome(false, 0, 1, MatchOutcomeType.Standard);
            tournament.Compute();
            AssertRankingTeams([null, null, null, null, team9, team1, team10, team5, team12, team8, team3, team4]);
            AssertRankingReasons();

            // Set the 3rd place playoff => Rankings 3/4 are now set
            tournament._matches.Single(x => x.Index == 25).SetOutcome(false, 3, 1, MatchOutcomeType.Standard);
            tournament.Compute();
            AssertRankingTeams([null, null, team7, team2, team9, team1, team10, team5, team12, team8, team3, team4]);
            AssertRankingReasons();

            // Set the final => All rankings are now set
            tournament._matches.Single(x => x.Index == 26).SetOutcome(false, 2, 0, MatchOutcomeType.Standard);
            tournament.Compute();
            AssertRankingTeams([team11, team6, team7, team2, team9, team1, team10, team5, team12, team8, team3, team4]);
            AssertRankingReasons();
        }
        else
        {
            // Set the second semi-final => The next two rankings appear with the two losers of the semi-finals since there is no 3rd place playoff
            tournament._matches.Single(x => x.Index == 24).SetOutcome(false, 0, 1, MatchOutcomeType.Standard);
            tournament.Compute();
            AssertRankingTeams([null, null, team2, team7, team9, team1, team10, team5, team12, team8, team3, team4]);
            AssertRankingReasons();

            // Set the final => All rankings are now set
            tournament._matches.Single(x => x.Index == 25).SetOutcome(false, 2, 0, MatchOutcomeType.Standard);
            tournament.Compute();
            AssertRankingTeams([team11, team6, team2, team7, team9, team1, team10, team5, team12, team8, team3, team4]);
            AssertRankingReasons();
        }

        return;

        // Helper function for asserting teams
        void AssertRankingTeams(Team?[] teams)
        {
            for (var i = 0; i < teams.Length; i++)
            {
                var r = tournament._ranking.Single(x => x.Position == i + 1);

                if (teams[i] is null)
                {
                    r.IsDefined.Should().BeFalse();
                    r.Team.Should().BeNull();
                }
                else
                {
                    r.IsDefined.Should().BeTrue();
                    r.Team.Should().Be(teams[i]);
                }
            }
        }

        // The ranking reasons never change during the tournament
        void AssertRankingReasons()
        {
            tournament._ranking.Single(x => x.Position == 1).Reason.Should().Be(RankingReason.WinnerOfFinal);
            tournament._ranking.Single(x => x.Position == 2).Reason.Should().Be(RankingReason.LoserOfFinal);
            tournament._ranking.Single(x => x.Position == 3).Reason.Should().Be(with3rdPlacePlayoff ? RankingReason.WinnerOfThirdPlacePlayoff : RankingReason.QualifiedForSemiFinals);
            tournament._ranking.Single(x => x.Position == 4).Reason.Should().Be(with3rdPlacePlayoff ? RankingReason.LoserOfThirdPlacePlayoff : RankingReason.QualifiedForSemiFinals);
            tournament._ranking.Single(x => x.Position == 5).Reason.Should().Be(RankingReason.QualifiedForQuarterFinals);
            tournament._ranking.Single(x => x.Position == 6).Reason.Should().Be(RankingReason.QualifiedForQuarterFinals);
            tournament._ranking.Single(x => x.Position == 7).Reason.Should().Be(RankingReason.QualifiedForQuarterFinals);
            tournament._ranking.Single(x => x.Position == 8).Reason.Should().Be(RankingReason.QualifiedForQuarterFinals);
            tournament._ranking.Single(x => x.Position == 9).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
            tournament._ranking.Single(x => x.Position == 10).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
            tournament._ranking.Single(x => x.Position == 11).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
            tournament._ranking.Single(x => x.Position == 12).Reason.Should().Be(RankingReason.NotQualifiedForFinals);
        }
    }

    [Fact]
    public void Tournament___Compute_Ranking_Overwrites_Are_Applied_Correctly___Works_As___Expected()
    {
        var tournament = TestTournament.Default;

        // Add group
        var groupA = tournament.AddGroup('A');

        // Add teams
        var team1 = tournament.AddTeam("Team 1");
        var team2 = tournament.AddTeam("Team 2");
        var team3 = tournament.AddTeam("Team 3");
        var team4 = tournament.AddTeam("Team 4");
        var team5 = tournament.AddTeam("Team 5");

        // Assign teams to groups
        tournament.AddGroupParticipant(groupA, team1);
        tournament.AddGroupParticipant(groupA, team2);
        tournament.AddGroupParticipant(groupA, team3);
        tournament.AddGroupParticipant(groupA, team4);
        tournament.AddGroupParticipant(groupA, team5);

        // Generate matches
        tournament.GenerateMatchPlan(new MatchPlanConfiguration
        {
            GroupRoundConfig = new GroupRoundConfig
            {
                GroupMatchOrder = GroupMatchOrder.Sequential,
                GroupPhaseRounds = 1
            }
        });

        // Just to make sure...
        tournament._matches.Should().HaveCount(10);

        // Apply some outcomes
        foreach (var match in tournament._matches)
        {
            match.SetOutcome(false, 0, 0, MatchOutcomeType.Standard);
        }

        // Compute and assert rankings. Because all matches are 0-0, the teams are effectively "ranked" by their ID
        tournament.Compute();
        tournament._ranking.Should().HaveCount(5);
        tournament._ranking.Single(x => x.Position == 1).Team.Should().Be(team1);
        tournament._ranking.Single(x => x.Position == 2).Team.Should().Be(team2);
        tournament._ranking.Single(x => x.Position == 3).Team.Should().Be(team3);
        tournament._ranking.Single(x => x.Position == 4).Team.Should().Be(team4);
        tournament._ranking.Single(x => x.Position == 5).Team.Should().Be(team5);
        tournament._ranking.Should().AllSatisfy(r => r.Reason.Should().Be(RankingReason.RankingViaGroupResults));

        // Add first overwrite
        var hideSecondPlace = tournament.AddRankingOverwrite(2, hideRanking: true);
        tournament.Compute();
        tournament._ranking.Should().HaveCount(4);
        tournament._ranking.Single(x => x.Position == 1).Team.Should().Be(team1);
        tournament._ranking.Single(x => x.Position == 3).Team.Should().Be(team3);
        tournament._ranking.Single(x => x.Position == 4).Team.Should().Be(team4);
        tournament._ranking.Single(x => x.Position == 5).Team.Should().Be(team5);
        tournament._ranking.Should().AllSatisfy(r => r.Reason.Should().Be(RankingReason.RankingViaGroupResults));

        // Add another overwrite
        var setFourthPlaceToTeam2 = tournament.AddRankingOverwrite(4, team2);
        tournament.Compute();
        tournament._ranking.Should().HaveCount(4);
        tournament._ranking.Single(x => x.Position == 1).Team.Should().Be(team1);
        tournament._ranking.Single(x => x.Position == 3).Team.Should().Be(team3);
        tournament._ranking.Single(x => x.Position == 4).Team.Should().Be(team2);
        tournament._ranking.Single(x => x.Position == 5).Team.Should().Be(team5);
        tournament._ranking.Single(x => x.Position == 1).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 3).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 4).Reason.Should().Be(RankingReason.ManuallyChanged);
        tournament._ranking.Single(x => x.Position == 5).Reason.Should().Be(RankingReason.RankingViaGroupResults);

        // Add another overwrite, also for 4th place
        tournament.AddRankingOverwrite(4, team5);
        tournament.Compute();
        tournament._ranking.Should().HaveCount(5);
        tournament._ranking.Single(x => x.Position == 1).Team.Should().Be(team1);
        tournament._ranking.Single(x => x.Position == 3).Team.Should().Be(team3);
        tournament._ranking.Single(x => x.Position == 5).Team.Should().Be(team5);
        tournament._ranking.Single(x => x.Position == 1).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 3).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 5).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        var place4 = tournament._ranking.Where(x => x.Position == 4).ToList();
        place4.Should().HaveCount(2);
        place4.Should().AllSatisfy(r => r.Reason.Should().Be(RankingReason.ManuallyChanged));
        place4.Any(x => x.Team == team2).Should().BeTrue();
        place4.Any(x => x.Team == team5).Should().BeTrue();

        // Remove the second overwrite
        tournament.RemoveRankingOverwrite(setFourthPlaceToTeam2);
        tournament.Compute();
        tournament._ranking.Should().HaveCount(4);
        tournament._ranking.Single(x => x.Position == 1).Team.Should().Be(team1);
        tournament._ranking.Single(x => x.Position == 3).Team.Should().Be(team3);
        tournament._ranking.Single(x => x.Position == 4).Team.Should().Be(team5);
        tournament._ranking.Single(x => x.Position == 5).Team.Should().Be(team5);
        tournament._ranking.Single(x => x.Position == 1).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 3).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 4).Reason.Should().Be(RankingReason.ManuallyChanged);
        tournament._ranking.Single(x => x.Position == 5).Reason.Should().Be(RankingReason.RankingViaGroupResults);

        // Remove the first overwrite
        tournament.RemoveRankingOverwrite(hideSecondPlace);
        tournament.Compute();
        tournament._ranking.Should().HaveCount(5);
        tournament._ranking.Single(x => x.Position == 1).Team.Should().Be(team1);
        tournament._ranking.Single(x => x.Position == 2).Team.Should().Be(team2);
        tournament._ranking.Single(x => x.Position == 3).Team.Should().Be(team3);
        tournament._ranking.Single(x => x.Position == 4).Team.Should().Be(team5);
        tournament._ranking.Single(x => x.Position == 5).Team.Should().Be(team5);
        tournament._ranking.Single(x => x.Position == 1).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 2).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 3).Reason.Should().Be(RankingReason.RankingViaGroupResults);
        tournament._ranking.Single(x => x.Position == 4).Reason.Should().Be(RankingReason.ManuallyChanged);
        tournament._ranking.Single(x => x.Position == 5).Reason.Should().Be(RankingReason.RankingViaGroupResults);
    }
}
