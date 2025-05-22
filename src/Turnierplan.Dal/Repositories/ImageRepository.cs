using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

internal sealed class ImageRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Image>(context, context.Images), IImageRepository
{
    public override Task<Image?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization)
            .FirstOrDefaultAsync();
    }
}
