using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.ResourcePlanner;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ResourcePlanners;

internal sealed class CreateResourcePlannerEndpoint : EndpointBase<ResourcePlannerDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/resource-planners";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateResourcePlannerEndpointRequest request,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IResourcePlannerRepository resourcePlannerRepository,
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

        var resourcePlanner = new ResourcePlanner(organization, request.Name.Trim());

        await resourcePlannerRepository.CreateAsync(resourcePlanner);
        await resourcePlannerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        accessValidator.AddRolesToResponseHeader(resourcePlanner);

        return Results.Ok(mapper.Map<ResourcePlannerDto>(resourcePlanner));
    }

    public sealed record CreateResourcePlannerEndpointRequest
    {
        public required PublicId OrganizationId { get; init; }

        public required string Name { get; init; }
    }

    internal sealed class Validator : AbstractValidator<CreateResourcePlannerEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
