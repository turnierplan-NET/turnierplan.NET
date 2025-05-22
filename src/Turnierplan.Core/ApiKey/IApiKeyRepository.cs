using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.ApiKey;

public interface IApiKeyRepository : IRepositoryWithPublicId<ApiKey, long>
{
    Task<List<ApiKeyRequest>> GetRequestsInTimeRange(ApiKey apiKey, DateTime start, DateTime end);
}
