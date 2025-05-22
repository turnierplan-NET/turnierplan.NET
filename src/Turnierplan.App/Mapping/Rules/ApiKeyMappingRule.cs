using Turnierplan.App.Models;
using Turnierplan.Core.ApiKey;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class ApiKeyMappingRule : MappingRuleBase<ApiKey, ApiKeyDto>
{
    protected override ApiKeyDto Map(IMapper mapper, MappingContext context, ApiKey source)
    {
        return new ApiKeyDto
        {
            Id = source.PublicId,
            Name = source.Name,
            Description = source.Description,
            Secret = null,
            CreatedAt = source.CreatedAt,
            ExpiryDate = source.ExpiryDate,
            IsExpired = source.IsExpired,
            IsActive = source.IsActive
        };
    }
}
