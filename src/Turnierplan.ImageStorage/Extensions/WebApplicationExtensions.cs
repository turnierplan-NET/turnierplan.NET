using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Turnierplan.ImageStorage.Local;

namespace Turnierplan.ImageStorage.Extensions;

public static class WebApplicationExtensions
{
    public static void MapImageStorageEndpoint(this WebApplication application)
    {
        var imageStorage = application.Services.GetRequiredService<IImageStorage>();

        if (imageStorage is not LocalImageStorage localImageStorage)
        {
            return;
        }

        localImageStorage.MapEndpoint(application);
    }

    public static async Task MigrateImageStorageAsync(this WebApplication application)
    {
        await using var scope = application.Services.CreateAsyncScope();

        var migrator = scope.ServiceProvider.GetService<ImageStorageMigrator>();

        if (migrator is null)
        {
            return;
        }

        await migrator.MigrateAsync(application.Lifetime.ApplicationStopping);

    }
}
