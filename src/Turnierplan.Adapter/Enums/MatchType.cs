namespace Turnierplan.Adapter.Enums;

/// <summary>
/// Represents the type of match.
/// </summary>
public enum MatchType
{
    /// <summary>
    /// A group match between two group participants.
    /// </summary>
    GroupMatch,

    /// <summary>
    /// A knockout (KO) match which is specifically not the final match, such as semi-final or quarter-final.
    /// </summary>
    NonFinalKnockout,

    /// <summary>
    /// A playoff match which is specifically not the 3rd position playoff.
    /// </summary>
    AdditionalPlayoff,

    /// <summary>
    /// The playoff match for 3rd place.
    /// </summary>
    ThirdPlacePlayoff,

    /// <summary>
    /// The final match or in other words the playoff match for 1st place.
    /// </summary>
    Final
}
