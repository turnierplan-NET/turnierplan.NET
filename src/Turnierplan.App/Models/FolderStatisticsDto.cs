using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record FolderStatisticsDto
{
    public required PublicId FolderId { get; init; }

    public required PublicId OrganizationId { get; init; }

    public required string FolderName { get; init; }

    public required int NumberOfTournaments { get; init; }

    public required int NumberOfMatches { get; init; }

    public required int NumberOfFinishedMatches { get; init; }

    public required float PercentageOfFinishedMatches { get; init; }

    public required int NumberOfTeams { get; init; }

    public required int NumberOfGroups { get; init; }

    public required int NumberOfGoals { get; init; }

    public required float AverageGoalsPerMatch { get; init; }

    public required int MostGoalsPerMatch { get; init; }

    public required FolderStatisticsOutcomeDto MostSignificantOutcome { get; init; }

    public required int NumberOfDecidingMatches { get; init; }

    public required int NumberOfPenaltyShootouts { get; init; }

    public required int NumberOfOverTimes { get; init; }

    public required int TotalPublicPageViews { get; init; }

    public required float AveragePublicPageViewsPerTournament { get; init; }

    public required List<FolderStatisticsPageViewsDto> TournamentPageViews { get; init; }

    public required List<FolderStatisticsOutcomeDistributionDto> OutcomeDistribution { get; init; }

    public required List<FolderStatisticsExcludedTournamentDto> GoalDistributionExcludedTournaments { get; init; }
}
