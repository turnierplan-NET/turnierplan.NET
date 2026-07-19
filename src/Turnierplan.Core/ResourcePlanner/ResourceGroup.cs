using Turnierplan.Core.Entity;
using Turnierplan.Core.Exceptions;

namespace Turnierplan.Core.ResourcePlanner;

public sealed class ResourceGroup : Entity<long>
{
    internal readonly List<ResourceAssignment> _resourceAssignments = [];

    private string? _name;
    private string? _description;

    internal ResourceGroup(long id, string? name, string? description, ResourceGroupType type, DateTime? start, DateTime? end)
    {
        Id = id;
        Type = type;
        Start = start;
        End = end;

        _name = name;
        _description = description;
    }

    internal ResourceGroup(ResourcePlanner resourcePlanner, string? name, string? description, ResourceGroupType type, DateTime? start, DateTime? end)
    {
        var isWorkshift = type is ResourceGroupType.Workshift;
        var isGeneral = type is ResourceGroupType.General;

        if (!isWorkshift && !isGeneral)
        {
            throw new TurnierplanException($"Invalid resource type: '{type}'");
        }

        if (isWorkshift)
        {
            if (!start.HasValue || end.HasValue)
            {
                throw new TurnierplanException($"Start and end time must be set if the type is '{ResourceGroupType.Workshift}'");
            }
        }

        if (isGeneral)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new TurnierplanException($"Name must be a non-empty string if the type is '{ResourceGroupType.General}'");
            }
        }

        Id = 0;
        ResourcePlanner = resourcePlanner;
        Type = type;
        Start = isWorkshift ? start : null;
        End = isWorkshift ? end : null;

        _name = name;
        _description = description;
    }

    public override long Id { get; protected set; }

    public ResourcePlanner ResourcePlanner { get; internal set; } = null!;

    public string? Name
    {
        get => _name;
        set
        {
            var trimmed = value?.Trim();

            if (Type is ResourceGroupType.General)
            {
                if (string.IsNullOrEmpty(trimmed))
                {
                    throw new TurnierplanException($"If the {nameof(Type)} is {ResourceGroupType.General}, the {nameof(Name)} must be a non-empty string.");
                }
            }
            else if (trimmed is not null && trimmed.Length == 0)
            {
                throw new TurnierplanException($"{nameof(Name)} must be null or a non-empty string");
            }

            _name = trimmed;
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            var trimmed = value?.Trim();

            if (trimmed is not null && trimmed.Length == 0)
            {
                throw new TurnierplanException($"{nameof(Description)} must be null or a non-empty string");
            }

            _description = trimmed;
        }
    }

    public ResourceGroupType Type { get; }

    public DateTime? Start
    {
        get;
        set
        {
            if (Type is ResourceGroupType.Workshift && value is null)
            {
                throw new TurnierplanException($"Start time may not be set to null if type is {ResourceGroupType.Workshift}.");
            }

            if (Type is not ResourceGroupType.Workshift && value is not null)
            {
                throw new TurnierplanException($"Start time may not be set to a non-null value if type is not {ResourceGroupType.Workshift}.");
            }

            field = value;
        }
    }

    public DateTime? End
    {
        get;
        set
        {
            if (Type is ResourceGroupType.Workshift && value is null)
            {
                throw new TurnierplanException($"End time may not be set to null if type is {ResourceGroupType.Workshift}.");
            }

            if (Type is not ResourceGroupType.Workshift && value is not null)
            {
                throw new TurnierplanException($"End time may not be set to a non-null value if type is not {ResourceGroupType.Workshift}.");
            }

            field = value;
        }
    }

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

    public void UnassignResource(ResourceAssignment assignment)
    {
        _resourceAssignments.Remove(assignment);
    }
}
