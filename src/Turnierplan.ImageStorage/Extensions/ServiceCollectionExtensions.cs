using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Turnierplan.ImageStorage.Local;

namespace Turnierplan.ImageStorage.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTurnierplanImageStorage(this IServiceCollection services, IConfigurationSection configuration)
    {
        var type = configuration.GetValue<string>("Type");

        switch (type)
        {
            case "Local":
                services.Configure<LocalImageStorageOptions>(configuration);
                services.AddSingleton<IImageStorage, LocalImageStorage>();
                break;
            default:
                throw new InvalidOperationException($"Invalid image storage type specified: '{type}'");
        }
    }
}
