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

    // TODO: Additional reason for tournament with no finals round?
    NotQualifiedForFinals,

    // TODO: This enum member will be used in https://github.com/turnierplan-NET/turnierplan.NET/issues/247
    ManuallyChanged
}
