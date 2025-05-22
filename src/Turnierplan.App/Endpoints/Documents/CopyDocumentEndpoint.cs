using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal;

namespace Turnierplan.App.Endpoints.Documents;

internal sealed class CopyDocumentEndpoint : EndpointBase<DocumentDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/documents/copy";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CopyDocumentEndpointRequest request,
        ITournamentRepository tournamentRepository,
        IDocumentRepository documentRepository,
        IAccessValidator accessValidator,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        var tournament = await tournamentRepository.GetByPublicIdAsync(request.TournamentId).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.CanSessionUserAccess(tournament.Organization))
        {
            return Results.Forbid();
        }

        var sourceDocument = await documentRepository.GetByPublicIdAsync(request.SourceDocumentId).ConfigureAwait(false);

        if (sourceDocument is null)
        {
            return Results.NotFound();
        }

        if (tournament.Organization != sourceDocument.Tournament.Organization)
        {
            return Results.BadRequest("Source document must belong to the same organization as the destination tournament.");
        }

        var documentCopyName = sourceDocument.Name.Length >= ValidationConstants.Document.MaxNameLength
            ? sourceDocument.Name[..(ValidationConstants.Document.MaxNameLength - 1)] + "*"
            : $"{sourceDocument.Name}*";

        var documentCopy = new Document(tournament, sourceDocument.Type, documentCopyName, sourceDocument.Configuration);

        await documentRepository.CreateAsync(documentCopy).ConfigureAwait(false);
        await documentRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.Ok(mapper.Map<DocumentDto>(documentCopy));
    }

    public sealed record CopyDocumentEndpointRequest
    {
        public required PublicId TournamentId { get; init; }

        public required PublicId SourceDocumentId { get; init; }
    }
}
