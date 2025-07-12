using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Organization;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.User;

namespace Turnierplan.App.Endpoints.Organizations;

internal sealed class CreateOrganizationEndpoint : EndpointBase<OrganizationDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/organizations";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateOrganizationEndpointRequest request,
        HttpContext httpContext,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var user = await userRepository.GetByIdAsync(httpContext.GetCurrentUserIdOrThrow()).ConfigureAwait(false);

        if (user is null)
        {
            return Results.Unauthorized();
        }

        var organization = new Organization(request.Name.Trim());

        organization.AddRoleAssignment(Role.Owner, user.AsPrincipal());

        await organizationRepository.CreateAsync(organization).ConfigureAwait(false);
        await organizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        accessValidator.AddRolesToResponseHeader(organization);

        return Results.Ok(mapper.Map<OrganizationDto>(organization));
    }

    public sealed record CreateOrganizationEndpointRequest
    {
        public required string Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<CreateOrganizationEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
