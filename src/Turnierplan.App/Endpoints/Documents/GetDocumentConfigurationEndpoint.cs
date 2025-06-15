using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;
using Turnierplan.PdfRendering;
using Turnierplan.PdfRendering.Configuration;

namespace Turnierplan.App.Endpoints.Documents;

internal abstract class GetDocumentConfigurationEndpoint : EndpointBase
{
    private static readonly JsonSerializerOptions __jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly DocumentType _documentType;
    private readonly Type _configurationType;

    protected GetDocumentConfigurationEndpoint(IDocumentTypeRegistry documentTypeRegistry, DocumentType documentType)
    {
        if (!documentTypeRegistry.TryGetDocumentConfigurationType(documentType, out var runtimeType))
        {
            throw new InvalidOperationException($"Failed to get runtime type for document type {documentType}.");
        }

        _documentType = documentType;
        _configurationType = runtimeType;
    }

    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => $"/api/documents/{{id}}/{_documentType.ToString().ToSnakeCase()}/configuration";

    protected override Delegate Handler => Handle;

    protected override void ConfigureMetadata(RouteHandlerBuilder builder)
    {
        builder.Produces(200, _configurationType);
    }

    private async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IDocumentRepository repository,
        IAccessValidator accessValidator)
    {
        var document = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (document is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(document.Tournament, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        if (document.Type != _documentType)
        {
            return Results.BadRequest("Specified document configuration does not match the type of the document.");
        }

        // The deserialization is required to populate properties that might be declared in the configuration type but
        // are not present in the database. This can occur, for example, if a turnierplan.NET update adds new properties
        // to existing document configurations.

        var deserialized = JsonSerializer.Deserialize(document.Configuration, _configurationType, __jsonSerializerOptions);

        if (deserialized is not IDocumentConfiguration result)
        {
            throw new InvalidOperationException("JSON deserialization returned object that is not assignable to IDocumentConfiguration.");
        }

        return Results.Ok(result);
    }

}

internal sealed class GetMatchPlanDocumentConfigurationEndpoint(IDocumentTypeRegistry documentTypeRegistry) : GetDocumentConfigurationEndpoint(documentTypeRegistry, DocumentType.MatchPlan);

internal sealed class GetReceiptsDocumentConfigurationEndpoint(IDocumentTypeRegistry documentTypeRegistry) : GetDocumentConfigurationEndpoint(documentTypeRegistry, DocumentType.Receipts);
