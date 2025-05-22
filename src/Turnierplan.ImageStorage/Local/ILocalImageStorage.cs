using Microsoft.AspNetCore.Builder;

namespace Turnierplan.ImageStorage.Local;

public interface ILocalImageStorage : IImageStorage
{
    void MapEndpoint(IApplicationBuilder builder);
}
