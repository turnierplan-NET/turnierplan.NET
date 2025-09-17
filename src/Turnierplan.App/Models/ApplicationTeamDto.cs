using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record ApplicationTeamDto
{
    public required long Id { get; init; }

    public required long TournamentClassId { get; init; }

    public required string Name { get; set; }

    public PublicId? LinkedTournamentId { get; set; }

    public string? LinkedTournamentFolderName { get; set; }

    public string? LinkedTournamentName { get; set; }
}
