using Turnierplan.Core.Extensions;

namespace Turnierplan.Core.Test.Unit.Tournament.Extensions;

public sealed class EnumerableExtensionsTest
{
    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 1, true)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1, true)]
    [InlineData(new[] { 6, 2, 5, 4, 7, 3, 1, 8 }, 1, true)]
    [InlineData(new[] { 4, 5, 6, 7, 8 }, 4, true)]
    [InlineData(new[] { 2, 3, 4, 5 }, 1, false)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 7, 8 }, 1, false)]
    [InlineData(new[] { 2, 3, 4, 5, 6, 8, 9 }, 1, false)]
    [InlineData(new[] { 2, 6, 9, 3, 4, 8, 5 }, 1, false)]
    [InlineData(new[] { 6, 2, 5, 4, 7, 3, 8 }, 1, false)]
    public void EnumerableExtensions___IsSequential___Returns_Correct_Result(int[] values, int first, bool expectedResult)
    {
        values.IsSequential(first).Should().Be(expectedResult);
    }
}
