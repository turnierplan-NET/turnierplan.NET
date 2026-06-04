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

    protected override string Route => "/api/tournament-planners/{tournamentPlannerId}/applications/{applicationId:int}/teams";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentPlannerId,
        [FromRoute] long applicationId,
        [FromBody] CreateApplicationTeamEndpointRequest request,
        ITournamentPlannerRepository tournamentPlannerRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournamentPlanner = await tournamentPlannerRepository.GetByPublicIdAsync(tournamentPlannerId, ITournamentPlannerRepository.Includes.TournamentClasses | ITournamentPlannerRepository.Includes.Applications);

        if (tournamentPlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournamentPlanner, Actions.ApplicationsWrite))
        {
            return Results.Forbid();
        }

        var application = tournamentPlanner.Applications.FirstOrDefault(x => x.Id == applicationId);
        var tournamentClass = tournamentPlanner.TournamentClasses.FirstOrDefault(x => x.Id == request.TournamentClassId);

        if (application is null || tournamentClass is null)
        {
            return Results.NotFound();
        }

        application.AddTeam(tournamentClass, request.TeamName);

        await tournamentPlannerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

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
