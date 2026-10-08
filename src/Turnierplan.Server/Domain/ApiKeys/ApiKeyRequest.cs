namespace Turnierplan.Server.Domain.ApiKeys;

internal sealed class ApiKeyRequest : EntityBase<long>
{
    public ApiKeyRequest(string path)
    {
        Id = 0;
        Timestamp = DateTime.UtcNow;
        Path = path;
    }

    public ApiKeyRequest(long id, DateTime timestamp, string path)
    {
        Id = id;
        Timestamp = timestamp;
        Path = path;
    }

    public override long Id { get; protected set; }

    public DateTime Timestamp { get; }

    public string Path { get; }
}
