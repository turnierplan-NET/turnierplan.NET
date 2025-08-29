namespace Turnierplan.ImageStorage.Azure;

internal sealed record AzureImageStorageOptions
{
    public string? StorageAccountName { get; init; }

    public string? ContainerName { get; init; }

    public bool? UseAccountKey { get; init; }

    public string? AccountKey { get; init; }

    public bool? UseClientSecret { get; init; }

    public string? TenantId { get; init; }

    public string? ClientId { get; init; }

    public string? ClientSecret { get; init; }
}
