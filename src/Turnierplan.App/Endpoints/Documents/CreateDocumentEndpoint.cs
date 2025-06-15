using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal;
using Turnierplan.PdfRendering;

namespace Turnierplan.App.Endpoints.Documents;

internal sealed class CreateDocumentEndpoint : EndpointBase<DocumentDto>
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

        var tournament = await tournamentRepository.GetByPublicIdAsync(request.TournamentId).ConfigureAwait(false);

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
            logger.LogCritical("Could not get the default document configuration for document type {DocumentType}.", request.Type);

            return Results.InternalServerError();
        }

        var document = new Document(tournament, request.Type, request.Name.Trim(), configuration);

        await documentRepository.CreateAsync(document).ConfigureAwait(false);
        await documentRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.Ok(mapper.Map<DocumentDto>(document));
    }

    public sealed record CreateDocumentEndpointRequest
    {
        public required PublicId TournamentId { get; init; }

        public required DocumentType Type { get; init; }

        [MaxLength(ValidationConstants.Document.MaxNameLength)]
        public required string Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<CreateDocumentEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(ValidationConstants.Tournament.MaxNameLength);
        }
    }
}
