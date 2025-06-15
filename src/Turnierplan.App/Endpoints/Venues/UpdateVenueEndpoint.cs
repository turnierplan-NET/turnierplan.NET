using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Venue;
using Turnierplan.Dal;

namespace Turnierplan.App.Endpoints.Venues;

internal sealed partial class UpdateVenueEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Put;

    protected override string Route => "/api/venues/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] UpdateVenueEndpointRequest request,
        IVenueRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var venue = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (venue is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(venue, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        venue.Name = request.Name;
        venue.Description = request.Description;
        venue.AddressDetails = [..request.AddressDetails];
        venue.ExternalLinks = [..request.ExternalLinks];

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record UpdateVenueEndpointRequest
    {
        public required string Name { get; init; }

        public required string Description { get; init; }

        public required string[] AddressDetails { get; init; }

        public required string[] ExternalLinks { get; init; }
    }

    internal sealed partial class Validator : AbstractValidator<UpdateVenueEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(ValidationConstants.Venue.MaxNameLength);

            RuleFor(x => x.Description)
                .MaximumLength(ValidationConstants.Venue.MaxDescriptionLength);

            RuleFor(x => x.AddressDetails.Length)
                .InclusiveBetween(0, ValidationConstants.Venue.MaxAddressDetailCount);

            RuleForEach(x => x.AddressDetails)
                .MaximumLength(ValidationConstants.Venue.MaxAddressDetailLength);

            RuleFor(x => x.ExternalLinks.Length)
                .InclusiveBetween(0, ValidationConstants.Venue.MaxExternalLinkCount);

            RuleForEach(x => x.ExternalLinks)
                .MaximumLength(ValidationConstants.Venue.MaxExternalLinkLength);
        }
    }
}
