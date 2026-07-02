using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class GetApplicationChangeLogEndpoint : EndpointBase<IEnumerable<ApplicationChangeLogDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/tournament-planners/{tournamentPlannerId}/applications/{applicationId:int}/changelog";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentPlannerId,
        [FromRoute] long applicationId,
        ITournamentPlannerRepository tournamentPlannerRepository,
        IAccessValidator accessValidator,
        IApplicationChangeLogRepository applicationChangeLogRepository,
        IMapper mapper)
    {
        var tournamentPlanner = await tournamentPlannerRepository.GetByPublicIdAsync(tournamentPlannerId, ITournamentPlannerRepository.Includes.Applications);

        if (tournamentPlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournamentPlanner, Actions.ApplicationsRead))
        {
            return Results.Forbid();
        }

        var application = tournamentPlanner.Applications.FirstOrDefault(x => x.Id == applicationId);

        if (application is null)
        {
            return Results.NotFound();
        }

        var changeLog = await applicationChangeLogRepository.GetByApplicationIdAsync(application.Id);

        // the oldest change log comes first in the result list
        changeLog.Sort((x, y) => Math.Sign(x.Timestamp.Ticks - y.Timestamp.Ticks));

        return Results.Ok(mapper.MapCollection<ApplicationChangeLogDto>(changeLog));
    }
}
