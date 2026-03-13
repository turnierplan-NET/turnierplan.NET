using System.Net;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Kiota.Abstractions;
using Turnierplan.App.Test.Functional.Client.Models;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Organization;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.User;
using Xunit;
using Role = Turnierplan.Core.RoleAssignment.Role;
using Visibility = Turnierplan.Core.Tournament.Visibility;

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

        await _testServer.Client.ApiKeys[apiKeyId].DeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        _testServer.ExecuteContextAction(db => db.OrganizationRoleAssignments.Count()).Should().Be(1);
        _testServer.ExecuteContextAction(db => db.TournamentRoleAssignments.Count()).Should().Be(1);

        await _testServer.Client.Users[userId].DeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        _testServer.ExecuteContextAction(db => db.OrganizationRoleAssignments.Count()).Should().Be(0);
        _testServer.ExecuteContextAction(db => db.TournamentRoleAssignments.Count()).Should().Be(0);
    }

    [Fact]
    public async Task New_User_Can_Not_Create_Organization_Unless_Explicitly_Granted_Permission()
    {
        const string newUserName = "test_user";
        const string newUserPassword = "test123";

        await _testServer.Client.Users.PostAsync(
            new CreateUserEndpointRequest { UserName = newUserName, Password = newUserPassword },
            cancellationToken: TestContext.Current.CancellationToken);

        {
            var userClient = await _testServer.CreateClientForUserAsync(newUserName, newUserPassword);

            var tryToCreateOrganization = () => userClient.Organizations.PostAsync(
                new CreateOrganizationEndpointRequest { Name = "test_org" },
                cancellationToken: TestContext.Current.CancellationToken);

            await ExpectErrorAsync(tryToCreateOrganization, HttpStatusCode.Forbidden);
        }

        // extra step required to get ID of new user
        var allUsers = await _testServer.Client.Users.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        var newUserId = allUsers!.Single(x => x.UserName!.Equals(newUserName)).Id; // TODO: UserName and Id should not be nullable?

        await _testServer.Client.Users[newUserId!.Value].PutAsync(new UpdateUserEndpointRequest
        {
            UserName = newUserName,
            IsAdministrator = false,
            AllowCreateOrganization = true,
            UpdatePassword = false
        }, cancellationToken: TestContext.Current.CancellationToken);

        {
            // We need to create a new client because a fresh login is required to get the new claims in the token
            var userClient = await _testServer.CreateClientForUserAsync(newUserName, newUserPassword);

            await userClient.Organizations.PostAsync(
                new CreateOrganizationEndpointRequest { Name = "test_org" },
                cancellationToken: TestContext.Current.CancellationToken);
        }
    }

    // TODO: Make pretty
    private async Task ExpectErrorAsync(Func<Task> func, HttpStatusCode code)
    {
        ApiException? ex = null;

        try
        {
            await func();
        }
        catch (ApiException ex2)
        {
            ex = ex2;
        }

        ex.Should().NotBeNull();
        ex.ResponseStatusCode.Should().Be((int)code);
    }
}
