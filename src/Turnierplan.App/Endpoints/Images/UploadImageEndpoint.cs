using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SkiaSharp;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Options;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;
using Turnierplan.ImageStorage;

namespace Turnierplan.App.Endpoints.Images;

internal sealed class UploadImageEndpoint : EndpointBase<ImageDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/images";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromForm] UploadImageEndpointRequest request, // Note that [FromForm] is used instead of [FromBody]
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IImageStorage imageStorage,
        IImageRepository imageRepository,
        IOptions<TurnierplanOptions> turnierplanOptions,
        ILogger<UploadImageEndpoint> logger,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        var maxImageSize = turnierplanOptions.Value.ImageMaxSize;
        var imageQuality = turnierplanOptions.Value.ImageQuality;

        if (maxImageSize is null or <= 0)
        {
            logger.LogError($"The '{nameof(TurnierplanOptions.ImageMaxSize)}' value in '{nameof(TurnierplanOptions)}' must be specified and greater than zero.");
            return Results.InternalServerError();
        }

        if (imageQuality is null or <= 0 or > 100)
        {
            logger.LogError($"The '{nameof(TurnierplanOptions.ImageQuality)}' value in '{nameof(TurnierplanOptions)}' must be specified, greater than zero, and less than or equal to 100.");
            return Results.InternalServerError();
        }

        if (!new Validator(maxImageSize.Value).ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        if (!PublicId.TryParse(request.OrganizationId, out var organizationId))
        {
            return Results.BadRequest("Invalid organization ID provided.");
        }

        var organization = await organizationRepository.GetByPublicIdAsync(organizationId.Value);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        SKBitmap imageData;

        await using (var stream = request.Image.OpenReadStream())
        {
            imageData = SKBitmap.Decode(stream);
        }

        if (imageData is null)
        {
            return Results.BadRequest("Could not process image.");
        }

        var memoryStream = new MemoryStream();
        var encodedData = imageData.Encode(SKEncodedImageFormat.Webp, imageQuality.Value);
        encodedData.SaveTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var image = new Image(organization, request.ImageName, "webp", memoryStream.Length, (ushort)imageData.Width, (ushort)imageData.Height);

        // Dispose here because Image() ctor accesses width and height of imageData
        imageData.Dispose();

        var saveImageResult = await imageStorage.SaveImageAsync(image, memoryStream);

        if (!saveImageResult)
        {
            return Results.InternalServerError("Could not save image internally.");
        }

        await imageRepository.CreateAsync(image);
        await imageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        accessValidator.AddRolesToResponseHeader(image);

        return Results.Ok(mapper.Map<ImageDto>(image));
    }

    public sealed record UploadImageEndpointRequest
    {
        /// <remarks>
        /// The mapping of multipart/form-data seems to not work with the current way that <see cref="PublicId"/>s processed
        /// in the context of other requests. Because of this, we accept a string and parse this identifier manually.
        /// </remarks>
        public required string OrganizationId { get; init; }

        public required IFormFile Image { get; init; }

        public required string ImageName { get; init; }
    }

    private sealed class Validator : AbstractValidator<UploadImageEndpointRequest>
    {
        public Validator(int maxSize)
        {
            RuleFor(x => x.Image.Length)
                .LessThanOrEqualTo(maxSize)
                .WithMessage($"The maximum allowed image size is {maxSize} bytes.");

            RuleFor(x => x.ImageName)
                .NotEmpty();
        }
    }
}
