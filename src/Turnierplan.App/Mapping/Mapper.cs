using Turnierplan.Localization;

namespace Turnierplan.App.Mapping;

internal interface IMapper
{
    private const string DefaultLanguageCode = "de";

    TDestination Map<TDestination>(object source, string languageCode = DefaultLanguageCode)
        where TDestination : class;

    TDestination? MapNullable<TDestination>(object? source, string languageCode = DefaultLanguageCode)
        where TDestination : class;

    IEnumerable<TDestination> MapCollection<TDestination>(IReadOnlyCollection<object> source, string languageCode = DefaultLanguageCode)
        where TDestination : class;
}

internal sealed class Mapper : IMapper
{
    private readonly ILocalizationProvider _localizationProvider;
    private readonly Dictionary<Key, IMappingRule> _rules = [];

    public Mapper(ILocalizationProvider localizationProvider, IServiceProvider serviceProvider)
    {
        _localizationProvider = localizationProvider;
        var interfaceType = typeof(IMappingRule);

        foreach (var ruleType in typeof(Mapper).Assembly.GetTypes().Where(type => type is { IsClass: true, IsAbstract: false } && type.IsAssignableTo(interfaceType)))
        {
            if (ActivatorUtilities.CreateInstance(serviceProvider, ruleType) is not IMappingRule instance)
            {
                throw new InvalidOperationException($"Could not instantiate mapping rule of type '{ruleType.FullName}'.");
            }

            var key = new Key(instance.SourceType, instance.DestinationType);

            _rules.Add(key, instance);
        }
    }

    public TDestination Map<TDestination>(object source, string langaugeCode)
        where TDestination : class
    {
        var key = new Key(source.GetType(), typeof(TDestination));

        if (!_rules.TryGetValue(key, out var rule))
        {
            throw new InvalidOperationException($"There exists no mapping rule from '{key.SourceType.FullName}' to '{key.DestinationType.FullName}'.");
        }

        var context = CreateContext(langaugeCode);

        return (TDestination)rule.Map(this, context, source);
    }

    public TDestination? MapNullable<TDestination>(object? source, string langaugeCode)
        where TDestination : class
    {
        return source is null ? null : Map<TDestination>(source, langaugeCode);
    }

    public IEnumerable<TDestination> MapCollection<TDestination>(IReadOnlyCollection<object> source, string langaugeCode)
        where TDestination : class
    {
        if (source.Count == 0)
        {
            return [];
        }

        var key = new Key(source.First().GetType(), typeof(TDestination));

        if (!_rules.TryGetValue(key, out var rule))
        {
            throw new InvalidOperationException($"There exists no mapping rule from '{key.SourceType.FullName}' to '{key.DestinationType.FullName}'.");
        }

        var context = CreateContext(langaugeCode);

        return source.Select(x => rule.Map(this, context, x)).Cast<TDestination>();
    }

    private MappingContext CreateContext(string langaugeCode)
    {
        if (!_localizationProvider.TryGetLocalization(langaugeCode, out var localization))
        {
            throw new ArgumentException($"There exists no localization with language code '{langaugeCode}'.");
        }

        return new MappingContext(localization);
    }

    private record struct Key(Type SourceType, Type DestinationType);
}
