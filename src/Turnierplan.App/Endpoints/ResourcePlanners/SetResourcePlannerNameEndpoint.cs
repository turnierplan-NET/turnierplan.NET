using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ResourcePlanners;

internal sealed class SetResourcePlannerNameEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/resource-planners/{id}/name";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetResourcePlannerNameEndpointRequest request,
        IResourcePlannerRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var resourcePlanner = await repository.GetByPublicIdAsync(id);

        if (resourcePlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(resourcePlanner, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        resourcePlanner.Name = request.Name.Trim();

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetResourcePlannerNameEndpointRequest
    {
        public required string Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetResourcePlannerNameEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
