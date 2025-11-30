using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.Repositories;

public interface IApplicationChangeLogRepository : IRepository<ApplicationChangeLog, long>
{
    Task<List<ApplicationChangeLog>> GetByApplicationIdAsync(long applicationId);
}

internal sealed class ApplicationChangeLogRepository(TurnierplanContext context) : RepositoryBase<ApplicationChangeLog, long>(context), IApplicationChangeLogRepository
{
    public Task<List<ApplicationChangeLog>> GetByApplicationIdAsync(long applicationId)
    {
        return DbSet.Where(x => x.Application.Id == applicationId)
            .ToListAsync();
    }
}
