using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record ImageDto
{
    public required PublicId Id { get; init; }

    public required string RbacScopeId { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required ImageType Type { get; init; }

    public required string Name { get; init; }

    public required string Url { get; init; }

    public required long FileSize { get; init; }

    public required ushort Width { get; init; }

    public required ushort Height { get; init; }
}
