using Turnierplan.Core.Entity;

namespace Turnierplan.Core.ResourcePlanner;

public sealed class ResourceGroup : Entity<long>
{
    // todo: List<ResourceAssignment> ResourceAssignments = [];
    // todo: List<ExternalViewAssignment> ExternalViewAssignment = [];

    public override long Id { get; protected set; }

    public ResourcePlanner ResourcePlanner { get; internal set; } = null!;

    public ResourceGroupType Type { get; }

    public DateTime? Start { get; } // only for type=shift

    public DateTime? End { get; } // only for type=shift

    public string Name { get; } // mandatory for type=general

    public string Description { get; } // can be empty?
}

public enum ResourceGroupType
{
    // Note: Don't change enum values (DB serialization)

    Shift = 1, // todo: name?
    General = 2, // todo: name?
    // ...todo: others?
}

public sealed class Resource : Entity<long>
{
    public override long Id { get; protected set; }

    public ResourcePlanner ResourcePlanner { get; internal set; } = null!;

    public ResourceType Type { get; }

    public string Name { get; }

    public string Notes { get; } // can be empty?
}

public enum ResourceType
{
    // Note: Don't change enum values (DB serialization)

    Personnel = 1, // todo: name?
    Commodity = 2, // todo: name?
    // ...todo: others?
}

public sealed class ResourceAssignment // analogous to group participant
{
    public ResourceGroup ResourceGroup { get; internal set; } = null!;

    public Resource Resource { get; internal set; } = null!;

    public ResourceAssignmentState State { get; }
}

public enum ResourceAssignmentState
{
    Proposed = 1,
    Requested = 2,
    Confirmed = 3,
    // ...todo: others?
}

public sealed class ResourcePlannerView : Entity<long>
{
    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; } // (for externally visible URL)

    public ResourcePlanner ResourcePlanner { get; internal set; } = null!;

    public bool IsActive { get; set; } // (display 404 externally when false)

    public bool DisplayAllGroups { get; set; } // (if false, assigned groups are configured via Group.ExternalViewAssignments)

    // todo: Valid until?
    // todo: Primary/Secondary Images?
}

// maybe not necessary if no further properties are needed... then EF can implicitly create the join table
public sealed class ExternalViewAssignment // analogous to group participant
{
    public ResourcePlannerView View { get; internal set; } = null!;

    public ResourceGroup ResourceGroup { get; internal set; } = null!;
}
