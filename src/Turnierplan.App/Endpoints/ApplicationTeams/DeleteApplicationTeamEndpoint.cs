using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ApplicationTeams;

internal sealed class DeleteApplicationTeamEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications/{applicationId:int}/teams/{applicationTeamId:int}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long applicationId,
        [FromRoute] long applicationTeamId,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.ApplicationsWithTeams);

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

        var applicationTeam = application.Teams.FirstOrDefault(x => x.Id == applicationTeamId);

        if (applicationTeam is null)
        {
            return Results.NotFound();
        }

        if (applicationTeam.TeamLink is not null)
        {
            return Results.BadRequest("Cannot delete team while the team link is not null.");
        }

        application.RemoveTeam(applicationTeam);

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
