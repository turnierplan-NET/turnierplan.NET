using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.Core.Organization;
using Turnierplan.Core.User;
using Turnierplan.Dal;

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

        var organization = new Organization(user, request.Name.Trim());

        await organizationRepository.CreateAsync(organization).ConfigureAwait(false);
        await organizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
                .NotEmpty()
                .MaximumLength(ValidationConstants.Organization.MaxNameLength);
        }
    }
}
