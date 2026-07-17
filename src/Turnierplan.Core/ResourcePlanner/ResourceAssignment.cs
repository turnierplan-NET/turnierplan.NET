namespace Turnierplan.Core.ResourcePlanner;

public sealed class ResourceAssignment
{
    internal ResourceAssignment(ResourceAssignmentState state)
    {
        State = state;
    }

    internal ResourceAssignment(ResourceGroup resourceGroup, Resource resource)
    {
        ResourceGroup = resourceGroup;
        Resource = resource;
        State = ResourceAssignmentState.Confirmed; // Todo: What's the default state for a newly assigned resource?
    }

    public ResourceGroup ResourceGroup { get; internal set; } = null!;

    public Resource Resource { get; internal set; } = null!;

    public ResourceAssignmentState State { get; set; }
}
