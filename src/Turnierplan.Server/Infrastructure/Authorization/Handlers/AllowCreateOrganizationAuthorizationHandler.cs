using Microsoft.AspNetCore.Authorization;
using Turnierplan.Server.Infrastructure.Authorization.Requirements;

namespace Turnierplan.Server.Infrastructure.Authorization.Handlers;

internal sealed class AllowCreateOrganizationAuthorizationHandler : AuthorizationHandler<AllowCreateOrganizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowCreateOrganizationRequirement requirement)
    {
        // TODO: Check if user has the 'allow create org' flag or the user is an administrator
        context.Fail();
        return Task.CompletedTask;
    }
}
