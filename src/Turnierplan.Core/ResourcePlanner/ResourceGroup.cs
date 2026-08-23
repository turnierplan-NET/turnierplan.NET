using Turnierplan.Core.Entity;
using Turnierplan.Core.Exceptions;

namespace Turnierplan.Core.ResourcePlanner;

public sealed class ResourceGroup : Entity<long>
{
    internal readonly List<ResourceAssignment> _resourceAssignments = [];

    private string _name;
    private string? _notes;

    internal ResourceGroup(long id, string name, string? notes, DateTime? start, DateTime? end)
    {
        Id = id;
        Start = start;
        End = end;

        _name = name;
        _notes = notes;
    }

    internal ResourceGroup(ResourcePlanner resourcePlanner, string name, string? notes, DateTime? start, DateTime? end)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new TurnierplanException($"{nameof(Name)} must be a non-empty string.");
        }

        if (notes is not null && notes.Trim().Length == 0)
        {
            throw new TurnierplanException($"{nameof(Notes)} must be null or a non-empty string.");
        }

        Id = 0;
        ResourcePlanner = resourcePlanner;
        Start = start;
        End = end;

        _name = name;
        _notes = notes;
    }

    public override long Id { get; protected set; }

    public ResourcePlanner ResourcePlanner { get; internal set; } = null!;

    public string Name
    {
        get => _name;
        set
        {
            var trimmed = value.Trim();

            if (string.IsNullOrEmpty(trimmed))
            {
                throw new TurnierplanException($"{nameof(Name)} must be a non-empty string.");
            }

            _name = trimmed;
        }
    }

    public string? Notes
    {
        get => _notes;
        set
        {
            var trimmed = value?.Trim();

            if (trimmed is not null && trimmed.Length == 0)
            {
                throw new TurnierplanException($"{nameof(Notes)} must be null or a non-empty string");
            }

            _notes = trimmed;
        }
    }

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    public IReadOnlyList<ResourceAssignment> ResourceAssignments => _resourceAssignments.AsReadOnly();

    public void AssignResource(Resource resource)
    {
        if (resource.ResourcePlanner != ResourcePlanner)
        {
            throw new TurnierplanException("Cannot assign resource from another resource planner.");
        }

        if (_resourceAssignments.Any(x => x.Resource == resource))
        {
            throw new TurnierplanException("The specified resource is already assigned to this resource group.");
        }

        _resourceAssignments.Add(new ResourceAssignment(this, resource));
    }

    public Resource AssignResource(ResourceType type, string name, string? notes)
    {
        var resource = new Resource(ResourcePlanner, type, name, notes);

        _resourceAssignments.Add(new ResourceAssignment(this, resource));

        return resource;
    }

    public void UnassignResource(ResourceAssignment assignment)
    {
        _resourceAssignments.Remove(assignment);
    }
}
