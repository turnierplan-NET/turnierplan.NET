using System.Text.Json;
using System.Text.Json.Serialization;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Converters;

internal sealed class JsonPublicIdConverter : JsonConverter<PublicId>
{
    public override PublicId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var representation = reader.GetString() ?? throw new InvalidOperationException("Expected string for public ID.");

        if (PublicId.TryParse(representation, out var publicId))
        {
            return publicId;
        }

        throw new JsonException("The string does not contain a valid public ID representation.");
    }

    public override void Write(Utf8JsonWriter writer, PublicId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    /// <remarks>
    /// Required for serialization of properties with the type <c>Dictionary&lt;PublicId, T&gt;</c>
    /// </remarks>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, PublicId value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString());
    }
}

