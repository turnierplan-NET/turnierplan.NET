using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage;

public interface IImageStorage : IDisposable
{
    string GetFullImageUrl(Image image);

    Task<bool> SaveImageAsync(Image image, MemoryStream imageData);

    Task<Stream> GetImageAsync(Image image);

    Task<bool> DeleteImageAsync(Image image);
}
