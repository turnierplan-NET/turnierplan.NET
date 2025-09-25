namespace Turnierplan.App.Models;

public sealed record FolderStatisticsOutcomeDistributionDto
{
    public required FolderStatisticsOutcomeDto Outcome { get; init;  }

    public required int Count { get; init;  }
}
