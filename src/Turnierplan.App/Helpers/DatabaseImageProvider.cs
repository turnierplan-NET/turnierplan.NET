using Turnierplan.Core.Image;
using Turnierplan.Dal.Repositories;
using Turnierplan.ImageStorage;

namespace Turnierplan.App.Helpers;

internal sealed class DatabaseImageProvider(IImageRepository imageRepository) : IImageProvider
{
    public async Task<IReadOnlyCollection<Image>> GetImagesAsync()
    {
        return (await imageRepository.GetAllImagesAsync()).AsReadOnly();
    }
}
