using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;
using Turnierplan.ImageStorage;

namespace Turnierplan.App.Endpoints.Images;

internal sealed class DeleteImageEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/images/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IImageRepository repository,
        IImageStorage imageStorage,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var image = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (image is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(image, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        var imageDeletedSuccessfully = await imageStorage.DeleteImageAsync(image).ConfigureAwait(false);

        if (!imageDeletedSuccessfully)
        {
            return Results.InternalServerError("Failed to delete the image.");
        }

        repository.Remove(image);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }
}
