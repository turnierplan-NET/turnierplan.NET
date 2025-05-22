using System.Text.Json;
using System.Text.Json.Serialization;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Converters;

internal sealed class JsonPublicIdConverter : JsonConverter<PublicId>
{
    public override PublicId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var representation = reader.GetString() ?? throw new InvalidOperationException("Expected string for public ID.");

        try
        {
            return new PublicId(representation);
        }
        catch (Exception ex)
        {
            throw new JsonException("The string does not contain a valid public ID representation.", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, PublicId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

