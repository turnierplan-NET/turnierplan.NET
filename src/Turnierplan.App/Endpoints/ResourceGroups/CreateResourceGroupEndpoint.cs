using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ResourceGroups;

internal sealed class CreateResourceGroupEndpoint : EndpointBase<ResourceGroupDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/resource-planners/{resourcePlannerId}/resource-groups";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId resourcePlannerId,
        [FromBody] CreateResourceGroupEndpointRequest request,
        IResourcePlannerRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var resourcePlanner = await repository.GetByPublicIdAsync(resourcePlannerId);

        if (resourcePlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(resourcePlanner, Actions.ApplicationsWrite))
        {
            return Results.Forbid();
        }

        var resourceGroup = resourcePlanner.AddResourceGroup(request.Name, request.Description, request.Start, request.End);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(mapper.Map<ResourceGroupDto>(resourceGroup));
    }

    public sealed record CreateResourceGroupEndpointRequest
    {
        public required string Name { get; init; }

        public string? Description { get; init; }

        public DateTime? Start { get; init; }

        public DateTime? End { get; init; }
    }

    internal sealed class Validator : AbstractValidator<CreateResourceGroupEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotEmpty()
                .When(x => x.Description is not null);
        }
    }
}
