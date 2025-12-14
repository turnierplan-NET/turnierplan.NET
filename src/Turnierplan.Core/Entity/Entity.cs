namespace Turnierplan.Core.Entity;

public abstract class Entity<TIdentifier>
{
    public abstract TIdentifier Id { get; protected set; }
}
