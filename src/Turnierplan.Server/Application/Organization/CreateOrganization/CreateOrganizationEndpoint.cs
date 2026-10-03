using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Database.Entities;
using Turnierplan.Server.Infrastructure.Endpoints;
using Turnierplan.Server.Infrastructure.Validation;

namespace Turnierplan.Server.Application.Organization.CreateOrganization;

internal sealed class CreateOrganizationEndpoint : EndpointBase
{
    public CreateOrganizationEndpoint()
        : base(HttpMethod.Post, "/organizations", PolicyNames.CreateOrganization)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] CreateOrganizationRequest request,
        TurnierplanContext context,
        CancellationToken cancellationToken)
    {
        var organization = new OrganizationEntity(request.Name);

        await context.Organizations.AddAsync(organization, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // TODO: accessValidator.AddRolesToResponseHeader(organization);

        return Results.Ok(new CreateOrganizationResponse
        {
            Id = organization.PublicId,
            Name = organization.Name
        });
    }
}

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
            .NotEmptyAndTrimmed();
    }
}
