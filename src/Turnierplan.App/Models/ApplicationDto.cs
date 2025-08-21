namespace Turnierplan.App.Models;

public sealed record ApplicationDto
{
    public required long Id { get; init; }

    public required int Tag { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required string Notes { get; set; }

    public required string Contact { get; set; }

    public required string? ContactEmail { get; set; }

    public required string? ContactTelephone { get; set; }

    public required string? Comment { get; set; }

    public required ApplicationTeamDto[] Teams { get; init; }
}
