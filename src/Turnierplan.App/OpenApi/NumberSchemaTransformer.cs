using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Turnierplan.App.OpenApi;

/// <summary>
/// By default, the OpenAPI generator generates the following schema for numeric properties (int, double, etc.):
///
/// <code>
/// "propertyXYZ": {
///   "pattern": "^-?(?:0|[1-9]\\d*)$",
///   "type": [
///     "integer",
///     "string"
///   ],
///   "format": "int32"
/// }
/// </code>
///
/// Because of this, the ng-openapi-gen package (correctly) generates the following TypeScript code:
///
/// <code>
/// propertyXYZ: (number | string);
/// </code>
///
/// This is unfortunate because the <c>string</c> type causes type-issues when doing mathematical operations on the
/// property, for example. By removing the <see cref="JsonSchemaType.String"/> flag from the <c>type</c>, we can
/// modify the OpenAPI document such that the resulting TypeScript code does not include the <c>string</c> type in
/// the generated type alias.
/// </summary>
internal sealed class NumberSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (schema.Type.HasValue && (schema.Type.Value.HasFlag(JsonSchemaType.Number) || schema.Type.Value.HasFlag(JsonSchemaType.Integer)))
        {
            schema.Type &= ~JsonSchemaType.String;
        }

        return Task.CompletedTask;
    }
}
