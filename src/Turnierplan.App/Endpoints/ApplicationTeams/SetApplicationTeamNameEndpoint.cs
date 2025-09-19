using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.ApplicationTeams;

internal sealed class SetApplicationTeamNameEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications/{applicationId:int}/teams/{applicationTeamId:int}/name";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long applicationId,
        [FromRoute] long applicationTeamId,
        [FromBody] SetApplicationTeamNameEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.ApplicationsWithTeamsAndTournamentLinks);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ManageApplications))
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

        applicationTeam.SetName(request.Name);

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetApplicationTeamNameEndpointRequest
    {
        public required string Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetApplicationTeamNameEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
