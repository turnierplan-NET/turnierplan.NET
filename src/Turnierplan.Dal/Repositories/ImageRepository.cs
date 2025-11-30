using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

public interface IImageRepository : IRepositoryWithPublicId<Image, long>;

internal sealed class ImageRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Image>(context), IImageRepository
{
    public override Task<Image?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization).ThenInclude(x => x.RoleAssignments)
            .Include(x => x.RoleAssignments)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }
}
