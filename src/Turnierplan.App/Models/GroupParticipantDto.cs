namespace Turnierplan.App.Models;

public sealed record GroupParticipantDto
{
    public required int TeamId { get; init; }

    public required int Priority { get; init; }

    public required TeamGroupStatisticsDto Statistics { get; init; }
}
