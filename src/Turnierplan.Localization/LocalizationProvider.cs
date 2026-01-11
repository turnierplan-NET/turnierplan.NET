using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Turnierplan.Localization;

internal sealed partial class LocalizationProvider : ILocalizationProvider
{
    private readonly LocalizationProviderLogger _logger;
    private readonly Dictionary<string, ILocalization> _cache = [];

    public LocalizationProvider(ILogger<LocalizationProvider> logger)
    {
        _logger = new LocalizationProviderLogger(logger);

        LoadLanguages();
    }

    public bool TryGetLocalization(string languageCode, [NotNullWhen(true)] out ILocalization? localization)
    {
        return _cache.TryGetValue(languageCode, out localization);
    }

    /// <remarks>This is <c>internal</c> because this is only used in tests.</remarks>
    internal IEnumerable<string> GetAvailableLanguages() => _cache.Keys;

    private void LoadLanguages()
    {
        var assembly = typeof(LocalizationProvider).Assembly;

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            var match = LanguageResourceNameRegex().Match(resourceName);

            if (!match.Success)
            {
                continue;
            }

            var languageCode = match.Groups["languageCode"].Value;

            try
            {
                var stream = assembly.GetManifestResourceStream(resourceName);

                if (stream is null)
                {
                    _logger.LocalizationResourceStreamIsNull(resourceName);
                    continue;
                }

                var culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures).FirstOrDefault(x => x.TwoLetterISOLanguageName.Equals(languageCode));

                if (culture is null)
                {
                    _logger.CouldNotFindCultureInfoForLanguageCode(languageCode);
                    culture = CultureInfo.InvariantCulture;
                }

                var translations = LoadTranslations(stream);
                _cache[languageCode] = new InMemoryLocalization(culture, translations.AsReadOnly());
            }
            catch (Exception ex)
            {
                _logger.FailedToLoadTranslationsForLanguage(ex, languageCode);
            }
        }
    }

    private static Dictionary<string, string> LoadTranslations(Stream stream)
    {
        var result = new Dictionary<string, string>();

        var json = JsonSerializer.Deserialize<JsonElement>(stream);

        if (json.ValueKind is not JsonValueKind.Object)
        {
            throw new IOException("Could not deserialize json or root element is not an Object.");
        }

        ParseTranslations(json, string.Empty, result);

        return result;
    }

    private static void ParseTranslations(JsonElement json, string current, Dictionary<string,string> destination)
    {
        foreach (var element in json.EnumerateObject())
        {
            if (!PathSegmentRegex().IsMatch(element.Name))
            {
                throw new InvalidOperationException($"Invalid path segment: '{element.Name}'");
            }

            var path = current.Length == 0 ? element.Name : $"{current}.{element.Name}";

            switch (element.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    ParseTranslations(element.Value, path, destination);
                    break;
                case JsonValueKind.String:
                    destination[path] = element.Value.GetString() ?? string.Empty;
                    break;
                default:
                    throw new InvalidOperationException("Child values must be Object or String.");
            }
        }
    }

    [GeneratedRegex("^[A-Za-z_0-9-]+$")]
    private static partial Regex PathSegmentRegex();

    [GeneratedRegex(@"^Turnierplan\.Localization\.Resources\.i18n\.(?<languageCode>[a-z]{2})\.json$")]
    private static partial Regex LanguageResourceNameRegex();
}
