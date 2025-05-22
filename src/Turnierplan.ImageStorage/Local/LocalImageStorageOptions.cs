namespace Turnierplan.ImageStorage.Local;

internal sealed record LocalImageStorageOptions
{
    public string? StoragePath { get; init; }
}
