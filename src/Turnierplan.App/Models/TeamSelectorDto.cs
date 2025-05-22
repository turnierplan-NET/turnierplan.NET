namespace Turnierplan.App.Models;

public sealed record TeamSelectorDto
{
    public required string Key { get; init; }

    public required string Localized { get; init; }
}
