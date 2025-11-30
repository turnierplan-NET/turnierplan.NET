using Turnierplan.Core.Entity;

namespace Turnierplan.Core.PlanningRealm;

public sealed class Label : Entity<long>
{
    internal Label(long id, string name, string description, string colorCode)
    {
        Id = id;
        Name = name;
        Description = description;
        ColorCode = colorCode;
    }

    internal Label(string name)
    {
        Id = 0;
        Name = name;
        Description = string.Empty;
        ColorCode = "aaaaaa";
    }

    public override long Id { get; protected set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ColorCode { get; set; }
}
