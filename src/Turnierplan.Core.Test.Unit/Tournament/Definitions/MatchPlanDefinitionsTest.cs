using Turnierplan.Core.Tournament.Definitions;

namespace Turnierplan.Core.Test.Unit.Tournament.Definitions;

public sealed class MatchPlanDefinitionsTest
{
    [Fact]
    public void MatchPlanDefinitions___Get_All_Group_Match_Definitions___Returns_Expected_Result()
    {
        // Act
        var definitions = MatchPlanDefinitions.GetAllGroupMatchDefinitions().ToList();

        // Assert
        definitions.Should().HaveCount(8);
        definitions.Select(x => x.TeamCount).Should().BeEquivalentTo(Enumerable.Range(2, 8));
        definitions.Should().AllSatisfy(x => x.Definition.MatchBlocks.Select(y => y.Matches.Length).Distinct().Count().Should().Be(1));
    }

    [Fact]
    public void MatchPlanDefinitions___Get_Group_Match_Definition_For_2_Teams___Returns_Expected_Result()
    {
        // Act
        var definition = MatchPlanDefinitions.GetGroupMatchDefinition(2);

        // Assert
        definition.Should().NotBeNull();
        definition!.BlockCount.Should().Be(1);
        definition.MatchBlocks.Should().BeEquivalentTo(new GroupMatchDefinition.MatchBlock[]
        {
            new([new GroupMatchDefinition.MatchDefinition(0, 1)])
        });
    }

    [Fact]
    public void MatchPlanDefinitions___All_Group_Match_Definitions___Contain_All_Required_Matches()
    {
        var definitions = MatchPlanDefinitions.GetAllGroupMatchDefinitions().ToList();

        foreach ((var teamCount, GroupMatchDefinition definition) in definitions)
        {
            var allMatches = definition.MatchBlocks.SelectMany(x => x.Matches).ToList();

            var expectedNumberOfMatches = (teamCount * (teamCount - 1)) / 2;
            allMatches.Count.Should().Be(expectedNumberOfMatches);

            for (var i = 0; i < teamCount; i++)
            {
                for (var j = 0; j < teamCount; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var count = 0;

                    foreach (var (teamA, teamB) in allMatches)
                    {
                        if ((teamA == i && teamB == j) || (teamA == j && teamB == i))
                        {
                            count++;
                        }
                    }

                    count.Should().Be(1, $"there should be exactly one match where team {i + 1} plays against team {j + 1} in group with n={teamCount}");
                }
            }
        }
    }

    [Fact]
    public void MatchPlanDefinitions___Get_Group_Match_Definition_For_4_Teams___Returns_Expected_Result()
    {
        // Act
        var definition = MatchPlanDefinitions.GetGroupMatchDefinition(4);

        // Assert
        definition.Should().NotBeNull();
        definition!.BlockCount.Should().Be(3);
        definition.MatchBlocks.Should().BeEquivalentTo(new GroupMatchDefinition.MatchBlock[]
        {
            new([
                new GroupMatchDefinition.MatchDefinition(0, 1),
                new GroupMatchDefinition.MatchDefinition(2, 3)
            ]),
            new([
                new GroupMatchDefinition.MatchDefinition(0, 2),
                new GroupMatchDefinition.MatchDefinition(1, 3)
            ]),
            new([
                new GroupMatchDefinition.MatchDefinition(3, 0),
                new GroupMatchDefinition.MatchDefinition(1, 2)
            ])
        });
    }

    [Fact]
    public void MatchPlanDefinitions___Get_All_Finals_Match_Definitions___Returns_Expected_Result()
    {
        // Act
        var definitions = MatchPlanDefinitions.GetAllFinalsMatchDefinitions().ToList();

        // Assert
        definitions.Should().HaveCount(11);
    }

    [Fact]
    public void MatchPlanDefinitions___Get_Group_Match_Definition_For_1_Group_And_1_Match___Returns_Expected_Result()
    {
        // Act
        var definition = MatchPlanDefinitions.GetFinalsMatchDefinition(1, 1);

        // Assert
        definition.Should().NotBeNull();
        definition!.Should().BeEquivalentTo(new FinalsMatchDefinition(new FinalsMatchDefinition.MatchDefinition[]
        {
            new(new AbstractTeamSelector(false, 0, 1, null),
                new AbstractTeamSelector(false, 0, 2, null))
        }));
    }

    [Fact]
    public void MatchPlanDefinitions___Get_Group_Match_Definition_For_3_Groups_And_2_Matches___Returns_Expected_Result()
    {
        // Act
        var definition = MatchPlanDefinitions.GetFinalsMatchDefinition(3, 2);

        // Assert
        definition.Should().NotBeNull();
        definition!.Should().BeEquivalentTo(new FinalsMatchDefinition(new FinalsMatchDefinition.MatchDefinition[]
        {
            new(new AbstractTeamSelector(false, 0, 1, null),
                new AbstractTeamSelector(false, 1, 1, null)),

            new(new AbstractTeamSelector(false, 2, 1, null),
                new AbstractTeamSelector(true, null, 2, 0))
        }));
    }

    [Fact]
    public void MatchPlanDefinitions___Get_Group_Match_Definition_For_5_Groups_And_4_Matches___Returns_Expected_Result()
    {
        // Act
        var definition = MatchPlanDefinitions.GetFinalsMatchDefinition(5, 4);

        // Assert
        definition.Should().NotBeNull();
        definition!.Should().BeEquivalentTo(new FinalsMatchDefinition(new FinalsMatchDefinition.MatchDefinition[]
        {
            new(new AbstractTeamSelector(false, 0, 1, null),
                new AbstractTeamSelector(false, 1, 1, null)),

            new(new AbstractTeamSelector(false, 2, 1, null),
                new AbstractTeamSelector(true, null, 2, 0)),

            new(new AbstractTeamSelector(false, 3, 1, null),
                new AbstractTeamSelector(true, null, 2, 1)),

            new(new AbstractTeamSelector(false, 4, 1, null),
                new AbstractTeamSelector(true, null, 2, 2))
        }));
    }
}
