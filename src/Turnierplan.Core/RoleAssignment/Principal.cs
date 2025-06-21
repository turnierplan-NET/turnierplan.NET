using System.Text.Json.Serialization;

namespace Turnierplan.Core.RoleAssignment;

public sealed record Principal
{
    public Principal(PrincipalKind kind, Guid principalId)
    {
        Kind = kind;
        PrincipalId = principalId;
    }

    [JsonPropertyName("k")]
    public PrincipalKind Kind { get; }

    [JsonPropertyName("pid")]
    public Guid PrincipalId { get; }
}
