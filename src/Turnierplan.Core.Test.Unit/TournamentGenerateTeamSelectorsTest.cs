using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit;

public sealed class TournamentGenerateTeamSelectorsTest
{
    [Fact]
    public void Tournament___GenerateAllTeamSelectors___Works_As_Expected()
    {
        var tournament = CreateTestTournament();
        var groupA = tournament._groups.Single(x => x.AlphabeticalId == 'A');
        var groupB = tournament._groups.Single(x => x.AlphabeticalId == 'B');

        var teamSelectors = tournament.GenerateAllTeamSelectors().ToArray();
        teamSelectors.Should().HaveCount(34);

        var staticTeamSelectors = teamSelectors.Take(7).OfType<StaticTeamSelector>().ToArray();
        staticTeamSelectors[0].TargetTeam.Should().BeSameAs(tournament._teams[0]);
        staticTeamSelectors[1].TargetTeam.Should().BeSameAs(tournament._teams[1]);
        staticTeamSelectors[2].TargetTeam.Should().BeSameAs(tournament._teams[2]);
        staticTeamSelectors[3].TargetTeam.Should().BeSameAs(tournament._teams[3]);
        staticTeamSelectors[4].TargetTeam.Should().BeSameAs(tournament._teams[4]);
        staticTeamSelectors[5].TargetTeam.Should().BeSameAs(tournament._teams[5]);
        staticTeamSelectors[6].TargetTeam.Should().BeSameAs(tournament._teams[6]);

        // Group definition/result selectors are interleaved - thus, they have the same skip/take parameters
        var groupDefinitionTeamSelectors = teamSelectors.Skip(7).Take(14).OfType<GroupDefinitionTeamSelector>().ToArray();
        var groupResultSelectors = teamSelectors.Skip(7).Take(14).OfType<GroupResultsTeamSelector>().ToArray();

        groupDefinitionTeamSelectors.Should().HaveCount(7);
        groupDefinitionTeamSelectors[0].TargetGroup.Should().Be(groupA);
        groupDefinitionTeamSelectors[0].TargetTeamIndex.Should().Be(0);
        groupDefinitionTeamSelectors[1].TargetGroup.Should().Be(groupA);
        groupDefinitionTeamSelectors[1].TargetTeamIndex.Should().Be(1);
        groupDefinitionTeamSelectors[2].TargetGroup.Should().Be(groupA);
        groupDefinitionTeamSelectors[2].TargetTeamIndex.Should().Be(2);
        groupDefinitionTeamSelectors[3].TargetGroup.Should().Be(groupA);
        groupDefinitionTeamSelectors[3].TargetTeamIndex.Should().Be(3);
        groupDefinitionTeamSelectors[4].TargetGroup.Should().Be(groupB);
        groupDefinitionTeamSelectors[4].TargetTeamIndex.Should().Be(0);
        groupDefinitionTeamSelectors[5].TargetGroup.Should().Be(groupB);
        groupDefinitionTeamSelectors[5].TargetTeamIndex.Should().Be(1);
        groupDefinitionTeamSelectors[6].TargetGroup.Should().Be(groupB);
        groupDefinitionTeamSelectors[6].TargetTeamIndex.Should().Be(2);

        groupResultSelectors.Should().HaveCount(7);
        groupResultSelectors[0].TargetGroup.Should().Be(groupA);
        groupResultSelectors[0].TargetGroupPosition.Should().Be(1);
        groupResultSelectors[1].TargetGroup.Should().Be(groupA);
        groupResultSelectors[1].TargetGroupPosition.Should().Be(2);
        groupResultSelectors[2].TargetGroup.Should().Be(groupA);
        groupResultSelectors[2].TargetGroupPosition.Should().Be(3);
        groupResultSelectors[3].TargetGroup.Should().Be(groupA);
        groupResultSelectors[3].TargetGroupPosition.Should().Be(4);
        groupResultSelectors[4].TargetGroup.Should().Be(groupB);
        groupResultSelectors[4].TargetGroupPosition.Should().Be(1);
        groupResultSelectors[5].TargetGroup.Should().Be(groupB);
        groupResultSelectors[5].TargetGroupPosition.Should().Be(2);
        groupResultSelectors[6].TargetGroup.Should().Be(groupB);
        groupResultSelectors[6].TargetGroupPosition.Should().Be(3);

        var nthRankedSelectors = teamSelectors.Skip(21).Take(7).OfType<GroupResultsNthRankedTeamSelector>().ToArray();
        nthRankedSelectors.Should().HaveCount(7);
        // placement rank = 1
        nthRankedSelectors[0].OrdinalNumber.Should().Be(0);
        nthRankedSelectors[0].PlacementRank.Should().Be(1);
        nthRankedSelectors[1].OrdinalNumber.Should().Be(1);
        nthRankedSelectors[1].PlacementRank.Should().Be(1);
        // placement rank = 2
        nthRankedSelectors[2].OrdinalNumber.Should().Be(0);
        nthRankedSelectors[2].PlacementRank.Should().Be(2);
        nthRankedSelectors[3].OrdinalNumber.Should().Be(1);
        nthRankedSelectors[3].PlacementRank.Should().Be(2);
        // placement rank = 3
        nthRankedSelectors[4].OrdinalNumber.Should().Be(0);
        nthRankedSelectors[4].PlacementRank.Should().Be(3);
        nthRankedSelectors[5].OrdinalNumber.Should().Be(1);
        nthRankedSelectors[5].PlacementRank.Should().Be(3);
        // placement rank = 4
        nthRankedSelectors[6].OrdinalNumber.Should().Be(0);
        nthRankedSelectors[6].PlacementRank.Should().Be(4);

        var matchTeamSelectors = teamSelectors.Skip(28).Take(6).OfType<MatchTeamSelector>().ToArray();
        matchTeamSelectors.Should().HaveCount(6);
        // winner/loser from match index 3
        matchTeamSelectors[0].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 3));
        matchTeamSelectors[0].SelectionMode.Should().Be(MatchTeamSelector.Mode.Winner);
        matchTeamSelectors[1].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 3));
        matchTeamSelectors[1].SelectionMode.Should().Be(MatchTeamSelector.Mode.Loser);
        // winner/loser from match index 4
        matchTeamSelectors[2].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 4));
        matchTeamSelectors[2].SelectionMode.Should().Be(MatchTeamSelector.Mode.Winner);
        matchTeamSelectors[3].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 4));
        matchTeamSelectors[3].SelectionMode.Should().Be(MatchTeamSelector.Mode.Loser);
        // winner/loser from match index 5
        matchTeamSelectors[4].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 5));
        matchTeamSelectors[4].SelectionMode.Should().Be(MatchTeamSelector.Mode.Winner);
        matchTeamSelectors[5].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 5));
        matchTeamSelectors[5].SelectionMode.Should().Be(MatchTeamSelector.Mode.Loser);
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
        // Also, NullTeamSelector() can be used because the team selector is not accessed by the tested method.

        tournament._matches.Add(new Match(1, new NullTeamSelector(), new NullTeamSelector(), groupA));
        tournament._matches.Add(new Match(2, new NullTeamSelector(), new NullTeamSelector(), groupB));

        tournament._matches.Add(new Match(3, new NullTeamSelector(), new NullTeamSelector(), null));
        tournament._matches.Add(new Match(4, new NullTeamSelector(), new NullTeamSelector(), null));
        tournament._matches.Add(new Match(5, new NullTeamSelector(), new NullTeamSelector(), null));

        return tournament;
    }
}
