using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Turnierplan.App.OpenApi;

/// <summary>
/// By default, the ng-openapi-gen package generates HTTP client code for endpoints which <c>application/pdf</c>
/// content type in such a way that the caller cannot retrieve the <c>Blob</c> returned by the server. This can be
/// fixed by modifying the <c>format</c> property and setting it to <c>binary</c>.
/// </summary>
internal sealed class ResponseOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var csvResponseType = typeof(ICsvResponse);
        var pdfResponseType = typeof(IPdfResponse);

        if (operation.Responses is null)
        {
            return Task.CompletedTask;
        }

        if (context.Description.SupportedResponseTypes.Any(x => x.Type == csvResponseType))
        {
            operation.Responses["200"] = new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["text/csv"] = new()
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                }
            };
        }

        if (context.Description.SupportedResponseTypes.Any(x => x.Type == pdfResponseType))
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

    public interface ICsvResponse;

    public interface IPdfResponse;
}

internal static class ResponseOperationTransformerExtensions
{
    public static void ProducesCsv(this RouteHandlerBuilder builder)
    {
        builder.Produces<ResponseOperationTransformer.ICsvResponse>();
    }

    public static void ProducesPdf(this RouteHandlerBuilder builder)
    {
        builder.Produces<ResponseOperationTransformer.IPdfResponse>();
    }
}
