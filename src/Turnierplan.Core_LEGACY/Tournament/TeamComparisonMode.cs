namespace Turnierplan.Core.Tournament;

public enum TeamComparisonMode
{
    // Note: Don't change enum values (DB serialization)

    ByPoints = 1,
    ByScoreDifference = 2,
    ByScore = 3,
    ByDirectComparison = 4
}
