using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Organization;

public interface IOrganizationRepository : IRepositoryWithPublicId<Organization, long>
{
    Task<Organization?> GetByPublicIdAsync(PublicId.PublicId id, Include include);

    Task<List<Organization>> GetByOwnerUserIdAsync(Guid ownerUserId);

    [Flags]
    public enum Include
    {
        None = 0,
        Folders = 1,
        Tournaments = 2,
        Venues = 4,
        Images = 8,
        ApiKeys = 16
    }
}
