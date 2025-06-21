using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Dal.Repositories;

internal sealed class OrganizationRepository(TurnierplanContext context) : RepositoryBaseWithPublicId<Organization, long>(context, context.Organizations), IOrganizationRepository
{
    public override Task<Organization?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.RoleAssignments)
            .FirstOrDefaultAsync();
    }

    public Task<Organization?> GetByPublicIdAsync(PublicId id, IOrganizationRepository.Include include)
    {
        var query = DbSet.Where(o => o.PublicId == id);

        if (include.HasFlag(IOrganizationRepository.Include.Tournaments))
        {
            query = query.Include(x => x.Tournaments).ThenInclude(x => x.Folder);
        }

        if (include.HasFlag(IOrganizationRepository.Include.Venues))
        {
            query = query.Include(x => x.Venues);
        }

        if (include.HasFlag(IOrganizationRepository.Include.Folders))
        {
            query = query.Include(x => x.Folders);
        }

        if (include.HasFlag(IOrganizationRepository.Include.Images))
        {
            query = query.Include(x => x.Images);
        }

        if (include.HasFlag(IOrganizationRepository.Include.ApiKeys))
        {
            query = query.Include(x => x.ApiKeys);
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
        // TODO: Try to optimize this query directly within EF

        return context.OrganizationRoleAssignments
            .Where(r => r.Principal.Equals(principal))
            .Include(r => r.Scope)
            .Select(r => r.Scope)
            .ToListAsync();
    }
}
