using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class SetTournamentPresentationConfigurationEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{id}/presentation-configuration";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetTournamentPresentationConfigurationEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await repository.GetByPublicIdAsync(id);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        tournament.PresentationConfiguration = new PresentationConfiguration
        {
            Header1 = new PresentationConfiguration.HeaderLine
            {
                Content = request.Configuration.Header1.Content,
                CustomContent = request.Configuration.Header1.CustomContent?.Trim()
            },
            Header2 = new PresentationConfiguration.HeaderLine
            {
                Content = request.Configuration.Header2.Content,
                CustomContent = request.Configuration.Header2.CustomContent?.Trim()
            },
            ShowResults = request.Configuration.ShowResults,
            ShowOrganizerLogo = request.Configuration.ShowOrganizerLogo,
            ShowSponsorLogo = request.Configuration.ShowSponsorLogo
        };

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetTournamentPresentationConfigurationEndpointRequest
    {
        public required PresentationConfigurationDto Configuration { get; init; }
    }

    internal sealed class Validator : AbstractValidator<SetTournamentPresentationConfigurationEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Configuration.Header1.Content)
                .IsInEnum();

            RuleFor(x => x.Configuration.Header1.CustomContent)
                .NotEmpty()
                .When(x => x.Configuration.Header1.Content is PresentationConfiguration.HeaderLineContent.CustomValue)
                .WithMessage($"Custom content of header 1 must not be empty if content is '{nameof(PresentationConfiguration.HeaderLineContent.CustomValue)}'.");

            RuleFor(x => x.Configuration.Header2.Content)
                .IsInEnum();

            RuleFor(x => x.Configuration.Header2.CustomContent)
                .NotEmpty()
                .When(x => x.Configuration.Header2.Content is PresentationConfiguration.HeaderLineContent.CustomValue)
                .WithMessage($"Custom content of header 2 must not be empty if content is '{nameof(PresentationConfiguration.HeaderLineContent.CustomValue)}'.");

            RuleFor(x => x.Configuration.ShowResults)
                .IsInEnum();
        }
    }
}

