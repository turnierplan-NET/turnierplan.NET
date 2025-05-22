using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Turnierplan.App.OpenApi;

/// <summary>
/// By default, the ng-openapi-gen package generates the following typescript code for OpenAPI enum schemas:
///
/// <code>
/// export type MyEnum = 'A' | 'B' | 'C';
/// </code>
///
/// Instead, we want the following syntax, which can be achieved by setting the <c>type</c> property of enum schemas
/// to <c>string</c>:
///
/// <code>
/// export enum ImageType {
///   A = 'A',
///   B = 'B',
///   C = 'C'
/// }
/// </code>
/// </summary>
internal sealed class EnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (schema.Enum is not null && schema.Enum.Count > 0)
        {
            schema.Type = "string";
        }

        return Task.CompletedTask;
    }
}
