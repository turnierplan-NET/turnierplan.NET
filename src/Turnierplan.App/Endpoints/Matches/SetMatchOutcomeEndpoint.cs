using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Models.Enums;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Matches;

internal sealed class SetMatchOutcomeEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{tournamentId}/matches/{matchId:int}/outcome";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentId,
        [FromRoute] int matchId,
        [FromBody] SetMatchOutcomeEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await repository.GetByPublicIdAsync(tournamentId, ITournamentRepository.Includes.Matches);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.TournamentConduct))
        {
            return Results.Forbid();
        }

        var match = tournament.Matches.FirstOrDefault(x => x.Id == matchId);

        if (match is null)
        {
            return Results.NotFound();
        }

        if (request.State is MatchState.NotStarted)
        {
            match.ResetOutcome();
        }
        else
        {
            var isCurrentlyPlaying = request.State is MatchState.CurrentlyPlaying;
            match.SetOutcome(isCurrentlyPlaying, request.ScoreA!.Value, request.ScoreB!.Value, request.OutcomeType!.Value);
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetMatchOutcomeEndpointRequest
    {
        /// <summary>
        /// The current state of the match. If it is <see cref="MatchState.Finished"/> or
        /// <see cref="MatchState.CurrentlyPlaying"/>, then <see cref="ScoreA"/>, <see cref="ScoreB"/> and
        /// <see cref="OutcomeType"/> must not be <c>null</c>. Otherwise, all three score parameters must be <c>null</c>.
        /// </summary>
        public required MatchState State { get; init; }

        /// <summary>
        /// The score achieved by team A or <c>null</c> (see <see cref="State"/>).
        /// </summary>
        public int? ScoreA { get; init; }

        /// <summary>
        /// The score achieved by team B or <c>null</c> (see <see cref="State"/>).
        /// </summary>
        public int? ScoreB { get; init; }

        /// <summary>
        /// The outcome type that shall be used or <c>null</c> (see <see cref="State"/>).
        /// </summary>
        public MatchOutcomeType? OutcomeType { get; init; }
    }

    internal sealed class Validator : AbstractValidator<SetMatchOutcomeEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.State)
                .IsInEnum()
                .WithMessage($"{nameof(SetMatchOutcomeEndpointRequest.State)} must be one of {nameof(MatchState.NotStarted)}, {nameof(MatchState.CurrentlyPlaying)} or {nameof(MatchState.Finished)}.");

            RuleFor(x => x.ScoreA)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .When(x => x.State is MatchState.CurrentlyPlaying or MatchState.Finished)
                .WithMessage($"{nameof(SetMatchOutcomeEndpointRequest.ScoreA)} must be not null and non-negative if {nameof(SetMatchOutcomeEndpointRequest.State)} is {nameof(MatchState.CurrentlyPlaying)} or {nameof(MatchState.Finished)}.");

            RuleFor(x => x.ScoreA)
                .Null()
                .When(x => x.State is MatchState.NotStarted)
                .WithMessage($"{nameof(SetMatchOutcomeEndpointRequest.ScoreA)} must be null if {nameof(SetMatchOutcomeEndpointRequest.State)} is {nameof(MatchState.NotStarted)}.");

            RuleFor(x => x.ScoreB)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .When(x => x.State is MatchState.CurrentlyPlaying or MatchState.Finished)
                .WithMessage($"{nameof(SetMatchOutcomeEndpointRequest.ScoreB)} must be not null and non-negative if {nameof(SetMatchOutcomeEndpointRequest.State)} is {nameof(MatchState.CurrentlyPlaying)} or {nameof(MatchState.Finished)}.");

            RuleFor(x => x.ScoreB)
                .Null()
                .When(x => x.State is MatchState.NotStarted)
                .WithMessage($"{nameof(SetMatchOutcomeEndpointRequest.ScoreB)} must be null if {nameof(SetMatchOutcomeEndpointRequest.State)} is {nameof(MatchState.NotStarted)}.");

            RuleFor(x => x.OutcomeType)
                .NotNull()
                .WithMessage($"{nameof(SetMatchOutcomeEndpointRequest.OutcomeType)} must be not null if {nameof(SetMatchOutcomeEndpointRequest.State)} is {nameof(MatchState.CurrentlyPlaying)} or {nameof(MatchState.Finished)}.")
                .IsInEnum()
                .When(x => x.State is MatchState.CurrentlyPlaying or MatchState.Finished);

            RuleFor(x => x.OutcomeType)
                .Null()
                .When(x => x.State is MatchState.NotStarted)
                .WithMessage($"{nameof(SetMatchOutcomeEndpointRequest.OutcomeType)} must be null if {nameof(SetMatchOutcomeEndpointRequest.State)} is {nameof(MatchState.NotStarted)}.");
        }
    }
}
