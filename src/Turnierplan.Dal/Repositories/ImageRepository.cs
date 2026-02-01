using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

public interface IImageRepository : IRepositoryWithPublicId<Image, long>
{
    Task<int> CountNumberOfReferencingTournamentsAsync(long imageId);
}

internal sealed class ImageRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Image>(context), IImageRepository
{
    private readonly TurnierplanContext _context = context;

    public override Task<Image?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization).ThenInclude(x => x.RoleAssignments)
            .Include(x => x.RoleAssignments)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    public async Task<int> CountNumberOfReferencingTournamentsAsync(long imageId)
    {
        var count = await _context.Tournaments
            .Where(x => x.PrimaryLogo!.Id == imageId || x.SecondaryLogo!.Id == imageId || x.BannerImage!.Id == imageId)
            .CountAsync();

        return count;
    }

}
