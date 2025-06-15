using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.ApiKeys;

internal sealed class DeleteApiKeyEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/api-keys/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IApiKeyRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var apiKey = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (apiKey is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(apiKey.Organization, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        repository.Remove(apiKey);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }
}
