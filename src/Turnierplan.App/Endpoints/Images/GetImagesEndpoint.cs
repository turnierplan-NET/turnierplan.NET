using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Images;

internal sealed class GetImagesEndpoint : EndpointBase<GetImagesEndpoint.GetImagesEndpointResponse>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/images";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId organizationId,
        [FromQuery] ImageType? imageType,
        [FromQuery] bool? includeReferences,
        IOrganizationRepository organizationRepository,
        IImageRepository imageRepository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var organization = await organizationRepository.GetByPublicIdAsync(organizationId, IOrganizationRepository.Includes.Images);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        var filteredImages = imageType.HasValue
            ? organization.Images.Where(x => x.Type == imageType)
            : organization.Images;

        var filteredAndSortedImages = filteredImages
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        foreach (var image in filteredAndSortedImages)
        {
            accessValidator.AddRolesToResponseHeader(image);
        }

        Dictionary<PublicId, int>? references = null;

        if (includeReferences == true)
        {
            references = [];

            foreach (var image in filteredAndSortedImages)
            {
                var count = await imageRepository.CountNumberOfReferencingTournamentsAsync(image.Id);
                references[image.PublicId] = count;
            }
        }

        return Results.Ok(new GetImagesEndpointResponse
        {
            Images = mapper.MapCollection<ImageDto>(filteredAndSortedImages).ToArray(),
            References = references
        });
    }

    public sealed record GetImagesEndpointResponse
    {
        public required ImageDto[] Images { get; init; }

        public required Dictionary<PublicId, int>? References { get; init; }
    }
}
