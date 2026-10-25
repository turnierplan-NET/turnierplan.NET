namespace Turnierplan.Core.Tournament;

public enum MatchOutcomeType
{
    // Note: Don't change enum values (DB serialization)

    Standard = 1,
    AfterOvertime = 2,
    AfterPenalties = 3,
    SpecialScoring = 4
}
