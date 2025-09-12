using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.ImageStorage;

namespace Turnierplan.App.Endpoints.Images;

internal sealed class UploadImageEndpoint : EndpointBase<ImageDto>
{
    private const int MinimumImageSizeInPixels = 50;
    private const int MaximumImageSizeInPixels = 3000;

    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/images";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        // Note the usage of [FromForm] instead of [FromBody]
        [FromForm] UploadImageEndpointRequest request,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IImageStorage imageStorage,
        IImageRepository imageRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
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

        if (imageData.Width < MinimumImageSizeInPixels || imageData.Height < MinimumImageSizeInPixels)
        {
            return Results.BadRequest($"Image is too small (must be at least {MinimumImageSizeInPixels}px along both sides).");
        }

        if (imageData.Width > MaximumImageSizeInPixels || imageData.Height > MaximumImageSizeInPixels)
        {
            return Results.BadRequest($"Image is too large (maximum is {MaximumImageSizeInPixels}px along each side).");
        }

        var constraints = ImageConstraints.GetImageConstraints(request.ImageType);

        if (!constraints.IsSizeValid((ushort)imageData.Width, (ushort)imageData.Height))
        {
            return Results.BadRequest($"Image dimensions do not meet the constraints: {constraints}");
        }

        imageData = ScaleBitmapToMaximumDimensions(imageData, request.ImageType);

        var memoryStream = new MemoryStream();
        var encodedData = imageData.Encode(SKEncodedImageFormat.Webp, 80); // IDEA: Make the quality configurable via app settings
        encodedData.SaveTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var image = new Image(organization, request.ImageName, request.ImageType, "webp", memoryStream.Length, (ushort)imageData.Width, (ushort)imageData.Height);

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

    private static SKBitmap ScaleBitmapToMaximumDimensions(SKBitmap imageData, ImageType imageType)
    {
        var maxWidth = imageType switch
        {
            ImageType.SquareLargeLogo => 400,
            ImageType.SponsorBanner => 1600,
            _ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
        };

        if (imageData.Width < maxWidth)
        {
            return imageData;
        }

        var scaleFactor = (float)maxWidth / imageData.Width;
        var destinationSize = new SKImageInfo(maxWidth, (int)(imageData.Height * scaleFactor));
        var scaledImage = imageData.Resize(destinationSize, SKFilterQuality.Medium);

        imageData.Dispose();

        return scaledImage;
    }

    public sealed record UploadImageEndpointRequest
    {
        /// <remarks>
        /// The mapping of multipart/form-data seems to not work with the current way that <see cref="PublicId"/>s processed
        /// in the context of other requests. Because of this, we accept a string and parse this identifier manually.
        /// </remarks>
        public required string OrganizationId { get; init; }

        public required ImageType ImageType { get; init; }

        public required IFormFile Image { get; init; }

        public required string ImageName { get; init; }
    }

    private sealed class Validator : AbstractValidator<UploadImageEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.ImageType)
                .IsInEnum();

            RuleFor(x => x.Image.Length)
                .LessThanOrEqualTo(8 * 1024 * 1024)
                .WithMessage("Image file size must be 8MB or less.");

            RuleFor(x => x.ImageName)
                .NotEmpty();
        }
    }
}
