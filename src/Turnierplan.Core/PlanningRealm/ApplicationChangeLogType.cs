namespace Turnierplan.Core.PlanningRealm;

public enum ApplicationChangeLogType
{
    // Note: Don't change enum values (DB serialization)

    NotesChanged = 1,
    ContactChanged = 2,
    ContactEmailChanged = 3,
    ContactTelephoneChanged = 4,
    CommentChanged = 5,
    TeamAdded = 6,
    TeamRenamed = 7
}
