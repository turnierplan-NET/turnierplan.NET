namespace Turnierplan.Core.PlanningRealm;

public enum ApplicationChangeLogProperty
{
    // Note: Don't change enum values (DB serialization)

    PreviousValue = 1,
    NewValue = 2,
    LabelName = 3,
    LabelColorCode = 4,
    TeamName = 5
}
