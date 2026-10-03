namespace Turnierplan.Server.Infrastructure.Endpoints.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    public static void MapTurnierplanEndpoints(this IEndpointRouteBuilder builder)
    {
        var baseType = typeof(EndpointBase);

        foreach (var type in baseType.Assembly.GetTypes())
        {
            if (!type.IsAssignableTo(baseType) || type == baseType)
            {
                continue;
            }

            var constructor = type.GetConstructor([]);

            if (constructor is null)
            {
                throw new InvalidOperationException($"Endpoint type '{type.FullName}' must declare a parameterless constructor.");
            }

            var instance = constructor.Invoke([]);

            if (instance is not EndpointBase endpointBase)
            {
                throw new InvalidOperationException($"Could not instantiate endpoint type '{type.FullName}'.");
            }

            endpointBase.Map(builder);
        }
    }
}
