namespace Turnierplan.Core.SeedWork;

public abstract class Entity<TIdentifier>
{
    public abstract TIdentifier Id { get; protected set; }
}
