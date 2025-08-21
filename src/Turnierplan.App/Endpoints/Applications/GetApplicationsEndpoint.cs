using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class GetApplicationsEndpoint : EndpointBase<PaginationResultDto<ApplicationDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Include.TournamentClasses | IPlanningRealmRepository.Include.ApplicationsWithTeams).ConfigureAwait(false);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ManageApplications))
        {
            return Results.Forbid();
        }

        var queryLogic = new QueryLogic(page, pageSize);
        var result = queryLogic.Process(planningRealm, mapper);

        return Results.Ok(result);
    }

    private sealed class QueryLogic
    {
        private const int DefaultPageSize = 20;

        private readonly int _page;
        private readonly int _pageSize;

        public QueryLogic(int? page, int? pageSize)
        {
            _page = page ?? 0;
            _pageSize = pageSize ?? DefaultPageSize;
        }

        public PaginationResultDto<ApplicationDto> Process(PlanningRealm planningRealm, IMapper mapper)
        {
            var applications = planningRealm.Applications.AsEnumerable();

            // TODO: Filter

            return PaginationHelper.Process<Application, ApplicationDto>(applications, _page, _pageSize, mapper);
        }
    }
}
