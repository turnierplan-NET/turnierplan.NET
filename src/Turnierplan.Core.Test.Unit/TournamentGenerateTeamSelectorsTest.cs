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
        var groupDefinitionSelectors = teamSelectors.Skip(7).Take(14).OfType<GroupDefinitionSelector>().ToArray();
        var groupResultSelectors = teamSelectors.Skip(7).Take(14).OfType<GroupResultsSelector>().ToArray();

        groupDefinitionSelectors.Should().HaveCount(7);
        groupDefinitionSelectors[0].TargetGroup.Should().Be(groupA);
        groupDefinitionSelectors[0].TargetTeamIndex.Should().Be(0);
        groupDefinitionSelectors[1].TargetGroup.Should().Be(groupA);
        groupDefinitionSelectors[1].TargetTeamIndex.Should().Be(1);
        groupDefinitionSelectors[2].TargetGroup.Should().Be(groupA);
        groupDefinitionSelectors[2].TargetTeamIndex.Should().Be(2);
        groupDefinitionSelectors[3].TargetGroup.Should().Be(groupA);
        groupDefinitionSelectors[3].TargetTeamIndex.Should().Be(3);
        groupDefinitionSelectors[4].TargetGroup.Should().Be(groupB);
        groupDefinitionSelectors[4].TargetTeamIndex.Should().Be(0);
        groupDefinitionSelectors[5].TargetGroup.Should().Be(groupB);
        groupDefinitionSelectors[5].TargetTeamIndex.Should().Be(1);
        groupDefinitionSelectors[6].TargetGroup.Should().Be(groupB);
        groupDefinitionSelectors[6].TargetTeamIndex.Should().Be(2);

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

        var nthRankedSelectors = teamSelectors.Skip(21).Take(7).OfType<GroupResultsNthRankedSelector>().ToArray();
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

        var matchSelectors = teamSelectors.Skip(28).Take(6).OfType<MatchSelector>().ToArray();
        matchSelectors.Should().HaveCount(6);
        // winner/loser from match index 3
        matchSelectors[0].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 3));
        matchSelectors[0].SelectionMode.Should().Be(MatchSelector.Mode.Winner);
        matchSelectors[1].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 3));
        matchSelectors[1].SelectionMode.Should().Be(MatchSelector.Mode.Loser);
        // winner/loser from match index 4
        matchSelectors[2].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 4));
        matchSelectors[2].SelectionMode.Should().Be(MatchSelector.Mode.Winner);
        matchSelectors[3].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 4));
        matchSelectors[3].SelectionMode.Should().Be(MatchSelector.Mode.Loser);
        // winner/loser from match index 5
        matchSelectors[4].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 5));
        matchSelectors[4].SelectionMode.Should().Be(MatchSelector.Mode.Winner);
        matchSelectors[5].TargetMatch.Should().BeSameAs(tournament._matches.Single(x => x.Index == 5));
        matchSelectors[5].SelectionMode.Should().Be(MatchSelector.Mode.Loser);
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
