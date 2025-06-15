using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.User;

public sealed class Role : Entity<Guid> // TODO: Delete
{
    internal Role(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public override Guid Id { get; protected set; }

    public string Name { get; }
}
