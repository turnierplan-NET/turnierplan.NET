using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage;

internal abstract class ImageStorageBase : IImageStorage
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

    protected virtual void Dispose(bool disposing)
    {
    }
}
