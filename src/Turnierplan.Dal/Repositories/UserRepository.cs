using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.User;

namespace Turnierplan.Dal.Repositories;

internal sealed class UserRepository(TurnierplanContext context) : RepositoryBase<User, Guid>(context, context.Users), IUserRepository
{
    public Task<List<User>> GetAllUsers()
    {
        return DbSet
            .ToListAsync();
    }

    public Task<User?> GetByIdAsync(Guid id, bool includeOrganizationsDeep = false)
    {
        var query = DbSet.Where(x => x.Id.Equals(id));

        if (includeOrganizationsDeep)
        {
            query = query.Include(x => x.Organizations).ThenInclude(x => x.Images);
            query = query.Include(x => x.Organizations).ThenInclude(x => x.Tournaments);
            query = query.Include(x => x.Organizations).ThenInclude(x => x.Venues);

            query = query.AsSplitQuery();
        }

        return query.FirstOrDefaultAsync();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEMail = User.NormalizeEmail(email);

        return DbSet
            .Where(x => x.NormalizedEMail.Equals(normalizedEMail))
            .FirstOrDefaultAsync();
    }
}
