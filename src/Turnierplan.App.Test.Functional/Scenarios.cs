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

        var resp = await _testServer.Client.DeleteAsync(Routes.ApiKeys.Delete(apiKeyId), TestContext.Current.CancellationToken);
        resp.EnsureSuccessStatusCode();

        _testServer.ExecuteContextAction(db => db.OrganizationRoleAssignments.Count()).Should().Be(1);
        _testServer.ExecuteContextAction(db => db.TournamentRoleAssignments.Count()).Should().Be(1);

        resp = await _testServer.Client.DeleteAsync(Routes.Users.Delete(userId), TestContext.Current.CancellationToken);
        resp.EnsureSuccessStatusCode();

        _testServer.ExecuteContextAction(db => db.OrganizationRoleAssignments.Count()).Should().Be(0);
        _testServer.ExecuteContextAction(db => db.TournamentRoleAssignments.Count()).Should().Be(0);
    }
}
