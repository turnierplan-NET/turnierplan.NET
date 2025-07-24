using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.InvitationLinks;

internal sealed class SetInvitationLinkImageEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/planning-realms/{planningRealmId}/invitation-links/{id}/image";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long id,
        [FromBody] SetInvitationLinkImageEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IImageRepository imageRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Include.InvitationLinks).ConfigureAwait(false);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        var invitationLink = planningRealm.InvitationLinks.FirstOrDefault(x => x.Id == id);

        if (invitationLink is null)
        {
            return Results.NotFound();
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

            if (planningRealm.Organization != image.Organization)
            {
                return Results.BadRequest("Image must belong to the same organization as the planning realm.");
            }
        }

        switch (request.Target)
        {
            case SetInvitationLinkImageEndpointRequest.SetInvitationLinkImageEndpointRequestTarget.PrimaryLogo:
                invitationLink.SetPrimaryLogo(image);
                break;
            case SetInvitationLinkImageEndpointRequest.SetInvitationLinkImageEndpointRequestTarget.SecondaryLogo:
                invitationLink.SetSecondaryLogo(image);
                break;
            default:
                // This cannot happen because it's prevented by the validator
                throw new InvalidOperationException();
        }

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetInvitationLinkImageEndpointRequest
    {
        public PublicId? ImageId { get; init; }

        public SetInvitationLinkImageEndpointRequestTarget Target { get; init; }

        public enum SetInvitationLinkImageEndpointRequestTarget
        {
            PrimaryLogo,
            SecondaryLogo
        }
    }

    private sealed class Validator : AbstractValidator<SetInvitationLinkImageEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Target).IsInEnum();
        }
    }
}
