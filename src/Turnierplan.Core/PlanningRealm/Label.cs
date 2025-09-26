using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class Label : Entity<long>
{
    internal Label(long id, string name, string colorCode)
    {
        Id = id;
        Name = name;
        ColorCode = colorCode;
    }

    internal Label(string name)
    {
        Id = 0;
        Name = name;
        ColorCode = "aaaaaa";
    }

    public override long Id { get; protected set; }

    public string Name { get; set; }

    public string ColorCode { get; set; }
}
