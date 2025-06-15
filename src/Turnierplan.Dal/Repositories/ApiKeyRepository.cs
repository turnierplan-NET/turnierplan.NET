using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

internal sealed class ApiKeyRepository : RepositoryBaseWithPublicId<ApiKey>, IApiKeyRepository
{
    private readonly DbSet<ApiKeyRequest> _requests;

    public ApiKeyRepository(TurnierplanContext context)
        : base(context, context.ApiKeys)
    {
        _requests = context.ApiKeyRequests;
    }

    public override Task<ApiKey?> GetByPublicIdAsync(PublicId id)
    {
        return DbSet.Where(x => x.PublicId == id)
            .Include(x => x.Organization).ThenInclude(x => x.RoleAssignments)
            .Include(x => x.RoleAssignments)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    public Task<List<ApiKeyRequest>> GetRequestsInTimeRange(ApiKey apiKey, DateTime start, DateTime end)
    {
        return _requests.Where(x => x.ApiKey == apiKey && x.Timestamp >= start && x.Timestamp <= end)
            .ToListAsync();
    }
}
