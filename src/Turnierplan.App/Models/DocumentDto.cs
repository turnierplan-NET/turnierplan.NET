using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record DocumentDto
{
    public required PublicId Id { get; init; }

    public required PublicId TournamentId { get; init; }

    public required DocumentType Type { get; init; }

    public required string Name { get; init; }

    public required DateTime LastModifiedAt { get; init; }

    public required int GenerationCount { get; init; }
}
