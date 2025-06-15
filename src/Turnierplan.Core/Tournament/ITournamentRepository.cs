using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Tournament;

public interface ITournamentRepository : IRepositoryWithPublicId<Tournament, long>
{
    Task<Tournament?> GetByPublicIdAsync(PublicId.PublicId id, Include include);

    [Flags]
    public enum Include
    {
        None = 0,
        Teams = 1,
        Groups = 2,
        Matches = 4,
        Documents = 8,
        Venue = 16,
        FolderWithTournaments = 32,
        Images = 64,

        GameRelevant = Teams | Groups | Matches
    }
}
