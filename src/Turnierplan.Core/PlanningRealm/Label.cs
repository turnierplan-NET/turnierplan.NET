using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class Label : Entity<long>
{
    internal Label(long id, string name, bool isSystemCreated, string colorCode)
    {
        Id = id;
        Name = name;
        IsSystemCreated = isSystemCreated;
        ColorCode = colorCode;
    }

    internal Label(string name, bool isSystemCreated)
    {
        Id = 0;
        Name = name;
        IsSystemCreated = isSystemCreated;
        ColorCode = "aaaaaa";
    }

    public override long Id { get; protected set; }

    public string Name { get; set; }

    public bool IsSystemCreated { get; }

    public string ColorCode { get; set; }
}
