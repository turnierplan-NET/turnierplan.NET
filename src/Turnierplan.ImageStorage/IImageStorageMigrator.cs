namespace Turnierplan.ImageStorage;

internal sealed class ImageStorageMigrator(IImageStorage imageStorage, IImageProvider imageProvider)
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if (imageStorage is not IMigratableImageStorage migratableImageStorage)
        {
            return;
        }

        await migratableImageStorage.MigrateAsync(imageProvider, cancellationToken);
    }
}
