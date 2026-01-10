using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.OpenApi;
using Turnierplan.App.Security;
using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Extensions;
using Turnierplan.Dal.Repositories;
using Turnierplan.Localization;
using Turnierplan.PdfRendering;
using Turnierplan.PdfRendering.Renderer;

namespace Turnierplan.App.Endpoints.Documents;

internal sealed partial class GetDocumentPdfEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/documents/{id}/pdf";

    protected override Delegate Handler => Handle;

    protected override void ConfigureMetadata(RouteHandlerBuilder builder)
    {
        builder.ProducesPdf();
    }

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromQuery] string languageCode,
        [FromQuery] string timeZone,
        ILocalizationProvider localizationProvider,
        IDocumentRepository repository,
        IAccessValidator accessValidator,
        IDocumentTypeRegistry documentTypeRegistry,
        IEnumerable<IDocumentRenderer> documentRenderers,
        ILogger<GetDocumentPdfEndpoint> logger,
        CancellationToken cancellationToken)
    {
        if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out var timeZoneInfo))
        {
            return Results.BadRequest("Invalid time zone specified.");
        }

        if (!localizationProvider.TryGetLocalization(languageCode, out var localization))
        {
            return Results.BadRequest("Invalid language code specified.");
        }

        var document = await repository.GetByPublicIdAsync(id, true);

        if (document is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(document.Tournament, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        if (!documentTypeRegistry.TryParseDocumentConfiguration(document.Type, document.Configuration, out var configuration))
        {
            FailedToParseDocumentConfiguration(logger, document.Type);

            return Results.InternalServerError();
        }

        var renderer = documentRenderers.SingleOrDefault(x => x.DocumentConfigurationType == configuration.GetType());

        if (renderer is null)
        {
            NoRendererAvailableForDocumentType(logger, document.Type);

            return Results.InternalServerError();
        }

        using var stream = new MemoryStream();

        // Wrap the code below in a transaction such that the generation count
        // is only incremented when the document is rendered successfully.

        await using (var transaction = await repository.UnitOfWork.WrapTransactionAsync())
        {
            document.IncreaseGenerationCount();

            // SaveChanges() must be called first because ShiftToTimezone() modifies the tournament itself
            await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            document.Tournament.ShiftToTimezone(timeZoneInfo);
            document.Tournament.Compute();

            renderer.Render(document.Tournament, configuration, localization, stream);

            transaction.ShouldCommit = true;
        }

        return Results.File(stream.ToArray(), "application/pdf");
    }

    [LoggerMessage(LogLevel.Error, "Failed to parse the document configuration of document with type '{DocumentType}'.", EventId = 100)]
    private static partial void FailedToParseDocumentConfiguration(ILogger<GetDocumentPdfEndpoint> logger, DocumentType documentType);

    [LoggerMessage(LogLevel.Critical, "No document renderer available for document type '{DocumentType}'.", EventId = 101)]
    private static partial void NoRendererAvailableForDocumentType(ILogger<GetDocumentPdfEndpoint> logger, DocumentType documentType);
}
