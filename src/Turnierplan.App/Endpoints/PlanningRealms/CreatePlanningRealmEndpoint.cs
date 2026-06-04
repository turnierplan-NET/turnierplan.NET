using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.TournamentPlanner;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.PlanningRealms;

internal sealed class CreatePlanningRealmEndpoint : EndpointBase<PlanningRealmDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/planning-realms";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreatePlanningRealmEndpointRequest request,
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

        var planningRealm = new TournamentPlanner(organization, request.Name.Trim());

        await tournamentPlannerRepository.CreateAsync(planningRealm);
        await tournamentPlannerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        accessValidator.AddRolesToResponseHeader(planningRealm);

        return Results.Ok(mapper.Map<PlanningRealmDto>(planningRealm));
    }

    public sealed record CreatePlanningRealmEndpointRequest
    {
        public required PublicId OrganizationId { get; init; }

        public required string Name { get; init; }
    }

    internal sealed class Validator : AbstractValidator<CreatePlanningRealmEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
