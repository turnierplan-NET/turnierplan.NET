using FluentAssertions;
using FluentAssertions.Extensions;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Organization;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.User;
using Xunit;

namespace Turnierplan.App.Test.Functional;

public sealed class Scenarios
{
    private readonly TestServer _testServer = new();

    [Fact]
    public async Task When_ApiKey_And_User_Are_Deleted_The_Role_Assignments_Are_Also_Removed()
    {
        var (apiKeyId, userId) = _testServer.ExecuteContextAction(db =>
        {
            var user = new User(string.Empty);
            db.Users.Add(user);

            var org = new Organization(string.Empty);
            db.Organizations.Add(org);

            var key = new ApiKey(org, string.Empty, null, DateTime.UtcNow + 1.Days());
            db.ApiKeys.Add(key);

            var tournament = new Tournament(org, string.Empty, Visibility.Public);
            db.Tournaments.Add(tournament);

            org.AddRoleAssignment(Role.Reader, user.AsPrincipal());
            tournament.AddRoleAssignment(Role.Contributor, user.AsPrincipal());
            tournament.AddRoleAssignment(Role.Reader, key.AsPrincipal());

            db.SaveChanges();

            return (key.PublicId, user.Id);
        });

        _testServer.ExecuteContextAction(db => db.OrganizationRoleAssignments.Count()).Should().Be(1);
        _testServer.ExecuteContextAction(db => db.TournamentRoleAssignments.Count()).Should().Be(2);

        await _testServer.Client.Api.ApiKeys[apiKeyId].DeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        _testServer.ExecuteContextAction(db => db.OrganizationRoleAssignments.Count()).Should().Be(1);
        _testServer.ExecuteContextAction(db => db.TournamentRoleAssignments.Count()).Should().Be(1);

        await _testServer.Client.Api.Users[userId].DeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        _testServer.ExecuteContextAction(db => db.OrganizationRoleAssignments.Count()).Should().Be(0);
        _testServer.ExecuteContextAction(db => db.TournamentRoleAssignments.Count()).Should().Be(0);
    }

    /*
    [Fact]
    public async Task New_User_Can_Not_Create_Organization_Unless_Explicitly_Granted_Permission()
    {
        const string newUserName = "test_user";
        const string newUserPassword = "test123";

        var resp = await _testServer.Client.PostAsJsonAsync(
            Routes.Users.Create(),
            new { UserName = newUserName, Password = newUserPassword },
            TestContext.Current.CancellationToken);
        resp.EnsureSuccessStatusCode();

        var userClient = _testServer.CreateNewClientAndLogIn(newUserName, newUserPassword);
        resp = await userClient.PostAsJsonAsync(
            Routes.Organizations.Create(),
            new { Name = "test_org" },
            TestContext.Current.CancellationToken);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // extra step required to get ID of new user
        resp = await _testServer.Client.GetAsync(Routes.Users.List(), TestContext.Current.CancellationToken);
        resp.EnsureSuccessStatusCode();
        var allUsers = await resp.Content.ReadFromJsonAsync<UserDto[]>(TestContext.Current.CancellationToken);
        var newUserId = allUsers!.Single(x => x.UserName.Equals(newUserName)).Id;

        resp = await _testServer.Client.PutAsJsonAsync(
            Routes.Users.Update(newUserId),
            new { UserName = newUserName, IsAdministrator = false, AllowCreateOrganization = true, UpdatePassword = false },
            TestContext.Current.CancellationToken);
        resp.EnsureSuccessStatusCode();

        userClient = _testServer.CreateNewClientAndLogIn(newUserName, newUserPassword);
        resp = await userClient.PostAsJsonAsync(
            Routes.Organizations.Create(),
            new { Name = "test_org" },
            TestContext.Current.CancellationToken);
        resp.EnsureSuccessStatusCode();
    }*/
}
