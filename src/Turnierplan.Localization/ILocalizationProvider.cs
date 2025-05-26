using System.Diagnostics.CodeAnalysis;

namespace Turnierplan.Localization;

public interface ILocalizationProvider
{
    bool TryGetLocalization(string languageCode, [NotNullWhen(true)] out ILocalization? localization);
}
