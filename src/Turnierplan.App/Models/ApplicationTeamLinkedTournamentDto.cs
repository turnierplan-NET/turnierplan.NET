using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record ApplicationTeamLinkedTournamentDto
{
    public required PublicId Id { get; init; }

    public required string Name { get; init; }

    public required string? FolderName { get; init; }
}
