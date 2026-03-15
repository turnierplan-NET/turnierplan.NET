using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage.Local;

internal sealed class LocalImageStorageMigration : IImageStorageMigration
{
    private readonly LocalImageStorage _imageStorage;

    public LocalImageStorageMigration(IImageStorage imageStorage)
    {
        if (imageStorage is not LocalImageStorage localImageStorage)
        {
            throw new ArgumentException($"{nameof(LocalImageStorageMigration)} requires the registered {nameof(IImageStorage)} to be of type {nameof(LocalImageStorage)}.", nameof(imageStorage));
        }

        _imageStorage = localImageStorage;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await _imageStorage.MigrateAsync(LoadImagesAsync, cancellationToken);
    }

    private async Task<IList<Image>> LoadImagesAsync()
    {
        // TODO: We cannot load images here because this project does not reference Turnierplan.Dal (!)
    }
}
