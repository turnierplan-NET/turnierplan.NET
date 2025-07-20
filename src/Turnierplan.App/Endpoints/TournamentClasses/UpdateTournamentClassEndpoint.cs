using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.TournamentClasses;

internal sealed class UpdateTournamentClassEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Put;

    protected override string Route => "/api/planning-realms/{planningRealmId}/tournament-classes/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long id,
        [FromBody] UpdateTournamentClassEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Include.TournamentClasses).ConfigureAwait(false);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        var tournamentClass = planningRealm.TournamentClasses.FirstOrDefault(x => x.Id == id);

        if (tournamentClass is null)
        {
            return Results.NotFound();
        }

        tournamentClass.Name = request.Name;
        tournamentClass.MaxTeamCount = request.MaxTeamCount;

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record UpdateTournamentClassEndpointRequest
    {
        public required string Name { get; init; }

        public int? MaxTeamCount { get; init; }
    }

    internal sealed class Validator : AbstractValidator<UpdateTournamentClassEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.MaxTeamCount)
                .GreaterThanOrEqualTo(2)
                .When(x => x.MaxTeamCount.HasValue);
        }
    }
}
