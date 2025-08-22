using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Organizations;

internal sealed class SetOrganizationNameEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/organizations/{id}/name";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetOrganizationNameEndpointRequest request,
        IOrganizationRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var organization = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.PrivilegedWrite))
        {
            return Results.Forbid();
        }

        organization.Name = request.Name.Trim();

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetOrganizationNameEndpointRequest
    {
        public required string Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetOrganizationNameEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
