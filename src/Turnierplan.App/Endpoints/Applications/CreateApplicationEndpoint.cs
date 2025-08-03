using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class CreateApplicationEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/planning-realms/{id}/applications";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] CreateApplicationEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(id, IPlanningRealmRepository.Include.TournamentClasses).ConfigureAwait(false);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        // TODO: Add application based on request body

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record CreateApplicationEndpointRequest
    {
        // TODO: Add required properties
    }

    internal sealed class Validator : AbstractValidator<CreateApplicationEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
        }
    }
}
