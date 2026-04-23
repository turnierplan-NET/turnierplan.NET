using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage;

internal abstract class ImageStorageBase : IImageStorage, IMigratableImageStorage
{
    ~ImageStorageBase()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public abstract string GetFullImageUrl(Image image);

    public abstract Task<bool> SaveImageAsync(Image image, MemoryStream imageData);

    public abstract Task<Stream> GetImageAsync(Image image);

    public abstract Task<bool> DeleteImageAsync(Image image);

    public virtual Task MigrateAsync(IImageProvider imageProvider, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
