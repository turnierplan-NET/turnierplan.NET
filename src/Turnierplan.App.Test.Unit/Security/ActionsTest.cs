using Turnierplan.App.Security;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.App.Test.Unit.Security;

public sealed class ActionsTest
{
    [Fact]
    public void IsAllowed___When_Called_With_Various_Roles___Returns_Correct_Value()
    {
        Actions.ReadOrWriteRoleAssignments.IsAllowed([Role.Owner]).Should().BeTrue();
        Actions.ReadOrWriteRoleAssignments.IsAllowed([Role.Contributor]).Should().BeFalse();
        Actions.ReadOrWriteRoleAssignments.IsAllowed([Role.Reader]).Should().BeFalse();

        Actions.GenericWrite.IsAllowed([Role.Owner]).Should().BeTrue();
        Actions.GenericWrite.IsAllowed([Role.Contributor]).Should().BeTrue();
        Actions.GenericWrite.IsAllowed([Role.Reader]).Should().BeFalse();

        Actions.GenericRead.IsAllowed([Role.Owner]).Should().BeTrue();
        Actions.GenericRead.IsAllowed([Role.Contributor]).Should().BeTrue();
        Actions.GenericRead.IsAllowed([Role.Reader]).Should().BeTrue();

        Actions.GenericWrite.IsAllowed([Role.Reader, Role.Contributor]).Should().BeTrue();
        Actions.GenericWrite.IsAllowed([Role.Reader]).Should().BeFalse();
    }
}
