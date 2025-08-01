using Turnierplan.App.Extensions;

namespace Turnierplan.App.Test.Unit.Extensions;

public sealed class EnumerableExtensionsTest
{
    [Fact]
    public void None___When_Called_With_Various_Arguments___Works_As_Expected()
    {
        int[] collection1 = [];
        int[] collection2 = [1, 8, 6, 4];

        // Empty array should always return true
        collection1.None(_ => true).Should().BeTrue();
        collection1.None(_ => false).Should().BeTrue();

        collection2.None(x => x >= 10).Should().BeTrue();
        collection2.None(x => x >= 8).Should().BeFalse();
        collection2.None(x => x >= 0).Should().BeFalse();
    }
}
