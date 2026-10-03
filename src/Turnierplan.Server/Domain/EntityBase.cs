namespace Turnierplan.Server.Domain;

internal abstract class EntityBase<TIdentifier>
{
    public abstract TIdentifier Id { get; protected set; }
}
