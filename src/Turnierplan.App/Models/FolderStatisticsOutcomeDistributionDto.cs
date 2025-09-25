namespace Turnierplan.App.Models;

public sealed record FolderStatisticsOutcomeDistributionDto
{
    internal FolderStatisticsOutcomeDistributionDto(FolderStatisticsOutcomeDto outcome, int count)
    {
        Outcome = outcome;
        Count = count;
    }

    public FolderStatisticsOutcomeDto Outcome { get; }

    public int Count { get; }
}
