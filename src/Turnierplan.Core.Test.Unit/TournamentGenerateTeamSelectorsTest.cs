using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit;

public sealed class TournamentGenerateTeamSelectorsTest
{
    [Fact]
    public void Tournament___GenerateAllTeamSelectors___Works_As_Expected()
    {
        // Arrange
        var tournament = CreateTestTournament();

        // Act
        var teamSelectors = tournament.GenerateAllTeamSelectors().ToList();
        var groupA = tournament._groups.Single(x => x.AlphabeticalId == 'A');
        var groupB = tournament._groups.Single(x => x.AlphabeticalId == 'B');

        // Assert
        teamSelectors.Should().HaveCount(34);

        // teamSelectors[0].Should().BeEquivalentTo(new StaticTeamSelector(0));
        // teamSelectors[1].Should().BeEquivalentTo(new StaticTeamSelector(1));
        // teamSelectors[2].Should().BeEquivalentTo(new StaticTeamSelector(2));
        // teamSelectors[3].Should().BeEquivalentTo(new StaticTeamSelector(3));
        // teamSelectors[4].Should().BeEquivalentTo(new StaticTeamSelector(4));
        // teamSelectors[5].Should().BeEquivalentTo(new StaticTeamSelector(5));
        // teamSelectors[6].Should().BeEquivalentTo(new StaticTeamSelector(6));

        // teamSelectors[7].Should().BeEquivalentTo(new GroupDefinitionSelector(7, 0));
        // teamSelectors[8].Should().BeEquivalentTo(new GroupResultsSelector(7, 1));
        // teamSelectors[9].Should().BeEquivalentTo(new GroupDefinitionSelector(7, 1));
        // teamSelectors[10].Should().BeEquivalentTo(new GroupResultsSelector(7, 2));
        // teamSelectors[11].Should().BeEquivalentTo(new GroupDefinitionSelector(7, 2));
        // teamSelectors[12].Should().BeEquivalentTo(new GroupResultsSelector(7, 3));
        // teamSelectors[13].Should().BeEquivalentTo(new GroupDefinitionSelector(7, 3));
        // teamSelectors[14].Should().BeEquivalentTo(new GroupResultsSelector(7, 4));

        // teamSelectors[15].Should().BeEquivalentTo(new GroupDefinitionSelector(8, 0));
        // teamSelectors[16].Should().BeEquivalentTo(new GroupResultsSelector(8, 1));
        // teamSelectors[17].Should().BeEquivalentTo(new GroupDefinitionSelector(8, 1));
        // teamSelectors[18].Should().BeEquivalentTo(new GroupResultsSelector(8, 2));
        // teamSelectors[19].Should().BeEquivalentTo(new GroupDefinitionSelector(8, 2));
        // teamSelectors[20].Should().BeEquivalentTo(new GroupResultsSelector(8, 3));

        // teamSelectors[21].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 0, 1));
        // teamSelectors[22].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 1, 1));
        // teamSelectors[23].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 0, 2));
        // teamSelectors[24].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 1, 2));
        // teamSelectors[25].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 0, 3));
        // teamSelectors[26].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 1, 3));
        // teamSelectors[27].Should().BeEquivalentTo(new GroupResultsNthRankedSelector([7, 8], 0, 4));
        //
        // teamSelectors[28].Should().BeEquivalentTo(new MatchSelector(3, MatchSelector.Mode.Winner));
        // teamSelectors[29].Should().BeEquivalentTo(new MatchSelector(3, MatchSelector.Mode.Loser));
        // teamSelectors[30].Should().BeEquivalentTo(new MatchSelector(4, MatchSelector.Mode.Winner));
        // teamSelectors[31].Should().BeEquivalentTo(new MatchSelector(4, MatchSelector.Mode.Loser));
        // teamSelectors[32].Should().BeEquivalentTo(new MatchSelector(5, MatchSelector.Mode.Winner));
        // teamSelectors[33].Should().BeEquivalentTo(new MatchSelector(5, MatchSelector.Mode.Loser));
    }

    private static Tournament CreateTestTournament()
    {
        var tournament = new Tournament(string.Empty);

        for (var i = 0; i < 7; i++)
        {
            tournament._teams.Add(new Team($"Team {i}", false));
        }

        var groupA = new Group('A');
        var groupB = new Group('B');

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

        tournament._matches.Add(new Match(1, new NullSelector(), new NullSelector(), groupA));
        tournament._matches.Add(new Match(2, new NullSelector(), new NullSelector(), groupB));

        tournament._matches.Add(new Match(3, new NullSelector(), new NullSelector(), null));
        tournament._matches.Add(new Match(4, new NullSelector(), new NullSelector(), null));
        tournament._matches.Add(new Match(5, new NullSelector(), new NullSelector(), null));

        return tournament;
    }
}
