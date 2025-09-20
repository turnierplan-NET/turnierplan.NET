namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents a team selector which can be used to uniquely select a specific team in the context of a <see cref="Tournament"/>.
/// </summary>
public sealed record TeamSelector
{
    /// <summary>
    /// The internal representation of the team selector.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// The team selector as a human-readable, localized value.
    /// </summary>
    public required string Localized { get; init; }
}
