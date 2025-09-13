using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Organization;

public interface IOrganizationRepository : IRepositoryWithPublicId<Organization, long>
{
    Task<Organization?> GetByPublicIdAsync(PublicId.PublicId id, Includes includes);

    Task<List<Organization>> GetAllAsync();

    /// <summary>
    /// Returns a list of all organizations that have any role assignment for the specified principal.
    /// </summary>
    Task<List<Organization>> GetByPrincipalAsync(Principal principal);

    [Flags]
    public enum Includes
    {
        None = 0,
        Folders = 1,
        Tournaments = 2,
        Venues = 4,
        Images = 8,
        ApiKeys = 16,
        PlanningRealms = 32
    }
}
