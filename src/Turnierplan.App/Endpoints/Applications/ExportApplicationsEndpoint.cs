using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.OpenApi;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class ExportApplicationsEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications-export";

    protected override Delegate Handler => Handle;

    protected override void ConfigureMetadata(RouteHandlerBuilder builder)
    {
        builder.ProducesCsv();
    }

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromQuery] bool includeApplicationTeams,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator)
    {
        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.TournamentClasses | IPlanningRealmRepository.Includes.ApplicationsWithTeamsAndTournamentLinks);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ApplicationsRead))
        {
            return Results.Forbid();
        }

        return Results.Text("csvcontent");
    }
}
