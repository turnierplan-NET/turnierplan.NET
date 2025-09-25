using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.User;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<List<User>> GetAllUsersAsync();

    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByPrincipalIdAsync(Guid id);

    Task<User?> GetByUserNameAsync(string userName);

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail);
}
