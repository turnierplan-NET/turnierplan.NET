using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Security;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.TournamentClasses;

internal sealed class CreateTournamentClassEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/planning-realms/{id}/tournament-classes";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] CreateTournamentClassEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        planningRealm.AddTournamentClass(request.Name.Trim());

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record CreateTournamentClassEndpointRequest
    {
        public required string Name { get; init; }
    }

    internal sealed class Validator : AbstractValidator<CreateTournamentClassEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
