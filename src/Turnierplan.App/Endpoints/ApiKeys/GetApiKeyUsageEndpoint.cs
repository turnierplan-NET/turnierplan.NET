using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ApiKeys;

internal sealed class GetApiKeyUsageEndpoint : EndpointBase<ApiKeyUsageDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/api-keys/{id}/usage";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromQuery] int rangeDays,
        IApiKeyRepository repository,
        IAccessValidator accessValidator)
    {
        if (rangeDays is < 1 or > 30)
        {
            return Results.BadRequest("Range in days must be between 1 and 30.");
        }

        var apiKey = await repository.GetByPublicIdAsync(id);

        if (apiKey is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(apiKey, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        var (bucketWidth, bucketCount) = GetBucketProperties(rangeDays);
        var timeRange = bucketWidth * bucketCount;

        var endTime = new DateTime((long)(Math.Ceiling((double)DateTime.UtcNow.Ticks / bucketWidth.Ticks) * bucketWidth.Ticks), DateTimeKind.Utc);
        var startTime = endTime - timeRange;

        var requests = await repository.GetRequestsInTimeRange(apiKey, startTime, endTime);

        var requestsGroupedIntoBuckets = requests.GroupBy(x =>
        {
            var offset = x.Timestamp - startTime;
            return (int)(offset.Ticks / bucketWidth.Ticks);
        });

        return Results.Ok(new ApiKeyUsageDto
        {
            TimeFrameStart = startTime,
            TimeFrameEnd = endTime,
            BucketWidthSeconds = (int)bucketWidth.TotalSeconds,
            BucketCount = bucketCount,
            Buckets = requestsGroupedIntoBuckets
                .Select(bucket => new ApiKeyUsageBucketDto
                {
                    BucketIndex = bucket.Key,
                    Count = bucket.Count()
                })
                .ToList()
        });
    }

    private static (TimeSpan BucketWidth, int BucketCount) GetBucketProperties(int rangeDays)
    {
        return rangeDays switch
        {
            < 1 => throw new ArgumentOutOfRangeException(nameof(rangeDays)),
            <= 1 => (TimeSpan.FromMinutes(30), 24 * 2),
            <= 3 => (TimeSpan.FromHours(1), rangeDays * 24),
            <= 7 => (TimeSpan.FromHours(2), rangeDays * 24 / 2),
            <= 14 => (TimeSpan.FromHours(4), rangeDays * 24 / 4),
            <= 30 => (TimeSpan.FromHours(8), rangeDays * 24 / 8),
            _ => throw new ArgumentOutOfRangeException(nameof(rangeDays))
        };
    }
}
