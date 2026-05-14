using Microsoft.AspNetCore.Builder;

namespace Turnierplan.ImageStorage;

internal interface IHostedImageStorage
{
    void MapEndpoint(IApplicationBuilder applicationBuilder);
}
