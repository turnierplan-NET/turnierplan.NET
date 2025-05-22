namespace Turnierplan.Core.Tournament;

public enum FinalsRoundOrder
{
    // Note: Don't change enum values (DB serialization)

    FinalOnly = 0,
    SemiFinals = 1,
    QuarterFinals = 2,
    EighthFinals = 3
}
