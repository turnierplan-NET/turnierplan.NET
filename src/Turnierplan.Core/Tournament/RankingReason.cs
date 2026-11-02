namespace Turnierplan.Core.Tournament;

public enum RankingReason
{
    WinnerOfFinal = 1,
    LoserOfFinal,

    WinnerOfThirdPlacePlayoff,
    LoserOfThirdPlacePlayoff,

    WinnerOfAdditionalPlayoff,
    LoserOfAdditionalPlayoff,

    QualifiedForSemiFinals,
    QualifiedForQuarterFinals,
    QualifiedForEighthFinals,
    QualifiedForBroaderFinals,

    NotQualifiedForFinals,
    RankingViaGroupResults,

    ManuallyChanged
}
