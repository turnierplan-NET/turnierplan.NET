using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Applications;

internal abstract class PatchApplicationEndpointBase<TRequest> : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => $"/api/planning-realms/{{planningRealmId}}/applications/{{applicationId:int}}/{RouteSuffix}";

    protected override Delegate Handler => Handle;

    protected abstract string RouteSuffix { get; }

    /// <remarks>
    /// Named <c>RequestValidator</c> instead of <c>Validator</c> to avoid errors in subclasses when they declare a nested <c>Validator</c> class.
    /// </remarks>
    protected abstract IValidator<TRequest> RequestValidator { get; }

    protected abstract void UpdateApplication(Application application, TRequest request);

    private async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long applicationId,
        [FromBody] TRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!RequestValidator.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.Applications);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ApplicationsWrite))
        {
            return Results.Forbid();
        }

        var application = planningRealm.Applications.FirstOrDefault(x => x.Id == applicationId);

        if (application is null)
        {
            return Results.NotFound();
        }

        UpdateApplication(application, request);

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
