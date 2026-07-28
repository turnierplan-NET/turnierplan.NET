using Turnierplan.Core.Entity;
using Turnierplan.Core.Exceptions;

namespace Turnierplan.Core.ResourcePlanner;

// Todo: Add 'valid until' date?
// Todo: Add option to show primary/secondary image?
public sealed class ResourcePlannerView : Entity<long>
{
    internal readonly List<ResourceGroup> _resourceGroups = [];

    internal ResourcePlannerView(long id, PublicId.PublicId publicId, bool isActive, bool displayAllGroups)
    {
        Id = id;
        PublicId = publicId;
        IsActive = isActive;
        DisplayAllGroups = displayAllGroups;
    }

    internal ResourcePlannerView(ResourcePlanner resourcePlanner, bool isActive, bool displayAllGroups)
    {
        Id = 0;
        PublicId = new PublicId.PublicId();
        ResourcePlanner = resourcePlanner;
        IsActive = isActive;
        DisplayAllGroups = displayAllGroups;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public ResourcePlanner ResourcePlanner { get; internal set; } = null!;

    public bool IsActive { get; set; }

    public bool DisplayAllGroups { get; set; }

    public IReadOnlyList<ResourceGroup> ResourceGroups => _resourceGroups.AsReadOnly();

    public void AddResourceGroup(ResourceGroup resourceGroup)
    {
        if (resourceGroup.ResourcePlanner != ResourcePlanner)
        {
            throw new TurnierplanException("Cannot assign resource group from another resource planner.");
        }

        if (DisplayAllGroups)
        {
            throw new TurnierplanException("Cannot assign resource group because this resource planner view is currently configured to display all resource groups.");
        }

        if (_resourceGroups.Contains(resourceGroup))
        {
            throw new TurnierplanException("The specified resource group is already included in this resource planner view.");
        }

        _resourceGroups.Add(resourceGroup);
    }

    public void RemoveResourceGroup(ResourceGroup resourceGroup)
    {
        _resourceGroups.Remove(resourceGroup);
    }
}
