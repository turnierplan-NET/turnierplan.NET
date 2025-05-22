using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament.TeamSelectors;

public sealed class GroupResultsNthRankedSelectorTest : TeamSelectorTestBase
{
    public static readonly TheoryData<int[], int, int, int?> TeamSelectionTestData = new()
    {
        { [100, 101, 102], 1, 3, null },
        { [100, 101, 102], 2, 3, null },
        { [100, 101, 102], 3, 3, null },
        { [100, 102], -1, 1, null },
        { [100, 102], 0, 0, null },
        { [100, 102], 0, 1, 7 },
        { [100, 102], 0, 2, 5 },
        { [100, 102], 0, 3, 6 },
        { [100, 102], 0, 4, 15 },
        { [100, 102], 1, 1, 12 },
        { [100, 102], 1, 2, 13 },
        { [100, 102], 1, 3, 14 },
        { [100, 102], 1, 4, 4 },
        { [90, 100, 102], 1, 4, 4 },
        { [100, 102], 2, 4, null },
        { [100, 102], 1, 5, null }
    };

    [Theory]
    [MemberData(nameof(TeamSelectionTestData))]
    public void GroupResultsNthRankedSelector___Get_Target_Team_Id___Returns_Expected_Result(int[] targetGroupIds, int ordinalNumber, int placementRank, int? expectedTeamId)
    {
        // Arrange
        var selector = new GroupResultsNthRankedSelector(targetGroupIds, ordinalNumber, placementRank);

        // Act
        var team = selector.GetTargetTeam(TestTournament);

        // Assert
        team?.Id.Should().Be(expectedTeamId);
    }

    [Theory]
    [MemberData(nameof(TeamSelectionTestData))]
    public void GroupResultsNthRankedSelector___Equals___Returns_Correct_Result(int[] targetGroupIds, int ordinalNumber, int placementRank,
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        int? _
#pragma warning restore xUnit1026
        )
    {
        // Arrange
        var instance1 = new GroupResultsNthRankedSelector(targetGroupIds, ordinalNumber, placementRank);
        var instance2 = new GroupResultsNthRankedSelector(targetGroupIds.Reverse().ToArray(), ordinalNumber, placementRank);

        // Act
        var hashCodeEquals = instance1.GetHashCode() == instance2.GetHashCode();
        var results = new []
        {
            instance1.Equals(instance1),
            instance2.Equals(instance2),
            instance1.Equals(instance2),
            instance2.Equals(instance1),
            Equals(instance1, instance2),
            Equals(instance2, instance1),
            ((TeamSelectorBase)instance1).Equals(instance1),
            ((TeamSelectorBase)instance1).Equals(instance2),
            ((TeamSelectorBase)instance2).Equals(instance2),
            ((object)instance1).Equals(instance1),
            ((object)instance1).Equals(instance2),
            ((object)instance2).Equals(instance2)
        };
        var resultsWithNull = new []
        {
            instance1.Equals(null),
            instance2.Equals(null),
            Equals(instance1, null),
            Equals(instance2, null),
            Equals(null, instance1),
            Equals(null, instance2),
            ((TeamSelectorBase)instance1!).Equals(null),
            ((TeamSelectorBase)instance2!).Equals(null),
            ((object)instance1!).Equals(null),
            ((object)instance2!).Equals(null)
        };

        // Assert
        results.Should().AllSatisfy(x => x.Should().BeTrue());
        resultsWithNull.Should().AllSatisfy(x => x.Should().BeFalse());
        hashCodeEquals.Should().BeTrue();
    }
}
