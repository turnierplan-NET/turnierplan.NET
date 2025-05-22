using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Models;

public sealed record TournamentHeaderDto
{
    public required PublicId Id { get; init; }

    public required PublicId OrganizationId { get; init; }

    public PublicId? FolderId { get; init; }

    public required string Name { get; init; }

    public required string OrganizationName { get; init; }

    public string? FolderName { get; init; }

    public required Visibility Visibility { get; init; }
}
