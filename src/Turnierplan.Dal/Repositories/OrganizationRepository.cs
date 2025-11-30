using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Dal.Repositories;

public interface IOrganizationRepository : IRepositoryWithPublicId<Organization, long>
{
    Task<Organization?> GetByPublicIdAsync(PublicId id, Includes includes);

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

internal sealed class OrganizationRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Organization>(context), IOrganizationRepository
{
    public override Task<Organization?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.RoleAssignments)
            .FirstOrDefaultAsync();
    }

    public Task<Organization?> GetByPublicIdAsync(PublicId id, IOrganizationRepository.Includes includes)
    {
        var query = DbSet.Where(o => o.PublicId == id);

        if (includes.HasFlag(IOrganizationRepository.Includes.Tournaments))
        {
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Folder);
        }

        if (includes.HasFlag(IOrganizationRepository.Includes.Venues))
        {
            query = query.Include(x => x.Venues);
        }

        if (includes.HasFlag(IOrganizationRepository.Includes.Folders))
        {
            query = query.Include(x => x.Folders);
        }

        if (includes.HasFlag(IOrganizationRepository.Includes.Images))
        {
            query = query.Include(x => x.Images);
        }

        if (includes.HasFlag(IOrganizationRepository.Includes.ApiKeys))
        {
            query = query.Include(x => x.ApiKeys);
        }

        if (includes.HasFlag(IOrganizationRepository.Includes.PlanningRealms))
        {
            query = query.Include(x => x.PlanningRealms);
        }

        query = query
            .Include(x => x.RoleAssignments)
            .AsSplitQuery();

        return query.FirstOrDefaultAsync();
    }

    public Task<List<Organization>> GetAllAsync()
    {
        return DbSet.ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<Organization>> GetByPrincipalAsync(Principal principal)
    {
        // IDEA: Try to optimize this query directly within EF

        return context.OrganizationRoleAssignments
            .Where(r => r.Principal.Equals(principal))
            .Include(r => r.Scope)
            .Select(r => r.Scope)
            .Distinct()
            .ToListAsync();
    }
}
