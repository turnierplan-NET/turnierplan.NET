using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Models;

public sealed record FolderStatisticsPageViewsDto
{
    internal FolderStatisticsPageViewsDto(Tournament tournament)
    {
        TournamentId = tournament.PublicId;
        TournamentName = tournament.Name;
        PublicPageViews = tournament.PublicPageViews;
    }

    public PublicId TournamentId { get; }

    public string TournamentName { get; }

    public int PublicPageViews { get; }
}
