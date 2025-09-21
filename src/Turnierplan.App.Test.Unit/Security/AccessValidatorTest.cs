using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Test.Unit.Security;

public sealed class AccessValidatorTest
{
    [Fact]
    public void IsActionAllowed___When_Called_With_Basic_Target___Returns_Expected_Result()
    {
        var target = new Organization("Test");

        var principal = new Principal(PrincipalKind.User, Guid.Parse("faa6d5d3-93ad-410e-bc81-171a04cf0130"));
        var otherPrincipal = new Principal(PrincipalKind.User, Guid.Parse("98f8cb8c-606f-47fc-805f-244210e1df51"));

        target.AddRoleAssignment(Role.Reader, principal);
        target.AddRoleAssignment(Role.Contributor, otherPrincipal);

        AccessValidator.IsActionAllowed(target, Actions.GenericRead, principal).Should().BeTrue();
        AccessValidator.IsActionAllowed(target, Actions.GenericWrite, principal).Should().BeFalse();

        AccessValidator.IsActionAllowed(target, Actions.GenericRead, otherPrincipal).Should().BeTrue();
        AccessValidator.IsActionAllowed(target, Actions.GenericWrite, otherPrincipal).Should().BeTrue();
    }

    [Fact]
    public void IsActionAllowed___When_Called_With_Indirect_Target___Returns_Expected_Result()
    {
        var organization = new Organization("Test");

        var principal = new Principal(PrincipalKind.User, Guid.Parse("faa6d5d3-93ad-410e-bc81-171a04cf0130"));
        var otherPrincipal = new Principal(PrincipalKind.User, Guid.Parse("98f8cb8c-606f-47fc-805f-244210e1df51"));

        organization.AddRoleAssignment(Role.Reader, principal);
        organization.AddRoleAssignment(Role.Contributor, otherPrincipal);

        void Test<T>(Func<T> factory)
            where T : Entity<long>, IEntityWithRoleAssignments<T>
        {
            var target = factory();

            AccessValidator.IsActionAllowed(target, Actions.GenericRead, principal).Should().BeTrue();
            AccessValidator.IsActionAllowed(target, Actions.GenericWrite, principal).Should().BeFalse();

            AccessValidator.IsActionAllowed(target, Actions.GenericRead, otherPrincipal).Should().BeTrue();
            AccessValidator.IsActionAllowed(target, Actions.GenericWrite, otherPrincipal).Should().BeTrue();
        }

        Test(() => new ApiKey(organization, "Test", null, DateTime.MaxValue));
        Test(() => new Image(organization, "Test", ImageType.Logo, "", 0, 1, 1));
        Test(() => new Folder(organization, "Test"));
        Test(() => new Tournament(organization, "Test", Visibility.Public));
        Test(() => new Venue(organization, "Test", ""));
    }

    [Fact]
    public void IsActionAllowed___When_Called_With_Tournament_Target_And_Role_Assignment_On_Folder___Returns_Expected_Result()
    {
        var organization = new Organization("Test");
        var folder = new Folder(organization, "Test");

        var principal = new Principal(PrincipalKind.User, Guid.Parse("faa6d5d3-93ad-410e-bc81-171a04cf0130"));
        var otherPrincipal = new Principal(PrincipalKind.User, Guid.Parse("98f8cb8c-606f-47fc-805f-244210e1df51"));

        folder.AddRoleAssignment(Role.Reader, principal);
        folder.AddRoleAssignment(Role.Contributor, otherPrincipal);

        var target = new Tournament(organization, "Test", Visibility.Public);
        target.SetFolder(folder);

        AccessValidator.IsActionAllowed(target, Actions.GenericRead, principal).Should().BeTrue();
        AccessValidator.IsActionAllowed(target, Actions.GenericWrite, principal).Should().BeFalse();

        AccessValidator.IsActionAllowed(target, Actions.GenericRead, otherPrincipal).Should().BeTrue();
        AccessValidator.IsActionAllowed(target, Actions.GenericWrite, otherPrincipal).Should().BeTrue();
    }

    [Fact]
    public void AddAvailableRoles___When_Called_With_Basic_Target___Returns_Expected_Result()
    {
        var target = new Organization("Test");

        var principal = new Principal(PrincipalKind.User, Guid.Parse("faa6d5d3-93ad-410e-bc81-171a04cf0130"));
        var otherPrincipal = new Principal(PrincipalKind.User, Guid.Parse("98f8cb8c-606f-47fc-805f-244210e1df51"));

        target.AddRoleAssignment(Role.Reader, principal);
        target.AddRoleAssignment(Role.Contributor, otherPrincipal);

        var resultSet = new HashSet<Role>();

        AccessValidator.AddAvailableRoles(target, resultSet, principal);
        resultSet.Should().BeEquivalentTo([Role.Reader]);

        resultSet.Clear();
        AccessValidator.AddAvailableRoles(target, resultSet, otherPrincipal);
        resultSet.Should().BeEquivalentTo([Role.Contributor]);
    }

    [Fact]
    public void AddAvailableRoles___When_Called_With_Indirect_Target___Returns_Expected_Result()
    {
        var organization = new Organization("Test");

        var principal = new Principal(PrincipalKind.User, Guid.Parse("faa6d5d3-93ad-410e-bc81-171a04cf0130"));
        var otherPrincipal = new Principal(PrincipalKind.User, Guid.Parse("98f8cb8c-606f-47fc-805f-244210e1df51"));

        organization.AddRoleAssignment(Role.Reader, principal);
        organization.AddRoleAssignment(Role.Contributor, otherPrincipal);

        void Test<T>(Func<T> factory)
            where T : Entity<long>, IEntityWithRoleAssignments<T>
        {
            var target = factory();
            var resultSet = new HashSet<Role>();

            AccessValidator.AddAvailableRoles(target, resultSet, principal);
            resultSet.Should().BeEquivalentTo([Role.Reader]);

            resultSet.Clear();
            AccessValidator.AddAvailableRoles(target, resultSet, otherPrincipal);
            resultSet.Should().BeEquivalentTo([Role.Contributor]);
        }

        Test(() => new ApiKey(organization, "Test", null, DateTime.MaxValue));
        Test(() => new Image(organization, "Test", ImageType.Logo, "", 0, 1, 1));
        Test(() => new Folder(organization, "Test"));
        Test(() => new Tournament(organization, "Test", Visibility.Public));
        Test(() => new Venue(organization, "Test", ""));
    }

    [Fact]
    public void AddAvailableRoles___When_Called_With_Tournament_Target_And_Role_Assignment_On_Folder___Returns_Expected_Result()
    {
        var organization = new Organization("Test");
        var folder = new Folder(organization, "Test");

        var principal = new Principal(PrincipalKind.User, Guid.Parse("faa6d5d3-93ad-410e-bc81-171a04cf0130"));
        var otherPrincipal = new Principal(PrincipalKind.User, Guid.Parse("98f8cb8c-606f-47fc-805f-244210e1df51"));

        folder.AddRoleAssignment(Role.Reader, principal);
        folder.AddRoleAssignment(Role.Contributor, otherPrincipal);

        var target = new Tournament(organization, "Test", Visibility.Public);
        target.SetFolder(folder);

        var resultSet = new HashSet<Role>();
        AccessValidator.AddAvailableRoles(target, resultSet, principal);
        resultSet.Should().BeEquivalentTo([Role.Reader]);

        resultSet.Clear();
        AccessValidator.AddAvailableRoles(target, resultSet, otherPrincipal);
        resultSet.Should().BeEquivalentTo([Role.Contributor]);
    }

    [Fact]
    public void AddAvailableRoles___When_There_Are_Overlapping_Role_Assignments___Returns_Expected_Result()
    {
        var organization = new Organization("Test");
        var folder = new Folder(organization, "Test");
        var target = new Tournament(organization, "Test", Visibility.Public);
        var principal = new Principal(PrincipalKind.User, Guid.Parse("faa6d5d3-93ad-410e-bc81-171a04cf0130"));

        organization.AddRoleAssignment(Role.Reader, principal);
        folder.AddRoleAssignment(Role.Contributor, principal);
        folder.AddRoleAssignment(Role.Reader, principal);
        target.AddRoleAssignment(Role.Owner, principal);
        target.SetFolder(folder);

        var resultSet = new HashSet<Role>();
        AccessValidator.AddAvailableRoles(target, resultSet, principal);
        resultSet.Should().BeEquivalentTo([Role.Reader, Role.Contributor, Role.Owner]);
    }
}
