using System.Globalization;

namespace Turnierplan.Localization;

internal sealed class InMemoryLocalization : LocalizationBase
{
    private readonly IReadOnlyDictionary<string, string> _values;

    public InMemoryLocalization(CultureInfo culture, IReadOnlyDictionary<string, string> values)
    {
        _values = values;
        Culture = culture;
    }

    // internal for testing
    internal CultureInfo Culture { get; }

    public override string Get(string key)
    {
        return _values.GetValueOrDefault(key, key);
    }

    public override string Get(string key, object arg1)
    {
        return _values.TryGetValue(key, out var value) ? string.Format(Culture, value, arg1) : key;
    }

    public override string Get(string key, object arg1, object arg2)
    {
        return _values.TryGetValue(key, out var value) ? string.Format(Culture, value, arg1, arg2) : key;
    }

    public override string Get(string key, object arg1, object arg2, object arg3)
    {
        return _values.TryGetValue(key, out var value) ? string.Format(Culture, value, arg1, arg2, arg3) : key;
    }

    public override string Get(string key, params object[] args)
    {
        return _values.TryGetValue(key, out var value) ? string.Format(Culture, value, args) : key;
    }
}
