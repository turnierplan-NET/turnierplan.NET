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

    public Task<User?> GetByIdAsync(Guid id)
    {
        return DbSet.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEMail = User.NormalizeEmail(email);

        return DbSet
            .Where(x => x.NormalizedEMail.Equals(normalizedEMail))
            .FirstOrDefaultAsync();
    }
}
