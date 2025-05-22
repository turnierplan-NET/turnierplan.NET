using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Turnierplan.Core.Document;
using Turnierplan.PdfRendering.Configuration;

namespace Turnierplan.PdfRendering;

public interface IDocumentTypeRegistry
{
    bool TryGetDocumentConfigurationType(DocumentType type, [NotNullWhen(true)] out Type? configurationType);

    bool TryParseDocumentConfiguration(DocumentType type, string configuration, [NotNullWhen(true)] out IDocumentConfiguration? result);

    bool TryGetDocumentDefaultConfiguration(DocumentType type, [NotNullWhen(true)] out string? configuration);
}

internal sealed class DocumentTypeRegistry : IDocumentTypeRegistry
{
    private static readonly JsonSerializerOptions __jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly Dictionary<DocumentType, Type> _configurationTypes;

    public DocumentTypeRegistry()
    {
        var documentConfigurationType = typeof(IDocumentConfiguration);

        _configurationTypes = typeof(DocumentTypeRegistry).Assembly.GetTypes()
            .Where(x => x is { IsPublic: true, IsInterface: false } && x.IsAssignableTo(documentConfigurationType))
            .ToDictionary(x => GetDocumentType(x.Name), x => x);

        foreach (var enumType in Enum.GetValues<DocumentType>())
        {
            if (!_configurationTypes.ContainsKey(enumType))
            {
                throw new InvalidOperationException($"The detected document configuration types contain no entry for '{enumType}'.");
            }
        }
    }

    public bool TryGetDocumentConfigurationType(DocumentType type, [NotNullWhen(true)] out Type? configurationType)
    {
        return _configurationTypes.TryGetValue(type, out configurationType);
    }

    public bool TryParseDocumentConfiguration(DocumentType type, string configuration, [NotNullWhen(true)] out IDocumentConfiguration? result)
    {
        if (!TryGetDocumentConfigurationType(type, out var clrType))
        {
            result = null;
            return false;
        }

        var deserializeResult = JsonSerializer.Deserialize(configuration, clrType, __jsonSerializerOptions);

        if (deserializeResult is IDocumentConfiguration parsedDocumentConfiguration)
        {
            result = parsedDocumentConfiguration;
            return true;
        }

        result = null;
        return false;
    }

    public bool TryGetDocumentDefaultConfiguration(DocumentType type, [NotNullWhen(true)] out string? configuration)
    {
        if (!TryGetDocumentConfigurationType(type, out var clrType))
        {
            configuration = null;
            return false;
        }

        var instance = Activator.CreateInstance(clrType);

        if (instance is null)
        {
            throw new InvalidOperationException("Failed to instantiate document configuration");
        }

        configuration = JsonSerializer.Serialize(instance, clrType, __jsonSerializerOptions);
        return true;
    }

    /// <remarks>This is <c>internal</c> because this is only used in tests.</remarks>
    internal IEnumerable<DocumentType> GetAvailableDocumentTypes() => _configurationTypes.Keys;

    private static DocumentType GetDocumentType(string typeName)
    {
        const string suffix = "DocumentConfiguration";
        var shortName = typeName.EndsWith(suffix)
            ? typeName[..^suffix.Length]
            : throw new InvalidOperationException($"Invalid document configuration type name: '{typeName}'");

        return Enum.Parse<DocumentType>(shortName);
    }
}
