using System.Text.Json.Serialization;

namespace Turnierplan.Core.RoleAssignment;

public sealed record Principal
{
    public Principal(PrincipalKind kind, string objectId)
    {
        Kind = kind;
        ObjectId = objectId;
    }

    [JsonPropertyName("k")]
    public PrincipalKind Kind { get; }

    [JsonPropertyName("oid")]
    public string ObjectId { get; }
}
