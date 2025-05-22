using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Venue;
using Turnierplan.Dal;

namespace Turnierplan.App.Endpoints.Venues;

internal sealed class CreateVenueEndpoint : EndpointBase<VenueDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/venues";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateVenueEndpointRequest request,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IVenueRepository venueRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var organization = await organizationRepository.GetByPublicIdAsync(request.OrganizationId).ConfigureAwait(false);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.CanSessionUserAccess(organization))
        {
            return Results.Forbid();
        }

        var venue = new Venue(organization, request.Name.Trim(), string.Empty);

        await venueRepository.CreateAsync(venue).ConfigureAwait(false);
        await venueRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.Ok(mapper.Map<VenueDto>(venue));
    }

    public sealed record CreateVenueEndpointRequest
    {
        public required PublicId OrganizationId { get; init; }

        public required string Name { get; init; }
    }

    internal sealed class Validator : AbstractValidator<CreateVenueEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(ValidationConstants.Venue.MaxNameLength);
        }
    }
}
