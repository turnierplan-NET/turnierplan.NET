using Microsoft.AspNetCore.Authorization;
using Turnierplan.Server.Infrastructure.Authentication;
using Turnierplan.Server.Infrastructure.Authorization.Requirements;

namespace Turnierplan.Server.Infrastructure.Authorization.Handlers;

internal sealed class AllowCreateOrganizationAuthorizationHandler : AuthorizationHandler<AllowCreateOrganizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowCreateOrganizationRequirement requirement)
    {
        if (context.User.HasClaim(ClaimTypes.AllowCreateOrganization, ClaimValues.True)
            || context.User.HasClaim(ClaimTypes.IsAdministrator, ClaimValues.True))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
