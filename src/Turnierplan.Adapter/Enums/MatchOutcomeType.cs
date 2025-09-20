namespace Turnierplan.Adapter.Enums;

/// <summary>
/// The match outcome type gives an indication of how a match ended.
/// </summary>
public enum MatchOutcomeType
{
    /// <summary>
    /// Indicates that the match ended after regular time without penalty shootout.
    /// </summary>
    Standard,

    /// <summary>
    /// Indicates that the match ended after over time but without penalty shootout.
    /// </summary>
    AfterOvertime,

    /// <summary>
    /// Indicates that the match ended after penalty shootout.
    /// </summary>
    AfterPenalties,

    /// <summary>
    /// Indicates that the match was not completed normally. Rather, the score is the result of some extraordinary circumstance.
    /// </summary>
    SpecialScoring
}
