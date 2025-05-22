using System.Text.Json.Serialization;
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

    public required Outcome MostSignificantOutcome { get; init; }

    public required int NumberOfDecidingMatches { get; init; }

    public required int NumberOfPenaltyShootouts { get; init; }

    public required int NumberOfOverTimes { get; init; }

    public required int TotalPublicPageViews { get; init; }

    public required float AveragePublicPageViewsPerTournament { get; init; }

    public required List<PageViewEntry> TournamentPageViews { get; init; }

    public required List<OutcomeDistributionEntry> OutcomeDistribution { get; init; }

    public required List<ExcludedTournamentEntry> GoalDistributionExcludedTournaments { get; init; }

    public sealed record Outcome
    {
        public Outcome(int scoreA, int scoreB)
        {
            if (scoreA < scoreB)
            {
                throw new ArgumentException($"{nameof(scoreA)} must be greater than or equal to {nameof(scoreB)}.");
            }

            ScoreA = scoreA;
            ScoreB = scoreB;
            Difference = scoreA - scoreB;
        }

        public int ScoreA { get; }

        public int ScoreB { get; }

        [JsonIgnore]
        internal int Difference { get; }
    }

    public sealed record OutcomeDistributionEntry(Outcome Outcome, int Count);

    public sealed record PageViewEntry(PublicId TournamentId, string TournamentName, int PublicPageViews);

    public sealed record ExcludedTournamentEntry(PublicId TournamentId, string TournamentName);
}
