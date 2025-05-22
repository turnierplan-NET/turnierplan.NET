using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament.TeamSelectors;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.Test.Unit.Converters;

public sealed class TeamSelectorConverterTest
{
    public static readonly TheoryData<TeamSelectorBase, string> ValidTestCases = new()
    {
        // GroupDefinitionSelector
        { new GroupDefinitionSelector(1, 1), "G1/1" },
        { new GroupDefinitionSelector(5, 2), "G5/2" },
        { new GroupDefinitionSelector(9, 13), "G9/13" },
        { new GroupDefinitionSelector(17, 4), "G17/4" },

        // GroupResultsNthRankedSelector
        { new GroupResultsNthRankedSelector(new[] {1, 2, 3 }, 1, 1), "N1,2,3/1/1" },
        { new GroupResultsNthRankedSelector(new[] {1, 2, 3 }, 2, 5), "N1,2,3/2/5" },
        { new GroupResultsNthRankedSelector(new[] {1, 2, 3 }, 3, 9), "N1,2,3/3/9" },
        { new GroupResultsNthRankedSelector(new[] {1, 2, 3 }, 4, 7), "N1,2,3/4/7" },
        { new GroupResultsNthRankedSelector(new[] {1, 2}, 4, 7), "N1,2/4/7" },
        { new GroupResultsNthRankedSelector(new[] {1}, 4, 7), "N1/4/7" },

        // GroupResultsSelector
        { new GroupResultsSelector(1, 1), "R1/1" },
        { new GroupResultsSelector(5, 2), "R5/2" },
        { new GroupResultsSelector(9, 13), "R9/13" },
        { new GroupResultsSelector(17, 4), "R17/4" },

        // MatchSelector
        { new MatchSelector(1, MatchSelector.Mode.Winner), "W1" },
        { new MatchSelector(8, MatchSelector.Mode.Winner), "W8" },
        { new MatchSelector(1, MatchSelector.Mode.Loser), "L1" },
        { new MatchSelector(8, MatchSelector.Mode.Loser), "L8" },

        // StaticTeamSelector
        { new StaticTeamSelector(1), "T1" },
        { new StaticTeamSelector(8), "T8" }
    };

    public static readonly TheoryData<string> InvalidTestCases = new()
    {
        // GroupDefinitionSelector
        "G1/",
        "G/1",
        "G11",
        "1/1",

        // GroupResultsNthRankedSelector
        "1,2,3/1/1",
        "N,2,3/1/1",
        "N,2,/1/1",
        "N1,2,/1/1",
        "N1,,3/1/1",
        "N1,2,",
        "N1,2,3/",
        "N1,2,3,1",

        // GroupResultsSelector
        "R1/",
        "R/1",
        "R11",
        "1/1",

        // MatchSelector
        "W",
        "L",
        "Wx",
        "Lx",

        // StaticTeamSelector
        "T",
        "Tx",

        // Other Invalid Strings
        string.Empty,
        "A",
        "1"
    };

    [Theory]
    [MemberData(nameof(ValidTestCases))]
    public void TeamSelectorConverter___Serialize_And_Deserialize___Works_As_Expected(TeamSelectorBase teamSelector, string representation)
    {
        var generatedRepresentation = TeamSelectorConverter.ConvertTeamSelectorToString(teamSelector);
        generatedRepresentation.Should().Be(representation);

        var deserializedTeamSelector = TeamSelectorConverter.ConvertStringToTeamSelector(representation);
        deserializedTeamSelector.Should().Be(teamSelector);
    }

    [Theory]
    [MemberData(nameof(InvalidTestCases))]
    public void TeamSelectorConverter___Deserialize_With_Invalid_Input___Throws_Exception(string input)
    {
        var action = () => TeamSelectorConverter.ConvertStringToTeamSelector(input);

        var expectedError = input.Length == 0
            ? "Invalid team selector string."
            : $"Malformed team selector: '{input}'";

        action.Should().ThrowExactly<TurnierplanException>().WithMessage(expectedError);
    }
}
