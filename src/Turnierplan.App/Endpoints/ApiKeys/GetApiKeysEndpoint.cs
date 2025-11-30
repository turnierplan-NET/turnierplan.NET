using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ApiKeys;

internal sealed class GetApiKeysEndpoint : EndpointBase<IEnumerable<ApiKeyDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/api-keys";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId organizationId,
        IOrganizationRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var organization = await repository.GetByPublicIdAsync(organizationId, IOrganizationRepository.Includes.ApiKeys);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        foreach (var apiKey in organization.ApiKeys)
        {
            accessValidator.AddRolesToResponseHeader(apiKey);
        }

        return Results.Ok(mapper.MapCollection<ApiKeyDto>(organization.ApiKeys));
    }
}
