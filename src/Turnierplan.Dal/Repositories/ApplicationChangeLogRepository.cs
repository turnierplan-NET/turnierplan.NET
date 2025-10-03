using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.Repositories;

internal sealed class ApplicationChangeLogRepository(TurnierplanContext context) : RepositoryBase<ApplicationChangeLog, long>(context, context.ApplicationChangeLogs), IApplicationChangeLogRepository
{
    public Task<List<ApplicationChangeLog>> GetByApplicationIdAsync(long applicationId)
    {
        return DbSet.Where(x => x.Application.Id == applicationId)
            .ToListAsync();
    }
}
