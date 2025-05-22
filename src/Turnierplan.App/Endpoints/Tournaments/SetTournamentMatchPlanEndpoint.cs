using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Exceptions;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;
using Turnierplan.Dal.Converters;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class SetTournamentMatchPlanEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{id}/match-plan";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetTournamentMatchPlanEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await repository.GetByPublicIdAsync(id, ITournamentRepository.Include.GameRelevant).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.CanSessionUserAccess(tournament.Organization))
        {
            return Results.Forbid();
        }

        var matchIdToIndexMap = tournament.Matches.ToDictionary(x => x.Id, x => x.Index);

        foreach (var entry in request.Matches)
        {
            var target = tournament.Matches.SingleOrDefault(x => x.Id == entry.Id);

            if (target is null)
            {
                return Results.BadRequest($"Match with id {entry.Id} does not exist.");
            }

            matchIdToIndexMap[entry.Id] = entry.Index;

            target.Court = entry.Court;
            target.Kickoff = entry.Kickoff;

            var teamSelectorA = TeamSelectorConverter.ConvertStringToTeamSelector(entry.TeamSelectorA);
            var teamSelectorB = TeamSelectorConverter.ConvertStringToTeamSelector(entry.TeamSelectorB);

            if (target.IsGroupMatch && (teamSelectorA is not GroupDefinitionSelector || teamSelectorB is not GroupDefinitionSelector))
            {
                return Results.BadRequest("When modifying a group match, both team selectors must be of type 'groupDefinition'.");
            }

            tournament.SetMatchTeamSelectors(entry.Id, teamSelectorA, teamSelectorB);
        }

        tournament.SetMatchOrder(matchIdToIndexMap);

        try
        {
            // Compute the tournament such that the changes are not saved in case of an exception
            tournament.Compute();
        }
        catch (TurnierplanException ex)
        {
            return Results.BadRequest($"Changes to match plan result in computation failure: {ex.Message}");
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetTournamentMatchPlanEndpointRequest
    {
        public required SetTournamentMatchPlanEndpointRequestEntry[] Matches { get; init; }

        public sealed record SetTournamentMatchPlanEndpointRequestEntry(int Id, int Index, short Court, DateTime? Kickoff, string TeamSelectorA, string TeamSelectorB);
    }

    private sealed class Validator : AbstractValidator<SetTournamentMatchPlanEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Matches)
                .NotEmpty()
                .WithMessage("Changes must include at least one match entry.");

            RuleForEach(x => x.Matches)
                .ChildRules(x =>
                {
                    x.RuleFor(y => y.Court).GreaterThanOrEqualTo((short)0);
                });
        }
    }
}
