using Turnierplan.Adapter.Models;

namespace Turnierplan.Adapter.Enums;

/// <summary>
/// Represents the visibility of a <see cref="Tournament"/>.
/// </summary>
public enum Visibility
{
    /// <summary>
    /// The tournament is only accessible by an authenticated user or API key.
    /// </summary>
    Private = 1,

    /// <summary>
    /// The tournament can be viewed by who has the correct link with the tournament ID.
    /// </summary>
    Public = 2
}
