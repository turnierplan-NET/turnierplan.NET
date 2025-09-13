using Microsoft.AspNetCore.Mvc;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.User;

namespace Turnierplan.App.Endpoints.Principals;

internal sealed class GetPrincipalNameEndpoint : EndpointBase<GetPrincipalNameEndpoint.GetPrincipalNameEndpointResponse>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/principals/name";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PrincipalKind principalKind,
        [FromQuery] Guid principalId,
        IApiKeyRepository apiKeyRepository,
        IUserRepository userRepository)
    {
        string? name;

        switch (principalKind)
        {
            case PrincipalKind.ApiKey:
                var apiKey = await apiKeyRepository.GetByPrincipalIdAsync(principalId);
                name = apiKey?.Name;
                break;
            case PrincipalKind.User:
                var user = await userRepository.GetByPrincipalIdAsync(principalId);
                name = user?.Name;
                break;
            default:
                return Results.BadRequest("Invalid principal kind specified.");
        }

        return name is not null
            ? Results.Ok(new GetPrincipalNameEndpointResponse(name))
            : Results.NotFound();
    }

    public sealed record GetPrincipalNameEndpointResponse(string Name);
}
