using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.InvitationLinks;

internal sealed class UpdateInvitationLinkEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Put;

    protected override string Route => "/api/planning-realms/{planningRealmId}/invitation-links/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long id,
        [FromBody] UpdateInvitationLinkEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
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

        // TODO Set properties

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record UpdateInvitationLinkEndpointRequest
    {
        public required string Name { get; init; }

        public string? Title { get; init; }

        public string? Description { get; init; }

        public required string ColorCode { get; init; }

        public DateTime? ValidUntil { get; init; }

        public string? ContactPerson { get; init; }

        public string? ContactEmail { get; init; }

        public string? ContactTelephone { get; init; }

        public string? PrimaryLogoImageId { get; init; }

        public string? SecondaryLogoImageId { get; init; }

        public required UpdateInvitationLinkEndpointRequestEntry[] Entries { get; init; }

        public record UpdateInvitationLinkEndpointRequestEntry
        {
            public required long TournamentClassId { get; init; }

            public required bool AllowNewRegistrations { get; init; }

            public int? MaxTeamsPerRegistration { get; init; }
        }
    }

    internal sealed class Validator : AbstractValidator<UpdateInvitationLinkEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
