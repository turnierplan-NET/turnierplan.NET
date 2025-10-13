namespace Turnierplan.Core.PlanningRealm;

public enum ApplicationChangeLogProperty
{
    // Note: Don't change enum values (DB serialization)

    PreviousValue = 1,
    NewValue = 2,
    LabelId = 3,
    LabelName = 4,
    LabelColorCode = 5,
    TeamName = 6,
    TournamentName = 7
}
