using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ApplicationTeams;

internal sealed class DeleteApplicationTeamEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/tournament-planners/{tournamentPlannerId}/applications/{applicationId:int}/teams/{applicationTeamId:int}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentPlannerId,
        [FromRoute] long applicationId,
        [FromRoute] long applicationTeamId,
        ITournamentPlannerRepository tournamentPlannerRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var tournamentPlanner = await tournamentPlannerRepository.GetByPublicIdAsync(tournamentPlannerId, ITournamentPlannerRepository.Includes.ApplicationsWithTeams);

        if (tournamentPlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournamentPlanner, Actions.ApplicationsWrite))
        {
            return Results.Forbid();
        }

        var application = tournamentPlanner.Applications.FirstOrDefault(x => x.Id == applicationId);

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

        await tournamentPlannerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
