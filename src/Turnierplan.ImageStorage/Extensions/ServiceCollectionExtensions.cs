using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Turnierplan.ImageStorage.Local;
using Turnierplan.ImageStorage.S3;

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
            case "S3":
                services.Configure<S3ImageStorageOptions>(configuration);
                services.AddSingleton<IImageStorage, S3ImageStorage>();
                break;
            default:
                throw new InvalidOperationException($"Invalid image storage type specified: '{type}'");
        }
    }
}
