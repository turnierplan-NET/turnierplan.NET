using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage;

public interface IImageProvider
{
    Task<IReadOnlyCollection<Image>> GetImagesAsync();
}
