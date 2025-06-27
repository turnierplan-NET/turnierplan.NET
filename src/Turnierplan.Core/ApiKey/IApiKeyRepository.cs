using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.ApiKey;

public interface IApiKeyRepository : IRepositoryWithPublicId<ApiKey, long>
{
    Task<ApiKey?> GetByPrincipalIdAsync(Guid id);

    Task<List<ApiKeyRequest>> GetRequestsInTimeRange(ApiKey apiKey, DateTime start, DateTime end);
}
