namespace Turnierplan.Core.ResourcePlanner;

public sealed class Column
{
    // - Type => "Day" or "General"
    // - OrderIndex
    // - Name (optional)
    // - Description (optional)

    // if type == "Day":
    // - Date (yyyy/mm/dd)

    List<Group> Groups = [];
}

public sealed class Group
{
    // - Type => "Shift" or "General"
    // - Name (optional / mandatory if type == "General")
    // - Description (optional)

    // if type == "Shift":
    //  - Start Timestamp
    //  - End Timestamp

    List<ResourceAssignment> ResourceAssignments = [];
    List<ExternalViewAssignment> ExternalViewAssignment = [];
}

public sealed class Resource
{
    // - Type => "Personnel" or "Commodity"
    // - Name
    // - Description/Notes (optional)
}

public sealed class ResourceAssignment
{
    // - State => "Proposed" / "Requested" / "Confirmed" / "..." [custom?]

    Group Group;
    Resource Resource;
}

public sealed class ExternalView
{
    // - Public ID (for externally visible URL)
    // - IsEnabled (display 404 externally when false)
    // - DisplayAllGroups (if false, assigned groups are configured via Group.ExternalViewAssignments)
}

public sealed class ExternalViewAssignment
{
    Group Group;
    ExternalView ExternalView;
}
