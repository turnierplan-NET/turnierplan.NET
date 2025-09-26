using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.PlanningRealms;

internal sealed class GetPlanningRealmEndpoint : EndpointBase<PlanningRealmDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/planning-realms/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IPlanningRealmRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        // Note: We muse use 'ApplicationsWithTeams' in order to have access to the number of applications/teams per invitation link/tournament class
        var planningRealm = await repository.GetByPublicIdAsync(id, IPlanningRealmRepository.Includes.All | IPlanningRealmRepository.Includes.ApplicationsWithTeams);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        accessValidator.AddRolesToResponseHeader(planningRealm);

        return Results.Ok(mapper.Map<PlanningRealmDto>(planningRealm));
    }
}
