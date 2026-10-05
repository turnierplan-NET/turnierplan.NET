using Microsoft.AspNetCore.Authorization;
using Turnierplan.Server.Infrastructure.Authentication.Extensions;
using Turnierplan.Server.Infrastructure.Authorization.Requirements;

namespace Turnierplan.Server.Infrastructure.Authorization.Handlers;

internal sealed class AllowCreateOrganizationAuthorizationHandler : AuthorizationHandler<AllowCreateOrganizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowCreateOrganizationRequirement requirement)
    {
        if (context.User.HasAllowCreateOrganizationClaim() || context.User.HasIsAdministratorClaim())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
