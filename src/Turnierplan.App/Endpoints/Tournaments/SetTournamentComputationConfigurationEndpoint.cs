using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class SetTournamentComputationConfigurationEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{id}/computation-configuration";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetTournamentComputationConfigurationEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        tournament.ComputationConfiguration = new ComputationConfiguration
        {
            MatchWonPoints = request.Configuration.MatchWonPoints,
            MatchDrawnPoints = request.Configuration.MatchDrawnPoints,
            MatchLostPoints = request.Configuration.MatchLostPoints,
            HigherScoreLoses = request.Configuration.HigherScoreLoses,
            ComparisonModes = request.Configuration.ComparisonModes.ToList()
        };

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetTournamentComputationConfigurationEndpointRequest
    {
        public required ComputationConfigurationDto Configuration { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetTournamentComputationConfigurationEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Configuration.MatchWonPoints)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100);

            RuleFor(x => x.Configuration.MatchDrawnPoints)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100);

            RuleFor(x => x.Configuration.MatchLostPoints)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100);

            RuleFor(x => x.Configuration.ComparisonModes)
                .NotEmpty()
                .Must(x => x.Distinct().Count() == x.Length)
                .WithMessage("Comparison modes may not contain duplicate values.")
                .ForEach(x => x.IsInEnum());
        }
    }
}
