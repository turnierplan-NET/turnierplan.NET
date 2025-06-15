using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Folders;

internal sealed class GetFoldersEndpoint : EndpointBase<IEnumerable<FolderDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/folders";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId organizationId,
        IOrganizationRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var organization = await repository.GetByPublicIdAsync(organizationId, IOrganizationRepository.Include.Folders).ConfigureAwait(false);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        return Results.Ok(mapper.MapCollection<FolderDto>(organization.Folders));
    }
}
