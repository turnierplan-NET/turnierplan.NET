using Turnierplan.App.Endpoints;

namespace Turnierplan.App.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    public static void MapTurnierplanEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpointBaseType = typeof(EndpointBase);
        var endpointTypes = typeof(EndpointRouteBuilderExtensions).Assembly.GetTypes().Where(x => x is { IsClass: true, IsAbstract: false } && x.IsAssignableTo(endpointBaseType));

        foreach (var endpointType in endpointTypes)
        {
            var instance = ActivatorUtilities.CreateInstance(builder.ServiceProvider, endpointType);

            if (instance is not EndpointBase endpoint)
            {
                throw new InvalidOperationException($"Failed to create instance of endpoint type '{endpointType.FullName}'.");
            }

            endpoint.Map(builder);
        }
    }
}
