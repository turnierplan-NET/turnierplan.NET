using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Entity;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;
using ClaimTypes = Turnierplan.App.Security.ClaimTypes;

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

        Test(() => new ApiKey(organization, "Test", null, DateTime.MaxValue));
        Test(() => new Image(organization, "Test", "", 0, 1, 1));
        Test(() => new Folder(organization, "Test"));
        Test(() => new Tournament(organization, "Test", Visibility.Public));
        Test(() => new Venue(organization, "Test", ""));

        return;

        void Test<T>(Func<T> factory)
            where T : Entity<long>, IEntityWithRoleAssignments<T>
        {
            var target = factory();

            AccessValidator.IsActionAllowed(target, Actions.GenericRead, principal).Should().BeTrue();
            AccessValidator.IsActionAllowed(target, Actions.GenericWrite, principal).Should().BeFalse();

            AccessValidator.IsActionAllowed(target, Actions.GenericRead, otherPrincipal).Should().BeTrue();
            AccessValidator.IsActionAllowed(target, Actions.GenericWrite, otherPrincipal).Should().BeTrue();
        }
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

        Test(() => new ApiKey(organization, "Test", null, DateTime.MaxValue));
        Test(() => new Image(organization, "Test", "", 0, 1, 1));
        Test(() => new Folder(organization, "Test"));
        Test(() => new Tournament(organization, "Test", Visibility.Public));
        Test(() => new Venue(organization, "Test", ""));

        return;

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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddRolesToResponseHeader___When_User_Is_Admin___Owner_Role_Assignments_Are_Included(bool isAdministrator)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.PrincipalKind, nameof(PrincipalKind.User)),
            new(ClaimTypes.PrincipalId, "3f4647f4-523b-4267-bac9-8f83b0efa9b1")
        ];

        if (isAdministrator)
        {
            claims.Add(new Claim(ClaimTypes.Administrator, "true"));
        }

        var httpContextAccessor = new MockHttpContextAccessor();
        httpContextAccessor.HttpContext!.User = new ClaimsPrincipal([new ClaimsIdentity(claims)]);

        var organization = new Organization("Test");

        var accessValidator = new AccessValidator(httpContextAccessor);
        accessValidator.AddRolesToResponseHeader(organization);

        var organizationId = organization.PublicId.ToString();

        var headers = httpContextAccessor.HttpContext!.Response.Headers;
        var headerValues = headers["X-Turnierplan-Roles"];

        if (isAdministrator)
        {
            headerValues.Should().HaveCount(1);
            headerValues.Single(x => x!.StartsWith(organizationId)).Should().Be($"{organizationId}=Owner");
        }
        else
        {
            headerValues.Should().BeEmpty();
        }
    }

    [Fact]
    public void AddRolesToResponseHeader___When_Called_Such_That_Entities_Are_Processed_More_Than_Once___Duplicates_Are_Not_Included()
    {
        const string userId = "3f4647f4-523b-4267-bac9-8f83b0efa9b1";
        var principal = new Principal(PrincipalKind.User, Guid.Parse(userId));

        var httpContextAccessor = new MockHttpContextAccessor();
        httpContextAccessor.HttpContext!.User = new ClaimsPrincipal([
            new ClaimsIdentity([
                new Claim(ClaimTypes.PrincipalKind, nameof(PrincipalKind.User)),
                new Claim(ClaimTypes.PrincipalId, userId)
            ])
        ]);

        var organization = new Organization("Test");
        organization.AddRoleAssignment(Role.Reader, principal);

        var folder = new Folder(organization, "Test");
        folder.AddRoleAssignment(Role.Contributor, principal);

        var tournament1 = new Tournament(organization, "Test", Visibility.Public);
        tournament1.SetFolder(folder);
        tournament1.AddRoleAssignment(Role.Owner, principal);

        var tournament2 = new Tournament(organization, "Test", Visibility.Public);
        tournament2.SetFolder(folder);

        var accessValidator = new AccessValidator(httpContextAccessor);
        accessValidator.AddRolesToResponseHeader(tournament1);
        accessValidator.AddRolesToResponseHeader(tournament2);

        var headers = httpContextAccessor.HttpContext!.Response.Headers;
        var headerValues = headers["X-Turnierplan-Roles"];

        var tournamentId1 = tournament1.PublicId.ToString();
        var tournamentId2 = tournament2.PublicId.ToString();

        headerValues.Should().HaveCount(2);
        headerValues.Single(x => x!.StartsWith(tournamentId1)).Should().Be($"{tournamentId1}=Owner+Reader+Contributor");
        headerValues.Single(x => x!.StartsWith(tournamentId2)).Should().Be($"{tournamentId2}=Reader+Contributor");
    }

    private sealed class MockHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = new MockHttpContext();
    }

    private sealed class MockHttpContext : HttpContext
    {
        public override void Abort() => throw new NotSupportedException();
        public override IFeatureCollection Features { get; } = null!;
        public override HttpRequest Request { get; } = null!;
        public override HttpResponse Response { get; } = new MockHttpResponse();
        public override ConnectionInfo Connection { get; } = null!;
        public override WebSocketManager WebSockets { get; } = null!;
        public override ClaimsPrincipal User { get; set; } = null!;
        public override IDictionary<object, object?> Items { get; set; } = null!;
        public override IServiceProvider RequestServices { get; set; } = null!;
        public override CancellationToken RequestAborted { get; set; }
        public override string TraceIdentifier { get; set; } = null!;
        public override ISession Session { get; set; } = null!;
    }

    private sealed class MockHttpResponse : HttpResponse
    {
        public override void OnStarting(Func<object, Task> callback, object state) => throw new NotSupportedException();
        public override void OnCompleted(Func<object, Task> callback, object state) => throw new NotSupportedException();
        public override void Redirect(string location, bool permanent) => throw new NotSupportedException();
        public override HttpContext HttpContext => null!;
        public override int StatusCode { get; set; }
        public override IHeaderDictionary Headers { get; } = new HeaderDictionary();
        public override Stream Body { get; set; } = null!;
        public override long? ContentLength { get; set; }
        public override string? ContentType { get; set; }
        public override IResponseCookies Cookies => null!;
        public override bool HasStarted => false;
    }
}
