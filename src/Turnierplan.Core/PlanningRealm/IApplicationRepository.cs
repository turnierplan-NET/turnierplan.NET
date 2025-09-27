using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public interface IApplicationRepository : IRepository<Application, long>
{
    Task<Application?> GetByFormSessionAsync(Guid formSession);
}
