namespace Turnierplan.App.Models;

public sealed record ApiKeyUsageDto
{
    public required DateTime TimeFrameStart { get; init; }

    public required DateTime TimeFrameEnd { get; init; }

    public required int BucketWidthSeconds { get; init; }

    public required int BucketCount { get; init; }

    public required List<ApiKeyUsageBucketDto> Buckets { get; init; }
}
