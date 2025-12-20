namespace Turnierplan.Adapter.Enums;

/// <summary>
/// Represents the reason of team selection for a single ranking.
/// </summary>
public enum RankingReason
{
    /// <summary>
    /// The ranking is defined by the winner of the final match.
    /// </summary>
    WinnerOfFinal,

    /// <summary>
    /// The ranking is defined by the loser of the final match.
    /// </summary>
    LoserOfFinal,

    /// <summary>
    /// The ranking is defined by the winner of the 3rd place playoff match.
    /// </summary>
    WinnerOfThirdPlacePlayoff,

    /// <summary>
    /// The ranking is defined by the loser of the 3rd place playoff match.
    /// </summary>
    LoserOfThirdPlacePlayoff,

    /// <summary>
    /// The ranking is defined by the winner of any additional (5th, 7th, ...) playoff match.
    /// </summary>
    WinnerOfAdditionalPlayoff,

    /// <summary>
    /// The ranking is defined by the loser of any additional (5th, 7th, ...) playoff match.
    /// </summary>
    LoserOfAdditionalPlayoff,

    /// <summary>
    /// The ranking is defined by a team which has qualified for the semi-finals but did not qualify for any further
    /// round or playoff match. If multiple rankings have this reason assigned, the teams will be sorted amongst each
    /// other by their relative success in the semi-final matches and then by their group statistics.
    /// </summary>
    QualifiedForSemiFinals,

    /// <summary>
    /// The ranking is defined by a team which has qualified for the quarter-finals but did not qualify for any further
    /// round or playoff match. If multiple rankings have this reason assigned, the teams will be sorted amongst each
    /// other by their relative success in the quarter-final matches and then by their group statistics.
    /// </summary>
    QualifiedForQuarterFinals,

    /// <summary>
    /// The ranking is defined by a team which has qualified for the eighth-finals but did not qualify for any further
    /// round or playoff match. If multiple rankings have this reason assigned, the teams will be sorted amongst each
    /// other by their relative success in the eighth-final matches and then by their group statistics.
    /// </summary>
    QualifiedForEighthFinals,

    /// <summary>
    /// The ranking is defined by a team which has qualified any finals round broader than eighth-finals (such as 16th, 32nd, ...)
    /// but did not qualify for any further round or playoff match. If multiple rankings have this reason assigned, the teams will
    /// be sorted amongst each other by their relative success in that finals round's matches and then by their group statistics.
    /// </summary>
    QualifiedForBroaderFinals,

    /// <summary>
    /// The ranking is defined by a team which has not qualified for any finals round at all. If multiple rankings have this
    /// reason assigned, the teams will be sorted amongst each other by their group statistics.
    /// </summary>
    /// <seealso cref="RankingViaGroupResults" />
    NotQualifiedForFinals,

    /// <summary>
    /// Used instead of <see cref="NotQualifiedForFinals"/> if the tournament does not contain any finals/playoff matches.
    /// </summary>
    RankingViaGroupResults,

    /// <summary>
    /// The ranking is defined manually.
    /// </summary>
    ManuallyChanged
}
