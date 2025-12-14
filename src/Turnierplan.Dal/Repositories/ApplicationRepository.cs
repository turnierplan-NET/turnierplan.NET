using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.Repositories;

public interface IApplicationRepository : IRepository<Application, long>
{
    Task<Application?> GetByFormSessionAsync(Guid formSession);
}

internal sealed class ApplicationRepository(TurnierplanContext context) : RepositoryBase<Application, long>(context), IApplicationRepository
{
    public Task<Application?> GetByFormSessionAsync(Guid formSession)
    {
        return DbSet.Where(x => x.FormSession == formSession).FirstOrDefaultAsync();
    }
}
