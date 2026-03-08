using FluentAssertions;
using Turnierplan.Core.Exceptions;
using Xunit;

namespace Turnierplan.Core.Test.Unit.User;

public sealed class UserTest
{
    [Theory]
    [InlineData(true, true, false, true)]
    [InlineData(false, true, true, true)]
    [InlineData(false, false, true, true)]
    public void User___When_SetIsAdministrator_Is_Called___AllowCreateOrganization_Is_Updated_As_Expected(
        bool currentIsAdministrator,
        bool currentAllowCreateOrganization,
        bool setIsAdministrator,
        bool expectedAllowCreateOrganization)
    {
        var user = new Core.User.User(Guid.Empty, Guid.Empty, DateTime.UtcNow, string.Empty, string.Empty, null, null, null, string.Empty, currentIsAdministrator, currentAllowCreateOrganization, DateTime.UtcNow, Guid.Empty);

        user.IsAdministrator.Should().Be(currentIsAdministrator);
        user.AllowCreateOrganization.Should().Be(currentAllowCreateOrganization);

        user.SetIsAdministrator(setIsAdministrator);

        user.IsAdministrator.Should().Be(setIsAdministrator);
        user.AllowCreateOrganization.Should().Be(expectedAllowCreateOrganization);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void User___When_SetAllowCreateOrganization_Is_Called_On_Non_Admin_User___Works_As_Expected(
        bool currentAllowCreateOrganization,
        bool setAllowCreateOrganization)
    {
        var user = new Core.User.User(Guid.Empty, Guid.Empty, DateTime.UtcNow, string.Empty, string.Empty, null, null, null, string.Empty, false, currentAllowCreateOrganization, DateTime.UtcNow, Guid.Empty);

        user.IsAdministrator.Should().Be(false);
        user.AllowCreateOrganization.Should().Be(currentAllowCreateOrganization);

        user.SetAllowCreateOrganization(setAllowCreateOrganization);

        user.AllowCreateOrganization.Should().Be(setAllowCreateOrganization);
    }

    [Fact]
    public void User___When_SetAllowCreateOrganization_Is_Called_With_False_On_Admin_User___Works_As_Expected()
    {
        var user = new Core.User.User(Guid.Empty, Guid.Empty, DateTime.UtcNow, string.Empty, string.Empty, null, null, null, string.Empty, true, true, DateTime.UtcNow, Guid.Empty);

        user.IsAdministrator.Should().Be(true);
        user.AllowCreateOrganization.Should().Be(true);

        var action = () => user.SetAllowCreateOrganization(false);
        action.Should().ThrowExactly<TurnierplanException>().WithMessage("'AllowCreateOrganization' cannot be set to false in an administrator user.");
    }
}
