using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.PlanningRealms;

internal sealed class UpdatePlanningRealmEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Put;

    protected override string Route => "/api/planning-realms/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] UpdatePlanningRealmEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        IImageRepository imageRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(id, IPlanningRealmRepository.Include.All).ConfigureAwait(false);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        if (!TryApplyChangesToPlanningRealm(planningRealm, request, out var error))
        {
            return Results.BadRequest(error);
        }

        // The invitation links are updated separately to avoid having to pass required services along multiple
        // nested method calls, and so we can use the Try..() pattern for the other apply methods.
        var applyImageResult = await UpdateInvitationLinkImagesAsync(imageRepository, accessValidator, planningRealm, request).ConfigureAwait(false);

        if (applyImageResult is not null)
        {
            // When applying images is not successful, the method determines the result status code
            return applyImageResult;
        }

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    private static bool TryApplyChangesToPlanningRealm(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        planningRealm.Name = request.Name.Trim();

        if (!TryDeleteNoLongerNeededTournamentClasses(planningRealm, request, out error)
            || !TryDeleteNoLongerNeededInvitationLinks(planningRealm, request, out error)
            || !TryCreateOrUpdateTournamentClasses(planningRealm, request, out error)
            || !TryCreateOrUpdateInvitationLinks(planningRealm, request, out error))
        {
            return false;
        }

        error = null;
        return true;
    }

    private static bool TryDeleteNoLongerNeededTournamentClasses(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        foreach (var tournamentClass in planningRealm.TournamentClasses.ToList())
        {
            var shouldDelete = request.TournamentClasses.None(x => x.Id == tournamentClass.Id);

            if (!shouldDelete)
            {
                continue;
            }

            var numberOfAppliedTeams = planningRealm.Applications
                .SelectMany(x => x.Teams)
                .Count(x => x.Class == tournamentClass);

            if (numberOfAppliedTeams > 0)
            {
                error = $"Cannot delete tournament class with id '{tournamentClass.Id}' because there are {numberOfAppliedTeams} applications for this class.";
                return false;
            }

            planningRealm.RemoveTournamentClass(tournamentClass);
        }

        error = null;
        return true;
    }

    private static bool TryDeleteNoLongerNeededInvitationLinks(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        foreach (var invitationLink in planningRealm.InvitationLinks.ToList())
        {
            var shouldDelete = request.InvitationLinks.None(x => x.Id == invitationLink.Id);

            if (!shouldDelete)
            {
                continue;
            }

            var numberOfApplications = planningRealm.Applications.Count(x => x.SourceLink == invitationLink);

            if (numberOfApplications > 0)
            {
                error = $"Cannot delete invitation link with id '{invitationLink.Id}' because there are {numberOfApplications} applications from this link.";
                return false;
            }

            planningRealm.RemoveInvitationLink(invitationLink);
        }

        error = null;
        return true;
    }

    private static bool TryCreateOrUpdateTournamentClasses(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        foreach (var requestTournamentClass in request.TournamentClasses)
        {
            TournamentClass tournamentClass;

            if (requestTournamentClass.Id.HasValue)
            {
                var result = planningRealm.TournamentClasses.FirstOrDefault(x => x.Id == requestTournamentClass.Id);

                if (result is null)
                {
                    error = $"There exists no tournament class with id {requestTournamentClass.Id}.";
                    return false;
                }

                tournamentClass = result;
                tournamentClass.Name = requestTournamentClass.Name.Trim();
            }
            else
            {
                tournamentClass = planningRealm.AddTournamentClass(requestTournamentClass.Name.Trim());
            }

            tournamentClass.MaxTeamCount = requestTournamentClass.MaxTeamCount;
        }

        error = null;
        return true;
    }

    private static bool TryCreateOrUpdateInvitationLinks(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        foreach (var requestInvitationLink in request.InvitationLinks)
        {
            InvitationLink invitationLink;

            if (requestInvitationLink.Id.HasValue)
            {
                var result = planningRealm.InvitationLinks.FirstOrDefault(x => x.Id == requestInvitationLink.Id);

                if (result is null)
                {
                    error = $"There exists no invitation link with id {requestInvitationLink.Id}.";
                    return false;
                }

                invitationLink = result;
                invitationLink.Name = requestInvitationLink.Name.Trim();
            }
            else
            {
                invitationLink = planningRealm.AddInvitationLink(requestInvitationLink.Name.Trim());
            }

            // This reference is required for the UpdateInvitationLinkImagesAsync() method so that it can
            // find the correct invitation link entity to update the images in.
            requestInvitationLink._invitationLink = invitationLink;

            invitationLink.Title = requestInvitationLink.Title;
            invitationLink.Description = requestInvitationLink.Description;
            invitationLink.ColorCode = requestInvitationLink.ColorCode;
            invitationLink.ValidUntil = requestInvitationLink.ValidUntil;
            invitationLink.ContactPerson = requestInvitationLink.ContactPerson;
            invitationLink.ContactEmail = requestInvitationLink.ContactEmail;
            invitationLink.ContactTelephone = requestInvitationLink.ContactTelephone;
        }

        error = null;
        return true;
    }

    private static async Task<IResult?> UpdateInvitationLinkImagesAsync(
        IImageRepository imageRepository,
        IAccessValidator accessValidator,
        PlanningRealm planningRealm,
        UpdatePlanningRealmEndpointRequest request)
    {
        foreach (var requestInvitationLink in request.InvitationLinks)
        {
            var primaryImageResult = await UpdateInvitationLinkImageAsync(
                imageRepository,
                accessValidator,
                planningRealm,
                requestInvitationLink._invitationLink!.PrimaryLogo,
                requestInvitationLink.PrimaryLogoId,
                x => requestInvitationLink._invitationLink.SetPrimaryLogo(x)
            ).ConfigureAwait(false);

            if (primaryImageResult is not null)
            {
                return primaryImageResult;
            }

            var secondaryImageResult = await UpdateInvitationLinkImageAsync(
                imageRepository,
                accessValidator,
                planningRealm,
                requestInvitationLink._invitationLink!.SecondaryLogo,
                requestInvitationLink.SecondaryLogoId,
                x => requestInvitationLink._invitationLink.SetSecondaryLogo(x)
            ).ConfigureAwait(false);

            if (secondaryImageResult is not null)
            {
                return primaryImageResult;
            }
        }

        return null;
    }

    private static async Task<IResult?> UpdateInvitationLinkImageAsync(
        IImageRepository imageRepository,
        IAccessValidator accessValidator,
        PlanningRealm planningRealm,
        Image? currentImage,
        PublicId? requestImageId,
        Action<Image?> updateImage)
    {
        if (currentImage is not null && requestImageId == currentImage.PublicId)
        {
            // The "correct" image is already set -> no change required
            return null; // means no error occurred
        }

        if (!requestImageId.HasValue)
        {
            // The request image id is null -> no database lookup required

            if (currentImage is not null)
            {
                // Remove image
                updateImage(null);
            }

            return null; // means no error occurred
        }

        var requestImage = await imageRepository.GetByPublicIdAsync(requestImageId.Value).ConfigureAwait(false);

        if (requestImage is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(requestImage, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        if (planningRealm.Organization != requestImage.Organization)
        {
            return Results.BadRequest("Image must belong to the same organization as the planning realm.");
        }

        updateImage(requestImage);

        return null; // means no error occurred
    }

    public sealed record UpdatePlanningRealmEndpointRequest
    {
        public required string Name { get; init; }

        public required UpdatePlanningRealmEndpointRequestTournamentClass[] TournamentClasses { get; init; }

        public required UpdatePlanningRealmEndpointRequestInvitationLink[] InvitationLinks { get; init; }
    }

    public sealed record UpdatePlanningRealmEndpointRequestTournamentClass
    {
        public long? Id { get; init; }

        public required string Name { get; init; }

        public int? MaxTeamCount { get; init; }
    }

    public sealed record UpdatePlanningRealmEndpointRequestInvitationLink
    {
        // The TryCreateOrUpdateInvitationLinks() method will save a reference to the InvitationLink entity
        // so that the UpdateInvitationLinkImagesAsync() method can update the image afterward.
        [JsonIgnore]
        internal InvitationLink? _invitationLink;

        public long? Id { get; init; }

        public required string Name { get; init; }

        public string? Title { get; init; }

        public string? Description { get; init; }

        public required string ColorCode { get; init; }

        public DateTime? ValidUntil { get; init; }

        public string? ContactPerson { get; init; }

        public string? ContactEmail { get; init; }

        public string? ContactTelephone { get; init; }

        public PublicId? PrimaryLogoId { get; init; }

        public PublicId? SecondaryLogoId { get; init; }
    }

    internal sealed class Validator : AbstractValidator<UpdatePlanningRealmEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleForEach(x => x.TournamentClasses)
                .ChildRules(@class =>
                {
                    @class.RuleFor(x => x.Id)
                        .GreaterThanOrEqualTo(1)
                        .When(x => x.Id.HasValue);

                    @class.RuleFor(x => x.Name)
                        .NotEmpty();

                    @class.RuleFor(x => x.MaxTeamCount)
                        .GreaterThanOrEqualTo(2)
                        .When(x => x.MaxTeamCount.HasValue);
                });

            RuleForEach(x => x.InvitationLinks)
                .ChildRules(link =>
                {
                    link.RuleFor(x => x.Name)
                        .NotEmpty();

                    link.RuleFor(x => x.ColorCode)
                        .Length(6)
                        .Must(x => x.All(char.IsAsciiHexDigit))
                        .NotEmpty();
                });
        }
    }
}
