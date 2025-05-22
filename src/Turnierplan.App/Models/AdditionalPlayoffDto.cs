namespace Turnierplan.App.Models;

public sealed record AdditionalPlayoffDto
{
    public required int PlayoffPosition { get; init; }

    public required string TeamSelectorA { get; init; }

    public required string TeamSelectorB { get; init; }
}
