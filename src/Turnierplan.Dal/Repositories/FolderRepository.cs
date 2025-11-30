using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Folder;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

public interface IFolderRepository : IRepositoryWithPublicId<Folder, long>
{
    Task<Folder?> GetByPublicIdAsync(PublicId id, Includes includes);

    [Flags]
    public enum Includes
    {
        None = 0,
        Tournaments = 1,
        TournamentsWithMatchesAndGroups = 2,
        TournamentsWithGameRelevant = 3
    }
}

internal sealed class FolderRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Folder>(context), IFolderRepository
{
    public override Task<Folder?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization).ThenInclude(x => x.RoleAssignments)
            .Include(x => x.RoleAssignments)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    public async Task<Folder?> GetByPublicIdAsync(PublicId id, IFolderRepository.Includes includes)
    {
        var query = DbSet.Where(x => x.PublicId == id);

        query = query.Include(x => x.Organization).ThenInclude(x => x.RoleAssignments);
        query = query.Include(x => x.RoleAssignments);

        if (includes.HasFlag(IFolderRepository.Includes.Tournaments))
        {
            query = query.Include(x => x.Tournaments);
        }

        if (includes.HasFlag(IFolderRepository.Includes.TournamentsWithMatchesAndGroups))
        {
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Matches);
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Groups);
        }

        if (includes.HasFlag(IFolderRepository.Includes.TournamentsWithGameRelevant))
        {
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Matches);
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Groups);
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Teams);
        }

        return await query.AsSplitQuery().FirstOrDefaultAsync();
    }
}
