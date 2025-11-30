using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ApplicationTeams;

internal sealed class CreateApplicationTeamEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications/{applicationId:int}/teams";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long applicationId,
        [FromBody] CreateApplicationTeamEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.TournamentClasses | IPlanningRealmRepository.Includes.Applications);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ApplicationsWrite))
        {
            return Results.Forbid();
        }

        var application = planningRealm.Applications.FirstOrDefault(x => x.Id == applicationId);
        var tournamentClass = planningRealm.TournamentClasses.FirstOrDefault(x => x.Id == request.TournamentClassId);

        if (application is null || tournamentClass is null)
        {
            return Results.NotFound();
        }

        application.AddTeam(tournamentClass, request.TeamName);

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record CreateApplicationTeamEndpointRequest
    {
        public required long TournamentClassId { get; init; }

        public required string TeamName { get; init; }
    }

    private sealed class Validator : AbstractValidator<CreateApplicationTeamEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.TournamentClassId)
                .GreaterThan(0);

            RuleFor(x => x.TeamName)
                .NotEmpty();
        }
    }
}
