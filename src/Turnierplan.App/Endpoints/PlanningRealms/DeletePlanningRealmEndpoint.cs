using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.PlanningRealms;

internal sealed class DeletePlanningRealmEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/planning-realm/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IPlanningRealmRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var planningRealm = await repository.GetByPublicIdAsync(id);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        repository.Remove(planningRealm);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
