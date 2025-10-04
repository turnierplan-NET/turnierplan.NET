namespace Turnierplan.App.Models;

public sealed record ApplicationDto
{
    public required long Id { get; init; }

    public required long? SourceLinkId { get; init; }

    public required int Tag { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required string Notes { get; init; }

    public required string Contact { get; init; }

    public required string? ContactEmail { get; init; }

    public required string? ContactTelephone { get; init; }

    public required string? Comment { get; init; }

    public required ApplicationTeamDto[] Teams { get; init; }
}
