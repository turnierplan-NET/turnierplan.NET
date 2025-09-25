using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Models;

public sealed record FolderStatisticsExcludedTournamentDto
{
    internal FolderStatisticsExcludedTournamentDto(Tournament tournament)
    {
        TournamentId = tournament.PublicId;
        TournamentName = tournament.Name;
    }

    public PublicId TournamentId { get; }

    public string TournamentName { get; }
}
