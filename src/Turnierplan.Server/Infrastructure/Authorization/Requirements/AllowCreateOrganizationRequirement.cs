using Microsoft.AspNetCore.Authorization;

namespace Turnierplan.Server.Infrastructure.Authorization.Requirements;

internal sealed class AllowCreateOrganizationRequirement : IAuthorizationRequirement;
