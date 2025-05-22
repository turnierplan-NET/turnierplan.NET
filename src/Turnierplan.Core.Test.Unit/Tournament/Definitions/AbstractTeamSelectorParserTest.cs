using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament.Definitions;

namespace Turnierplan.Core.Test.Unit.Tournament.Definitions;

public sealed class AbstractTeamSelectorParserTest
{
    public static readonly TheoryData<string, string, object> ParseAbstractTeamSelectorTestData = new()
    {
        { "1.0", "A1", new AbstractTeamSelector(false, 0, 1, null) },
        { "2.0", "A2", new AbstractTeamSelector(false, 0, 2, null) },
        { "1.1", "B1", new AbstractTeamSelector(false, 1, 1, null) },
        { "3.2", "C3", new AbstractTeamSelector(false, 2, 3, null) },
        { "4.25", "Z4", new AbstractTeamSelector(false, 25, 4, null) },

        { "0B1", "1B1", new AbstractTeamSelector(true, null, 1, 0) },
        { "2B1", "3B1", new AbstractTeamSelector(true, null, 1, 2) },
        { "1B3", "2B3", new AbstractTeamSelector(true, null, 3, 1) },
        { "4B4", "5B4", new AbstractTeamSelector(true, null, 4, 4) }
    };

    [Theory]
    [MemberData(nameof(ParseAbstractTeamSelectorTestData))]
    public void AbstractTeamSelectorParser___Parse_Valid_Abstract_Team_Selector___Works_As_Expected(string input,
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        string _,
#pragma warning restore xUnit1026
        object expected)
    {
        // Arrange
        var expectedTeamSelector = (AbstractTeamSelector)expected;

        // Act
        var parsed = AbstractTeamSelectorParser.ParseAbstractTeamSelector(input);

        // Assert
        parsed.Should().BeEquivalentTo(expectedTeamSelector);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("0")]
    [InlineData("A0")]
    [InlineData("00")]
    [InlineData("_0")]
    [InlineData("2B0")]
    [InlineData("0C2")]
    public void AbstractTeamSelectorParser___Parse_Invalid_Abstract_Team_Selector___Throws_Exception(string input)
    {
        // Act
        var func = void () => AbstractTeamSelectorParser.ParseAbstractTeamSelector(input);

        // Assert
        func.Should().ThrowExactly<TurnierplanException>().WithMessage($"The abstract team selector '{input}' is not valid.");
    }

    [Theory]
    [MemberData(nameof(ParseAbstractTeamSelectorTestData))]
    public void AbstractTeamSelectorParser___Parse_Valid_Abstract_Team_Selector_With_Definitions_Format___Works_As_Expected(
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        string _,
#pragma warning restore xUnit1026
        string input,
        object expected)
    {
        // Arrange
        var expectedTeamSelector = (AbstractTeamSelector)expected;

        // Act
        var parsed = AbstractTeamSelectorParser.ParseAbstractTeamSelectorFromDefinitionFormat(input);

        // Assert
        parsed.Should().BeEquivalentTo(expectedTeamSelector);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("0")]
    [InlineData("A0")]
    [InlineData("00")]
    [InlineData("_0")]
    [InlineData("0B2")]
    [InlineData("2B0")]
    [InlineData("0C2")]
    public void AbstractTeamSelectorParser___Parse_Invalid_Abstract_Team_Selector_With_Definitions_Format___Throws_Exception(string input)
    {
        // Act
        var func = void () => AbstractTeamSelectorParser.ParseAbstractTeamSelectorFromDefinitionFormat(input);

        // Assert
        func.Should().ThrowExactly<InvalidOperationException>().WithMessage($"Invalid abstract team selector: '{input}'");
    }
}
