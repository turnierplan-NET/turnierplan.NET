using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

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

        if (context.Description.SupportedResponseTypes.Any(x => x.Type == pdfResponseType))
        {
            operation.Responses["200"] = new OpenApiResponse
            {
                Content =
                {
                    ["application/pdf"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    }
                }
            };
        }

        return Task.CompletedTask;
    }

    public sealed class PdfResponse;
}

internal static class PdfResponseOperationTransformerExtensions
{
    public static RouteHandlerBuilder ProducesPdf(this RouteHandlerBuilder builder)
    {
        return builder.Produces<PdfResponseOperationTransformer.PdfResponse>();
    }
}
