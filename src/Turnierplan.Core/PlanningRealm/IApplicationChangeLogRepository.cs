using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public interface IApplicationChangeLogRepository : IRepository<ApplicationChangeLog, long>
{
    Task<List<ApplicationChangeLog>> GetByApplicationIdAsync(long applicationId);
}
