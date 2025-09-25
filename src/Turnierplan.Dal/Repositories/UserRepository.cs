using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.User;

namespace Turnierplan.Dal.Repositories;

internal sealed class UserRepository(TurnierplanContext context) : RepositoryBase<User, Guid>(context, context.Users), IUserRepository
{
    public Task<List<User>> GetAllUsersAsync()
    {
        return DbSet
            .ToListAsync();
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return DbSet.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
    }

    public Task<User?> GetByPrincipalIdAsync(Guid id)
    {
        return DbSet.Where(x => x.PrincipalId == id).FirstOrDefaultAsync();
    }

    public Task<User?> GetByUserNameAsync(string userName)
    {
        var normalizedUserName = User.Normalize(userName);

        return DbSet
            .Where(x => x.NormalizedUserName.Equals(normalizedUserName))
            .FirstOrDefaultAsync();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEMail = User.Normalize(email);

        return DbSet
            .Where(x => x.NormalizedEMail != null && x.NormalizedEMail.Equals(normalizedEMail))
            .FirstOrDefaultAsync();
    }

    public Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail)
    {
        var normalizedUserNameOrEmail = User.Normalize(userNameOrEmail);

        return DbSet
            .Where(x => x.NormalizedUserName.Equals(normalizedUserNameOrEmail) || (x.NormalizedEMail != null && x.NormalizedEMail.Equals(normalizedUserNameOrEmail)))
            .FirstOrDefaultAsync();
    }
}
