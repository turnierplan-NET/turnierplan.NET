using Turnierplan.Core.Entity;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.ResourcePlanner;

public sealed class ResourcePlanner : Entity<long>, IEntityWithRoleAssignments<ResourcePlanner>, IEntityWithOrganization
{
    internal readonly List<RoleAssignment<ResourcePlanner>> _roleAssignments = [];
    internal readonly List<ResourceGroup> _resourceGroups = [];
    internal readonly List<ResourcePlannerView> _resourcePlannerViews = [];

    public ResourcePlanner(Organization.Organization organization, string name)
    {
        organization._resourcePlanners.Add(this);

        Id = 0;
        PublicId = new PublicId.PublicId();
        Organization = organization;
        CreatedAt = DateTime.UtcNow;
        Name = name;
    }

    internal ResourcePlanner(long id, PublicId.PublicId publicId, DateTime createdAt, string name)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    public IReadOnlyList<RoleAssignment<ResourcePlanner>> RoleAssignments => _roleAssignments.AsReadOnly();

    public IReadOnlyList<ResourceGroup> ResourceGroups => _resourceGroups.AsReadOnly();

    public IReadOnlyList<ResourcePlannerView> ResourcePlannerViews => _resourcePlannerViews.AsReadOnly();

    public DateTime CreatedAt { get; }

    public string Name { get; }

    public RoleAssignment<ResourcePlanner> AddRoleAssignment(Role role, Principal principal)
    {
        var roleAssignment = new RoleAssignment<ResourcePlanner>(this, role, principal);
        _roleAssignments.Add(roleAssignment);

        return roleAssignment;
    }

    public void RemoveRoleAssignment(RoleAssignment<ResourcePlanner> roleAssignment)
    {
        _roleAssignments.Remove(roleAssignment);
    }

    public ResourceGroup AddResourceGroup(string? name, string? description, ResourceGroupType type, DateTime? start, DateTime? end)
    {
        var resourceGroup = new ResourceGroup(this, name, description, type, start, end);
        _resourceGroups.Add(resourceGroup);

        return resourceGroup;
    }

    public void RemoveResourceGroup(ResourceGroup resourceGroup)
    {
        _resourceGroups.Remove(resourceGroup);
    }

    public ResourcePlannerView AddResourcePlannerView(bool isActive, bool displayAllGroups)
    {
        var view = new ResourcePlannerView(this, isActive, displayAllGroups);
        _resourcePlannerViews.Add(view);

        return view;
    }

    public void RemoveResourcePlannerView(ResourcePlannerView resourcePlannerView)
    {
        _resourcePlannerViews.Remove(resourcePlannerView);
    }
}
