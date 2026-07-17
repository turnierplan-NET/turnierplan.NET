using Turnierplan.Core.Entity;
using Turnierplan.Core.Exceptions;

namespace Turnierplan.Core.ResourcePlanner;

public sealed class Resource : Entity<long>
{
    private string _name;
    private string? _notes;

    internal Resource(long id, ResourceType type, string name, string? notes)
    {
        Id = id;
        Type = type;
        _name = name;
        _notes = notes;
    }

    public Resource(ResourcePlanner resourcePlanner, ResourceType type, string name, string? notes)
    {
        Id = 0;
        ResourcePlanner = resourcePlanner;
        Type = type;
        _name = name;
        _notes = notes;
    }

    public override long Id { get; protected set; }

    public ResourcePlanner ResourcePlanner { get; internal set; } = null!;

    public ResourceType Type { get; set; }

    public string Name
    {
        get => _name;
        set => _name = value.Trim();
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
}
