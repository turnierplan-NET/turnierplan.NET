namespace Turnierplan.ImageStorage;

internal interface IMigratableImageStorage
{
    Task MigrateAsync(IImageProvider imageProvider, CancellationToken cancellationToken);
}
