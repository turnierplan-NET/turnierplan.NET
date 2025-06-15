using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.User;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<List<User>> GetAllUsers();

    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);
}
