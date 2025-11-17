namespace Turnierplan.PdfRendering.Configuration;

public sealed record MatchPlanDocumentConfiguration : IDocumentConfiguration
{
    public string? OrganizerNameOverride { get; init; }

    public string? TournamentNameOverride { get; init; }

    public string? VenueOverride { get; init; }

    public MatchPlanDateFormat DateFormat { get; init; } = MatchPlanDateFormat.DateAndDayOfWeek;

    public MatchPlanOutcomes Outcomes { get; init; } = MatchPlanOutcomes.ShowEmptyOutcomeStructures;

    public bool IncludeQrCode { get; init; } = true;

    public bool IncludeRankingTable { get; init; }
}

public enum MatchPlanDateFormat
{
    NoDate,
    DateOnly,
    DateAndDayOfWeek,
    DateAndDayOfWeekAndTimeOfDay
}

public enum MatchPlanOutcomes
{
    HideOutcomeStructures,
    ShowEmptyOutcomeStructures,
    ShowOutcomeStructuresWithOutcomes
}
