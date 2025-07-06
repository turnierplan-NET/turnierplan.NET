using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Organizations;

internal sealed class GetOrganizationEndpoint : EndpointBase<OrganizationDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/organizations/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IOrganizationRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var organization = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        accessValidator.AddRolesToResponseHeader(organization);

        return Results.Ok(mapper.Map<OrganizationDto>(organization));
    }
}
