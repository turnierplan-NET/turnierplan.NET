using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal;

namespace Turnierplan.App.Endpoints.ApiKeys;

internal sealed class CreateApiKeyEndpoint : EndpointBase<ApiKeyDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/api-keys";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateApiKeyEndpointRequest request,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IPasswordHasher<ApiKey> secretHasher,
        IApiKeyRepository apiKeyRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var organization = await organizationRepository.GetByPublicIdAsync(request.OrganizationId).ConfigureAwait(false);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        var expiryDate = DateTime.UtcNow.AddDays(request.Validity);
        var apiKey = new ApiKey(organization, request.Name.Trim(), request.Description.Trim(), expiryDate);

        apiKey.AssignNewSecret(plainText => secretHasher.HashPassword(apiKey, plainText), out var secret);

        await apiKeyRepository.CreateAsync(apiKey).ConfigureAwait(false);
        await apiKeyRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.Ok(mapper.Map<ApiKeyDto>(apiKey) with { Secret = secret });
    }

    public sealed record CreateApiKeyEndpointRequest
    {
        public required PublicId OrganizationId { get; init; }

        public required string Name { get; init; }

        public required string Description { get; init; }

        public required int Validity { get; init; }
    }

    private sealed class Validator : AbstractValidator<CreateApiKeyEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(ValidationConstants.ApiKey.MaxNameLength);

            RuleFor(x => x.Description)
                .MaximumLength(ValidationConstants.ApiKey.MaxDescriptionLength);

            RuleFor(x => x.Validity)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(180);
        }
    }
}
