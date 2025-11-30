using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Repositories;

public interface IApiKeyRepository : IRepositoryWithPublicId<ApiKey, long>
{
    Task<ApiKey?> GetByPrincipalIdAsync(Guid id);

    Task<List<ApiKeyRequest>> GetRequestsInTimeRange(ApiKey apiKey, DateTime start, DateTime end);
}

internal sealed class ApiKeyRepository : RepositoryBaseWithPublicId<ApiKey>, IApiKeyRepository
{
    private readonly DbSet<ApiKeyRequest> _requests;

    public ApiKeyRepository(TurnierplanContext context)
        : base(context)
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

    public Task<ApiKey?> GetByPrincipalIdAsync(Guid id)
    {
        return DbSet.Where(x => x.PrincipalId == id).FirstOrDefaultAsync();
    }

    public Task<List<ApiKeyRequest>> GetRequestsInTimeRange(ApiKey apiKey, DateTime start, DateTime end)
    {
        return _requests.Where(x => x.ApiKey == apiKey && x.Timestamp >= start && x.Timestamp <= end)
            .ToListAsync();
    }
}
