using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ApiKeys;

internal sealed class SetApiKeyExpiryDateEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/api-keys/{id}/expiry-date";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetApiKeyExpiryDateRequest request,
        IApiKeyRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var apiKey = await repository.GetByPublicIdAsync(id);

        if (apiKey is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(apiKey, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        apiKey.ExpiryDate = DateTime.UtcNow.AddDays(request.Validity);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetApiKeyExpiryDateRequest
    {
        public required int Validity { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetApiKeyExpiryDateRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Validity)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(365);
        }
    }
}
