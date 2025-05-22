using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Turnierplan.ImageStorage.Local;

namespace Turnierplan.ImageStorage.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapImageStorageEndpoint(this IApplicationBuilder builder)
    {
        var imageStorage = builder.ApplicationServices.GetRequiredService<IImageStorage>();

        if (imageStorage is not ILocalImageStorage localImageStorage)
        {
            return;
        }

        localImageStorage.MapEndpoint(builder);
    }
}
