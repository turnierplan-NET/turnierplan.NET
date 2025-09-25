namespace Turnierplan.App.Models;

public sealed record ApiKeyUsageBucketDto
{
    public required int BucketIndex { get; init; }

    public required int Count { get; init; }
}
