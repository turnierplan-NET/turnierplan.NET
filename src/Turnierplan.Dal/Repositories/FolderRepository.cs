using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Folder;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

internal sealed class FolderRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Folder>(context, context.Folders), IFolderRepository
{
    public override Task<Folder?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization)
            .FirstOrDefaultAsync();
    }

    public async Task<Folder?> GetByPublicIdAsync(PublicId id, IFolderRepository.Include include)
    {
        var query = DbSet.Where(x => x.PublicId == id);

        query = query.Include(x => x.Organization);

        if (include.HasFlag(IFolderRepository.Include.Tournaments))
        {
            query = query.Include(x => x.Tournaments);
        }

        if (include.HasFlag(IFolderRepository.Include.TournamentsWithMatchesAndGroups))
        {
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Matches);
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Groups);

            query = query.AsSplitQuery();
        }

        if (include.HasFlag(IFolderRepository.Include.TournamentsWithGameRelevant))
        {
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Matches);
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Groups);
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Teams);

            query = query.AsSplitQuery();
        }

        return await query.FirstOrDefaultAsync();
    }
}
