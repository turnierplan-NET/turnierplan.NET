namespace Turnierplan.ImageStorage.S3;

internal sealed record S3ImageStorageOptions
{
    public string? ServiceUrl { get; init; }

    public string? RegionEndpoint { get; init; }

    public string? AccessKey { get; init; }

    public string? AccessKeySecret { get; init; }

    public string? BucketName { get; init; }
}
