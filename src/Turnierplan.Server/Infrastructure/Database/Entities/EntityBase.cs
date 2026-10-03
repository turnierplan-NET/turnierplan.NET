namespace Turnierplan.Server.Infrastructure.Database.Entities;

internal abstract class EntityBase<TIdentifier>
{
    public abstract TIdentifier Id { get; protected set; }
}
