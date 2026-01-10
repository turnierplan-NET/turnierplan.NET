using Microsoft.Extensions.Logging;

namespace Turnierplan.Localization;

internal sealed partial class LocalizationProviderLogger
{
    private readonly ILogger<LocalizationProvider> _logger;

    public LocalizationProviderLogger(ILogger<LocalizationProvider> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Error, "Stream for translations file '{LocalizationResourceName}' is null.", EventId = 101)]
    public partial void LocalizationResourceStreamIsNull(string localizationResourceName);

    [LoggerMessage(LogLevel.Warning, "Could not find CultureInfo for language code '{LanguageCode}'. Falling back to 'InvariantCulture'.", EventId = 102)]
    public partial void CouldNotFindCultureInfoForLanguageCode(string languageCode);

    [LoggerMessage(LogLevel.Error, "Failed to load translations for language with code '{LanguageCode}'.", EventId = 103)]
    public partial void FailedToLoadTranslationsForLanguage(Exception exception, string languageCode);
}
