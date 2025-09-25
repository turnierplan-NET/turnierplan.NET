using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record FolderStatisticsPageViewsDto
{
    public required PublicId TournamentId { get; init; }

    public required string TournamentName { get; init; }

    public required int PublicPageViews { get; init; }
}
