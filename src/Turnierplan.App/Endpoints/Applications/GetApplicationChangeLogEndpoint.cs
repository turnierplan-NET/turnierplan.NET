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

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications/{applicationId:int}/changelog";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long applicationId,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        IApplicationChangeLogRepository applicationChangeLogRepository,
        IMapper mapper)
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

        var changeLog = await applicationChangeLogRepository.GetByApplicationIdAsync(application.Id);

        // the oldest change log comes first in the result list
        changeLog.Sort((x, y) => Math.Sign(x.Timestamp.Ticks - y.Timestamp.Ticks));

        return Results.Ok(mapper.MapCollection<ApplicationChangeLogDto>(changeLog));
    }
}
