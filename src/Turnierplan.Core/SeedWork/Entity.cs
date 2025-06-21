namespace Turnierplan.Core.SeedWork;

public abstract class Entity;

public abstract class Entity<TIdentifier> : Entity
{
    public abstract TIdentifier Id { get; protected set; }
}
