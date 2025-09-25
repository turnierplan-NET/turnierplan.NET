using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record FolderStatisticsExcludedTournamentDto
{
    public required PublicId TournamentId { get; init; }

    public required string TournamentName { get; init; }
}
