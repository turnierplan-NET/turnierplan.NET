using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ApplicationTeams;

internal sealed class SetApplicationTeamNameEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournament-planners/{tournamentPlannerId}/applications/{applicationId:int}/teams/{applicationTeamId:int}/name";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentPlannerId,
        [FromRoute] long applicationId,
        [FromRoute] long applicationTeamId,
        [FromBody] SetApplicationTeamNameEndpointRequest request,
        ITournamentPlannerRepository tournamentPlannerRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        // Note: We must use 'ApplicationsWithTeamsAndTournamentLinks' because if a team link exists, the tournament team must also be renamed
        var tournamentPlanner = await tournamentPlannerRepository.GetByPublicIdAsync(tournamentPlannerId, ITournamentPlannerRepository.Includes.ApplicationsWithTeamsAndTournamentLinks);

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

        applicationTeam.SetName(request.Name);

        await tournamentPlannerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

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
