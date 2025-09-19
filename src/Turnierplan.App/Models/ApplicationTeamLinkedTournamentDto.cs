using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record ApplicationTeamLinkedTournamentDto
{
    public required PublicId Id { get; set; }

    public required string Name { get; set; }

    public required string? FolderName { get; set; }
}
