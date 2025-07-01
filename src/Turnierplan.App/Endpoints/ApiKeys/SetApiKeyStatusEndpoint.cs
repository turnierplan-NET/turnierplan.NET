using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.ApiKeys;

internal sealed class SetApiKeyStatusEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/api-keys/{id}/status";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetApiKeyStatusRequest request,
        IApiKeyRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var apiKey = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (apiKey is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(apiKey, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        if (apiKey.IsExpired)
        {
            return Results.BadRequest("API key is already expired.");
        }

        if (apiKey.IsActive == request.IsActive)
        {
            return Results.NoContent();
        }

        apiKey.IsActive = request.IsActive;

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetApiKeyStatusRequest
    {
        public required bool IsActive { get; init; }
    }
}
