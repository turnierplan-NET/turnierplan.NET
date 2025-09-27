using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.Repositories;

internal sealed class ApplicationRepository(TurnierplanContext context) : RepositoryBase<Application, long>(context, context.Applications), IApplicationRepository
{
    public Task<Application?> GetByFormSessionAsync(Guid formSession)
    {
        return DbSet.Where(x => x.FormSession == formSession).FirstOrDefaultAsync();
    }
}
