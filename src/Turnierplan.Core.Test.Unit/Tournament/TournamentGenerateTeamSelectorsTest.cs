using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament;

public sealed class TournamentGenerateTeamSelectorsTest
{
    [Fact]
    public void Tournament___GenerateAllTeamSelectors___Works_As_Expected()
    {
        // Arrange
        var tournament = CreateTestTournament();

        // Act
        var teamSelectors = tournament.GenerateAllTeamSelectors().ToList();

        // Assert
        teamSelectors.Should().HaveCount(34);
        var i = 0;

        teamSelectors[i++].Should().BeEquivalentTo(new StaticTeamSelector(0));
        teamSelectors[i++].Should().BeEquivalentTo(new StaticTeamSelector(1));
        teamSelectors[i++].Should().BeEquivalentTo(new StaticTeamSelector(2));
        teamSelectors[i++].Should().BeEquivalentTo(new StaticTeamSelector(3));
        teamSelectors[i++].Should().BeEquivalentTo(new StaticTeamSelector(4));
        teamSelectors[i++].Should().BeEquivalentTo(new StaticTeamSelector(5));
        teamSelectors[i++].Should().BeEquivalentTo(new StaticTeamSelector(6));

        teamSelectors[i++].Should().BeEquivalentTo(new GroupDefinitionSelector(7, 0));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsSelector(7, 1));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupDefinitionSelector(7, 1));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsSelector(7, 2));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupDefinitionSelector(7, 2));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsSelector(7, 3));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupDefinitionSelector(7, 3));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsSelector(7, 4));

        teamSelectors[i++].Should().BeEquivalentTo(new GroupDefinitionSelector(8, 0));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsSelector(8, 1));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupDefinitionSelector(8, 1));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsSelector(8, 2));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupDefinitionSelector(8, 2));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsSelector(8, 3));

        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 0, 1));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 1, 1));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 0, 2));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 1, 2));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 0, 3));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 1, 3));
        teamSelectors[i++].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 0, 4));

        teamSelectors[i++].Should().BeEquivalentTo(new MatchSelector(3, MatchSelector.Mode.Winner));
        teamSelectors[i++].Should().BeEquivalentTo(new MatchSelector(3, MatchSelector.Mode.Loser));
        teamSelectors[i++].Should().BeEquivalentTo(new MatchSelector(4, MatchSelector.Mode.Winner));
        teamSelectors[i++].Should().BeEquivalentTo(new MatchSelector(4, MatchSelector.Mode.Loser));
        teamSelectors[i++].Should().BeEquivalentTo(new MatchSelector(5, MatchSelector.Mode.Winner));
        teamSelectors[i].Should().BeEquivalentTo(new MatchSelector(5, MatchSelector.Mode.Loser));
    }

    private static Core.Tournament.Tournament CreateTestTournament()
    {
        var tournament = TestTournament.Default;

        for (var i = 0; i < 7; i++)
        {
            tournament._teams.Add(new Team(i, $"Team {i}"));
        }

        var groupA = new Group(7, 'A');
        var groupB = new Group(8, 'B');

        tournament._groups.Add(groupA);
        tournament._groups.Add(groupB);

        groupA._participants.Add(new GroupParticipant(groupA, tournament._teams[0], 0, 0));
        groupA._participants.Add(new GroupParticipant(groupA, tournament._teams[1], 1, 0));
        groupA._participants.Add(new GroupParticipant(groupA, tournament._teams[2], 2, 0));
        groupA._participants.Add(new GroupParticipant(groupA, tournament._teams[3], 3, 0));

        groupB._participants.Add(new GroupParticipant(groupB, tournament._teams[4], 0, 0));
        groupB._participants.Add(new GroupParticipant(groupB, tournament._teams[5], 1, 0));
        groupB._participants.Add(new GroupParticipant(groupB, tournament._teams[6], 2, 0));

        // For test purposes, we don't need all matches which would be generated for such a tournament.
        // Also, NullSelector() can be used because the team selector is not accessed by the tested method.

        tournament._matches.Add(new Match(9, 1, new NullSelector(), new NullSelector(), groupA));
        tournament._matches.Add(new Match(10, 2, new NullSelector(), new NullSelector(), groupB));

        tournament._matches.Add(new Match(11, 3, new NullSelector(), new NullSelector(), null));
        tournament._matches.Add(new Match(12, 4, new NullSelector(), new NullSelector(), null));
        tournament._matches.Add(new Match(13, 5, new NullSelector(), new NullSelector(), null));

        return tournament;
    }
}
