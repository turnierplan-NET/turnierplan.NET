using FluentAssertions;
using Turnierplan.App.Helpers;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;
using Xunit;

namespace Turnierplan.App.Test.Unit.Helpers;

public sealed class RbacScopeHelperTest
{
    [Fact]
    public void RbacScopeHelper___GetScopeId___Returns_Expected_Value()
    {
        var organization = new Organization("Test");
        var tournament = new Tournament(organization, "Test", Visibility.Public);

        var organizationId = organization.PublicId.ToString();
        var tournamentId = tournament.PublicId.ToString();

        organization.GetScopeId().Should().Be($"Organization:{organizationId}");
        tournament.GetScopeId().Should().Be($"Tournament:{tournamentId}");
    }

    [Theory]
    [InlineData("ApiKey:NobBmcA2jNc", nameof(ApiKey))]
    [InlineData("Folder:NobBmcA2jNc", nameof(Folder))]
    [InlineData("Image:NobBmcA2jNc", nameof(Image))]
    [InlineData("Organization:NobBmcA2jNc", nameof(Organization))]
    [InlineData("PlanningRealm:NobBmcA2jNc", nameof(PlanningRealm))]
    [InlineData("Tournament:NobBmcA2jNc", nameof(Tournament))]
    [InlineData("Venue:NobBmcA2jNc", nameof(Venue))]
    public void RbacScopeHelper___TryParseScopeId___Works_As_Expected(string scopeId, string expectedTypeName)
    {
        RbacScopeHelper.TryParseScopeId(scopeId, out var actualTypeName, out var objectId).Should().BeTrue();

        actualTypeName.Should().Be(expectedTypeName);
        objectId.Should().Be(new PublicId("NobBmcA2jNc"));
    }

    [Theory]
    [InlineData("InvitationLink:NobBmcA2jNc")]
    [InlineData("ApiKey:NobBmcA2jN")]
    public void RbacScopeHelper___TryParseScopeId_With_Invalid_Scope_Id___Works_As_Expected(string scopeId)
    {
        RbacScopeHelper.TryParseScopeId(scopeId, out var actualTypeName, out var objectId).Should().BeFalse();

        actualTypeName.Should().BeNull();
        objectId.Should().Be(PublicId.Empty);
    }
}
