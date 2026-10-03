using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Organization.CreateOrganization;

internal sealed class CreateOrganizationEndpoint : EndpointBase
{
    public CreateOrganizationEndpoint()
        : base(HttpMethod.Post, "/organizations", PolicyNames.CreateOrganization)
    {
    }

    public static Task<IResult> HandleAsync([FromBody] CreateOrganizationRequest request)
    {
        return Task.FromResult(Results.Ok(request.Name));
    }
}

// [NoValidation]
internal sealed record CreateOrganizationRequest
{
    public required string Name { get; init; }
}

internal sealed record CreateOrganizationResponse
{
    public required PublicId Id { get; init; }

    // public required string RbacScopeId { get; init; } // TODO: Required?

    public required string Name { get; init; }
}

internal sealed class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
