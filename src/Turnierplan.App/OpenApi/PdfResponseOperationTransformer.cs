using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Turnierplan.App.OpenApi;

/// <summary>
/// By default, the ng-openapi-gen package generates HTTP client code for endpoints which <c>application/pdf</c>
/// content type in such a way that the caller cannot retrieve the <c>Blob</c> returned by the server. This can be
/// fixed by modifying the <c>format</c> property and setting it to <c>binary</c>.
/// </summary>
internal sealed class PdfResponseOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var pdfResponseType = typeof(PdfResponse);

        if (context.Description.SupportedResponseTypes.Any(x => x.Type == pdfResponseType) && operation.Responses is not null)
        {
            operation.Responses["200"] = new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/pdf"] = new()
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Format = "binary"
                        }
                    }
                }
            };
        }

        return Task.CompletedTask;
    }

    public sealed record PdfResponse;
}

internal static class PdfResponseOperationTransformerExtensions
{
    public static void ProducesPdf(this RouteHandlerBuilder builder)
    {
        builder.Produces<PdfResponseOperationTransformer.PdfResponse>();
    }
}
