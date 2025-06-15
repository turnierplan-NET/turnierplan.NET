using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Images;

internal sealed class GetImagesEndpoint : EndpointBase<IEnumerable<ImageDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/images";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId organizationId,
        [FromQuery] ImageType imageType,
        IOrganizationRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var organization = await repository.GetByPublicIdAsync(organizationId, IOrganizationRepository.Include.Images).ConfigureAwait(false);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        var filteredImages = organization.Images.Where(x => x.Type == imageType).ToList();

        return Results.Ok(mapper.MapCollection<ImageDto>(filteredImages));
    }
}
