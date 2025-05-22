using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record ApiKeyDto
{
    public required PublicId Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string? Secret { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime ExpiryDate { get; init; }

    public required bool IsExpired { get; init; }

    public required bool IsActive { get; init; }
}
