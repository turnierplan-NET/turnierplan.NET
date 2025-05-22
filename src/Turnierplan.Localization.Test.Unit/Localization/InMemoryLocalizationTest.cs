using System.Globalization;

namespace Turnierplan.Localization.Test.Unit.Localization;

public sealed class InMemoryLocalizationTest
{
    private static readonly IReadOnlyDictionary<string, string> __values = new Dictionary<string, string>
    {
        { "Test0", "Test" },
        { "Test1", "Test {0}" },
        { "Test2", "Test {0} {1}" },
        { "Test3", "Test {0} {1} {2}" },
        { "Test4", "Test {0} {1} {2} {3}" },
        { "Test5", "Test {0} {1} {2} {3} {4}" },
        { "TestNr", "TestNumber {0:F2}" }
    }.AsReadOnly();

    [Fact]
    public void Language___Get___Returns_Correct_Result()
    {
        var language = Make();

        language.Get("Test0").Should().Be("Test");
        language.Get("Test1", "X").Should().Be("Test X");
        language.Get("Test2", "X", "X").Should().Be("Test X X");
        language.Get("Test3", "X", "X", "X").Should().Be("Test X X X");
        language.Get("Test4", "X", "X", "X", "X").Should().Be("Test X X X X");
        language.Get("Test5", "X", "X", "X", "X", "X").Should().Be("Test X X X X X");

        language.Get("KeyWhichDoesNotExist").Should().Be("KeyWhichDoesNotExist");
        language.Get("KeyWhichDoesNotExist", "X").Should().Be("KeyWhichDoesNotExist");
        language.Get("KeyWhichDoesNotExist", "X", "X").Should().Be("KeyWhichDoesNotExist");
        language.Get("KeyWhichDoesNotExist", "X", "X", "X").Should().Be("KeyWhichDoesNotExist");
        language.Get("KeyWhichDoesNotExist", "X", "X", "X", "X").Should().Be("KeyWhichDoesNotExist");
        language.Get("KeyWhichDoesNotExist", "X", "X", "X", "X", "X").Should().Be("KeyWhichDoesNotExist");
    }

    [Theory]
    [InlineData("de", "TestNumber 48,12")]
    [InlineData("en", "TestNumber 48.12")]
    public void Language___Get_With_Culture___Returns_Correct_Result(string cultureName, string expected)
    {
        var language = Make(CultureInfo.GetCultureInfo(cultureName));
        language.Get("TestNr", 48.1244).Should().Be(expected);
    }

    private static InMemoryLocalization Make(CultureInfo? culture = null)
    {
        culture ??= CultureInfo.InvariantCulture;

        return new InMemoryLocalization(culture, __values);
    }
}
