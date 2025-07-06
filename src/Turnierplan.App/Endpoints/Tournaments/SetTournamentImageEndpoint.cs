using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class SetTournamentImageEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{id}/image";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetTournamentImageEndpointRequest request,
        ITournamentRepository tournamentRepository,
        IImageRepository imageRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await tournamentRepository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        Image? image = null;

        if (request.ImageId.HasValue)
        {
            image = await imageRepository.GetByPublicIdAsync(request.ImageId.Value);

            if (image is null)
            {
                return Results.NotFound();
            }

            if (!accessValidator.IsActionAllowed(image, Actions.GenericRead))
            {
                return Results.Forbid();
            }

            if (tournament.Organization != image.Organization)
            {
                return Results.BadRequest("Image must belong to the same organization as the tournament.");
            }
        }

        switch (request.Target)
        {
            case SetTournamentImageEndpointRequest.TargetImage.OrganizerLogo:
                tournament.SetOrganizerLogo(image);
                break;
            case SetTournamentImageEndpointRequest.TargetImage.SponsorLogo:
                tournament.SetSponsorLogo(image);
                break;
            case SetTournamentImageEndpointRequest.TargetImage.SponsorBanner:
                tournament.SetSponsorBanner(image);
                break;
            default:
                // This cannot happen because it's prevented by the validator
                throw new InvalidOperationException();
        }

        await tournamentRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetTournamentImageEndpointRequest
    {
        public PublicId? ImageId { get; init; }

        public TargetImage Target { get; init; }

        public enum TargetImage
        {
            OrganizerLogo,
            SponsorLogo,
            SponsorBanner
        }
    }

    private sealed class Validator : AbstractValidator<SetTournamentImageEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Target).IsInEnum();
        }
    }
}
