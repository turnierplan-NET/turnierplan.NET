namespace Turnierplan.App.Mapping;

internal interface IMappingRule
{
    Type SourceType { get; }

    Type DestinationType { get; }

    object Map(IMapper mapper, MappingContext context, object source);
}

internal abstract class MappingRuleBase<TSource, TDestination> : IMappingRule
    where TSource : class
    where TDestination : class
{
    public Type SourceType { get; } = typeof(TSource);

    public Type DestinationType { get; } = typeof(TDestination);

    protected abstract TDestination Map(IMapper mapper, MappingContext context, TSource source);

    object IMappingRule.Map(IMapper mapper, MappingContext context, object source)
    {
        if (source is not TSource convertedSource)
        {
            throw new InvalidOperationException($"Cannot map object with source type '{source.GetType().FullName}'.");
        }

        return Map(mapper, context, convertedSource);
    }
}
