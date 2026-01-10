using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;
using Turnierplan.PdfRendering;

namespace Turnierplan.App.Endpoints.Documents;

internal sealed partial class CreateDocumentEndpoint : EndpointBase<DocumentDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/documents";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateDocumentEndpointRequest request,
        IDocumentTypeRegistry documentTypeRegistry,
        ITournamentRepository tournamentRepository,
        IDocumentRepository documentRepository,
        IAccessValidator accessValidator,
        IMapper mapper,
        ILogger<CreateDocumentEndpoint> logger,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        if (!documentTypeRegistry.TryGetDocumentConfigurationType(request.Type, out _))
        {
            return Results.BadRequest("The specified document type does not exist.");
        }

        var tournament = await tournamentRepository.GetByPublicIdAsync(request.TournamentId);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        if (!documentTypeRegistry.TryGetDocumentDefaultConfiguration(request.Type, out var configuration))
        {
            CouldNotGetDefaultDocumentConfiguration(logger, request.Type);

            return Results.InternalServerError();
        }

        var document = new Document(tournament, request.Type, request.Name.Trim(), configuration);

        await documentRepository.CreateAsync(document);
        await documentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(mapper.Map<DocumentDto>(document));
    }

    public sealed record CreateDocumentEndpointRequest
    {
        public required PublicId TournamentId { get; init; }

        public required DocumentType Type { get; init; }

        public required string Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<CreateDocumentEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }

    [LoggerMessage(LogLevel.Critical, "Could not get the default document configuration for document type {DocumentType}.", EventId = 100)]
    private static partial void CouldNotGetDefaultDocumentConfiguration(ILogger<CreateDocumentEndpoint> logger, DocumentType documentType);
}
