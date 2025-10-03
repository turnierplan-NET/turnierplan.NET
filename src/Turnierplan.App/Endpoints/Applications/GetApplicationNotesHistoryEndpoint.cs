using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class GetApplicationNotesHistoryEndpoint : EndpointBase<GetApplicationNotesHistoryEndpoint.GetApplicationNotesHistoryEndpointResponse>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications/{applicationId:int}/notes-history";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long applicationId,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.Applications);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ApplicationsRead))
        {
            return Results.Forbid();
        }

        var application = planningRealm.Applications.FirstOrDefault(x => x.Id == applicationId);

        if (application is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new GetApplicationNotesHistoryEndpointResponse
        {
            CurrentValue = application.Notes,
            PreviousValues = application.NotesHistory.ToArray()
        });
    }

    public sealed record GetApplicationNotesHistoryEndpointResponse
    {
        public required string CurrentValue { get; init; }

        public required string[] PreviousValues { get; init; }
    }
}
