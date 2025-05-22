namespace Turnierplan.App.Models;

public sealed record TeamDto
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required bool OutOfCompetition { get; init; }

    public required bool HasPaidEntryFee { get; init; }

    public DateTime? EntryFeePaidAt { get; init; }
}
