using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.TournamentPlanner;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.TournamentPlanners;

internal sealed class CreateTournamentPlannerEndpoint : EndpointBase<TournamentPlannerDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/tournament-planners";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateTournamentPlannerEndpointRequest request,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        ITournamentPlannerRepository tournamentPlannerRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var organization = await organizationRepository.GetByPublicIdAsync(request.OrganizationId);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        var tournamentPlanner = new TournamentPlanner(organization, request.Name.Trim());

        await tournamentPlannerRepository.CreateAsync(tournamentPlanner);
        await tournamentPlannerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        accessValidator.AddRolesToResponseHeader(tournamentPlanner);

        return Results.Ok(mapper.Map<TournamentPlannerDto>(tournamentPlanner));
    }

    public sealed record CreateTournamentPlannerEndpointRequest
    {
        public required PublicId OrganizationId { get; init; }

        public required string Name { get; init; }
    }

    internal sealed class Validator : AbstractValidator<CreateTournamentPlannerEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
