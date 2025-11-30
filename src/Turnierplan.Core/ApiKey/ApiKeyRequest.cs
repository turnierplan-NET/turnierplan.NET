using Turnierplan.Core.Entity;

namespace Turnierplan.Core.ApiKey;

public sealed class ApiKeyRequest : Entity<long>
{
    public ApiKeyRequest(string path)
    {
        Id = 0;
        Timestamp = DateTime.UtcNow;
        Path = path;
    }

    internal ApiKeyRequest(long id, DateTime timestamp, string path)
    {
        Id = id;
        Timestamp = timestamp;
        Path = path;
    }

    public override long Id { get; protected set; }

    public ApiKey ApiKey { get; internal set; } = null!;

    public DateTime Timestamp { get; }

    public string Path { get; }
}
