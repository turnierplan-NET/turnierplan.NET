using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Image;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.PlanningRealms;

internal sealed partial class UpdatePlanningRealmEndpoint : EndpointBase
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
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(id, IPlanningRealmRepository.Includes.All);

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
        var applyImageResult = await UpdateInvitationLinkImagesAsync(imageRepository, accessValidator, planningRealm, request);

        if (applyImageResult is not null)
        {
            // When applying images is not successful, the method determines the result status code
            return applyImageResult;
        }

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    private static bool TryApplyChangesToPlanningRealm(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        planningRealm.Name = request.Name.Trim();

        if (!TryDeleteNoLongerNeededTournamentClasses(planningRealm, request, out error)
            || !TryDeleteNoLongerNeededInvitationLinks(planningRealm, request, out error)
            || !TryDeleteNoLongerNeededLabels(planningRealm, request, out error)
            || !TryCreateOrUpdateTournamentClasses(planningRealm, request, out error)
            || !TryCreateOrUpdateInvitationLinks(planningRealm, request, out error)
            || !TryCreateOrUpdateLabels(planningRealm, request, out error))
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

    private static bool TryDeleteNoLongerNeededLabels(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        foreach (var label in planningRealm.Labels.ToList())
        {
            var shouldDelete = request.Labels.None(x => x.Id == label.Id);

            if (shouldDelete)
            {
                planningRealm.RemoveLabel(label);
            }
        }

        error = null;
        return true;
    }

    private static bool TryCreateOrUpdateTournamentClasses(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        foreach (var requestTournamentClass in request.TournamentClasses)
        {
            if (requestTournamentClass.Id.HasValue)
            {
                var result = planningRealm.TournamentClasses.FirstOrDefault(x => x.Id == requestTournamentClass.Id);

                if (result is null)
                {
                    error = $"There exists no tournament class with id {requestTournamentClass.Id}.";
                    return false;
                }

                result.Name = requestTournamentClass.Name.Trim();
            }
            else
            {
                planningRealm.AddTournamentClass(requestTournamentClass.Name.Trim());
            }
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
            invitationLink.IsActive = requestInvitationLink.IsActive;
            invitationLink.ValidUntil = requestInvitationLink.ValidUntil;
            invitationLink.ContactPerson = requestInvitationLink.ContactPerson;
            invitationLink.ContactEmail = requestInvitationLink.ContactEmail;
            invitationLink.ContactTelephone = requestInvitationLink.ContactTelephone;

            invitationLink.ExternalLinks = requestInvitationLink.ExternalLinks
                .Select(requestExternalLink => new InvitationLink.ExternalLink(requestExternalLink.Name, requestExternalLink.Url))
                .ToList();

            var entriesToRemove = invitationLink.Entries
                .Where(existingEntry => requestInvitationLink.Entries.None(requestEntry => requestEntry.TournamentClassId == existingEntry.Class.Id))
                .ToList();

            foreach (var entry in entriesToRemove)
            {
                invitationLink.RemoveEntry(entry);
            }

            foreach (var requestEntry in requestInvitationLink.Entries)
            {
                var entry = invitationLink.Entries.FirstOrDefault(x => x.Class.Id == requestEntry.TournamentClassId);

                if (entry is null)
                {
                    var @class = planningRealm.TournamentClasses.FirstOrDefault(x => x.Id == requestEntry.TournamentClassId);

                    if (@class is null)
                    {
                        error = $"There exists no tournament class with id {requestEntry.TournamentClassId}.";
                        return false;
                    }

                    entry = invitationLink.AddEntry(@class);
                }

                entry.MaxTeamsPerRegistration = requestEntry.MaxTeamsPerRegistration;
                entry.AllowNewRegistrations = requestEntry.AllowNewRegistrations;
            }
        }

        error = null;
        return true;
    }

    private static bool TryCreateOrUpdateLabels(PlanningRealm planningRealm, UpdatePlanningRealmEndpointRequest request, [NotNullWhen(false)] out string? error)
    {
        foreach (var requestLabel in request.Labels)
        {
            Label label;

            if (requestLabel.Id.HasValue)
            {
                var result = planningRealm.Labels.FirstOrDefault(x => x.Id == requestLabel.Id);

                if (result is null)
                {
                    error = $"There exists no label with id {requestLabel.Id}.";
                    return false;
                }

                label = result;
                label.Name = requestLabel.Name.Trim();
            }
            else
            {
                label = planningRealm.AddLabel(requestLabel.Name.Trim());
            }

            label.ColorCode = requestLabel.ColorCode;
            label.Description = requestLabel.Description.Trim();
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
            );

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
            );

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

        var requestImage = await imageRepository.GetByPublicIdAsync(requestImageId.Value);

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

        public required UpdatePlanningRealmEndpointRequestLabel[] Labels { get; init; }
    }

    public sealed record UpdatePlanningRealmEndpointRequestTournamentClass
    {
        public long? Id { get; init; }

        public required string Name { get; init; }
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

        public required bool IsActive { get; init; }

        public DateTime? ValidUntil { get; init; }

        public string? ContactPerson { get; init; }

        public string? ContactEmail { get; init; }

        public string? ContactTelephone { get; init; }

        public PublicId? PrimaryLogoId { get; init; }

        public PublicId? SecondaryLogoId { get; init; }

        public required UpdatePlanningRealmEndpointRequestInvitationLinkExternalLink[] ExternalLinks { get; init; }

        public required UpdatePlanningRealmEndpointRequestInvitationLinkEntry[] Entries { get; init; }
    }

    public sealed record UpdatePlanningRealmEndpointRequestInvitationLinkEntry
    {
        public required long TournamentClassId { get; init; }

        public required bool AllowNewRegistrations { get; init; }

        public int? MaxTeamsPerRegistration { get; init; }
    }

    public sealed record UpdatePlanningRealmEndpointRequestInvitationLinkExternalLink
    {
        public required string Name { get; init; }

        public required string Url { get; init; }
    }

    public sealed record UpdatePlanningRealmEndpointRequestLabel
    {
        public long? Id { get; init; }

        public required string Name { get; init; }

        public required string Description { get; init; }

        public required string ColorCode { get; init; }
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
                });

            RuleForEach(x => x.InvitationLinks)
                .ChildRules(link =>
                {
                    link.RuleFor(x => x.Name)
                        .NotEmpty();

                    link.RuleFor(x => x.Title)
                        .NotEmpty()
                        .When(x => x.Title is not null);

                    link.RuleFor(x => x.Description)
                        .NotEmpty()
                        .When(x => x.Description is not null);

                    link.RuleFor(x => x.ContactPerson)
                        .NotEmpty()
                        .When(x => x.ContactPerson is not null);

                    link.RuleFor(x => x.ContactEmail)
                        .NotEmpty()
                        .When(x => x.ContactEmail is not null);

                    link.RuleFor(x => x.ContactTelephone)
                        .NotEmpty()
                        .When(x => x.ContactTelephone is not null);

                    link.RuleFor(x => x.ColorCode)
                        .Length(6)
                        .Must(x => x.All(char.IsAsciiHexDigit))
                        .NotEmpty();

                    link.RuleFor(x => x.Entries)
                        .Must(entries =>
                        {
                            var unique = entries.Select(x => x.TournamentClassId).Distinct();
                            return unique.Count() == entries.Length;
                        })
                        .WithMessage("Invitation link entries must only contain unique tournament classes.");

                    link.RuleForEach(x => x.Entries)
                        .ChildRules(entry =>
                        {
                            entry.RuleFor(x => x.MaxTeamsPerRegistration)
                                .GreaterThanOrEqualTo(1)
                                .When(x => x.MaxTeamsPerRegistration.HasValue);
                        });

                    link.RuleForEach(x => x.ExternalLinks)
                        .ChildRules(externalLink =>
                        {
                            externalLink.RuleFor(x => x.Name)
                                .NotEmpty();

                            externalLink.RuleFor(x => x.Url)
                                .NotEmpty()
                                .Matches(ExternalLinkRegex());
                        });
                });

            RuleForEach(x => x.Labels)
                .ChildRules(link =>
                {
                    link.RuleFor(x => x.Name)
                        .NotEmpty();

                    link.RuleFor(x => x.Description)
                        .NotNull();

                    link.RuleFor(x => x.ColorCode)
                        .Length(6)
                        .Must(x => x.All(char.IsAsciiHexDigit))
                        .NotEmpty();
                });
        }
    }

    [GeneratedRegex(@"^https:\/\/(?:[A-Za-z0-9-]+\.)+[a-z]+(?:\/.*)?$")]
    private static partial Regex ExternalLinkRegex();
}
