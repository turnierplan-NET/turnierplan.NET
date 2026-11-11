using FluentAssertions;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public sealed class GroupResultsNthRankedTeamSelectorTest
{
    [Theory]
    [InlineData(0, 1, "Team 3")]
    [InlineData(0, 2, "Team 1")]
    [InlineData(0, 3, "Team 2")]
    [InlineData(0, 4, "Team 7")]
    [InlineData(1, 1, "Team 4")]
    [InlineData(1, 2, "Team 5")]
    [InlineData(1, 3, "Team 6")]
    [InlineData(1, 4, "Team 0")]
    public void GroupResultsNthRankedTeamSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Expected_Result(int ordinalNumber, int placementRank, string expectedTeamName)
    {
        // Arrange
        var tournament = new Tournament(string.Empty);
        var teams = Enumerable.Range(0, 8).Select(x => tournament.AddTeam($"Team {x}")).ToArray();
        var groupA = tournament.AddGroup('A');
        var groupB = tournament.AddGroup('B');
        tournament.AddGroupParticipant(groupA, teams[0]).Statistics = new TeamGroupStatistics { Position = 4, Points = 0 };
        tournament.AddGroupParticipant(groupA, teams[1]).Statistics = new TeamGroupStatistics { Position = 2, Points = 6 };
        tournament.AddGroupParticipant(groupA, teams[2]).Statistics = new TeamGroupStatistics { Position = 3, Points = 3 };
        tournament.AddGroupParticipant(groupA, teams[3]).Statistics = new TeamGroupStatistics { Position = 1, Points = 9 };
        tournament.AddGroupParticipant(groupB, teams[4]).Statistics = new TeamGroupStatistics { Position = 1, Points = 5 };
        tournament.AddGroupParticipant(groupB, teams[5]).Statistics = new TeamGroupStatistics { Position = 2, Points = 4 };
        tournament.AddGroupParticipant(groupB, teams[6]).Statistics = new TeamGroupStatistics { Position = 3, Points = 3 };
        tournament.AddGroupParticipant(groupB, teams[7]).Statistics = new TeamGroupStatistics { Position = 4, Points = 2 };
        var selector = new GroupResultsNthRankedTeamSelector([groupA, groupB], ordinalNumber, placementRank);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().NotBeNull();
        team.Name.Should().Be(expectedTeamName);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 0)]
    [InlineData(2, 4)]
    [InlineData(1, 5)]
    public void GroupResultsNthRankedTeamSelector___GetTargetTeam_When_Target_Is_Valid___Returns_Null(int ordinalNumber, int placementRank)
    {
        // Arrange
        var tournament = new Tournament(string.Empty);
        var teams = Enumerable.Range(0, 8).Select(x => tournament.AddTeam(string.Empty)).ToArray();
        var groupA = tournament.AddGroup('A');
        var groupB = tournament.AddGroup('B');
        tournament.AddGroupParticipant(groupA, teams[0]);
        tournament.AddGroupParticipant(groupA, teams[1]);
        tournament.AddGroupParticipant(groupA, teams[2]);
        tournament.AddGroupParticipant(groupA, teams[3]);
        tournament.AddGroupParticipant(groupB, teams[4]);
        tournament.AddGroupParticipant(groupB, teams[5]);
        tournament.AddGroupParticipant(groupB, teams[6]);
        tournament.AddGroupParticipant(groupB, teams[7]);
        var selector = new GroupResultsNthRankedTeamSelector([groupA, groupB], ordinalNumber, placementRank);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void GroupResultsNthRankedTeamSelector___GetTargetTeam_When_Match_Is_Currently_Playing___Returns_Null(bool playingGroupA, bool playingGroupB)
    {
        // Arrange
        var tournament = new Tournament(string.Empty);
        var teams = Enumerable.Range(0, 8).Select(x => tournament.AddTeam($"Team {x}")).ToArray();
        var groupA = tournament.AddGroup('A');
        var groupB = tournament.AddGroup('B');
        tournament.AddGroupParticipant(groupA, teams[0]);
        tournament.AddGroupParticipant(groupA, teams[1]);
        tournament.AddGroupParticipant(groupA, teams[2]);
        tournament.AddGroupParticipant(groupA, teams[3]);
        tournament.AddGroupParticipant(groupB, teams[4]);
        tournament.AddGroupParticipant(groupB, teams[5]);
        tournament.AddGroupParticipant(groupB, teams[6]);
        tournament.AddGroupParticipant(groupB, teams[7]);
        tournament._matches.Add(new Match(1, new NullTeamSelector(), new NullTeamSelector(), groupA)
        {
            IsCurrentlyPlaying = playingGroupA ? true : null,
            ScoreA = playingGroupA ? null : 0,
            ScoreB = playingGroupA ? null : 0,
            OutcomeType = playingGroupA ? null : MatchOutcomeType.Standard
        });
        tournament._matches.Add(new Match(2, new NullTeamSelector(), new NullTeamSelector(), groupB)
        {
            IsCurrentlyPlaying = playingGroupB ? true : null,
            ScoreA = playingGroupB ? null : 0,
            ScoreB = playingGroupB ? null : 0,
            OutcomeType = playingGroupB ? null : MatchOutcomeType.Standard
        });
        var selector = new GroupResultsNthRankedTeamSelector([groupA, groupB], 1, 1);

        // Act
        var team = selector.GetTargetTeam();

        // Assert
        team.Should().BeNull();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 4)]
    public void GroupResultsNthRankedTeamSelector___Equals___Works_As_Expected(int ordinalNumber, int placementRank)
    {
        var tournament = new Tournament(string.Empty);
        var groupA = tournament.AddGroup('A');
        var groupB = tournament.AddGroup('B');
        var groupC = tournament.AddGroup('C');
        var instance1 = new GroupResultsNthRankedTeamSelector([groupA, groupB, groupC], ordinalNumber, placementRank);
        var instance2 = new GroupResultsNthRankedTeamSelector([groupC, groupA, groupB], ordinalNumber, placementRank);

        instance1.GetHashCode().Should().Be(instance2.GetHashCode());

        instance1.Equals(instance1).Should().BeTrue();
        instance2.Equals(instance2).Should().BeTrue();
        instance1.Equals(instance2).Should().BeTrue();
        instance2.Equals(instance1).Should().BeTrue();
        Equals(instance1, instance2).Should().BeTrue();

        instance1.Equals(null).Should().BeFalse();
        instance2.Equals(null).Should().BeFalse();
    }
}
