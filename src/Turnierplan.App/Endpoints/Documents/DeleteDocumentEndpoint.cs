using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Documents;

internal sealed class DeleteDocumentEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/documents/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IDocumentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var document = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (document is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(document.Tournament, Actions.GenericDelete))
        {
            return Results.Forbid();
        }

        repository.Remove(document);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }
}
