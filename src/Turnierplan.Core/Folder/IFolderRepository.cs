using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Folder;

public interface IFolderRepository : IRepositoryWithPublicId<Folder, long>
{
    Task<Folder?> GetByPublicIdAsync(PublicId.PublicId id, Include include);

    [Flags]
    public enum Include
    {
        None = 0,
        Tournaments = 1,
        TournamentsWithMatchesAndGroups = 2,
        TournamentsWithGameRelevant = 3
    }
}
