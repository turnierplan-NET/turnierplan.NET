using Turnierplan.Localization;

namespace Turnierplan.App.Mapping;

internal record struct MappingContext
{
    internal MappingContext(ILocalization localization)
    {
        Localization = localization;
    }

    public ILocalization Localization { get; init; }
}
