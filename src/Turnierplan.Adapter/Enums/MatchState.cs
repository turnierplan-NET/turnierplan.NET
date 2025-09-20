namespace Turnierplan.Adapter.Enums;

/// <summary>
/// Represents the state of a match.
/// </summary>
public enum MatchState
{
    /// <summary>
    /// The match has not started yet.
    /// </summary>
    NotStarted,

    /// <summary>
    /// The match is currently being played, i.e. a "LIVE" outcome was published.
    /// </summary>
    CurrentlyPlaying,

    /// <summary>
    /// The match is finished, i.e. a "final" result was published.
    /// </summary>
    Finished
}
