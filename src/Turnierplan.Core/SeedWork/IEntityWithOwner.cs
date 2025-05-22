namespace Turnierplan.Core.SeedWork;

public interface IEntityWithOwner
{
    public Guid OwnerId { get; }
}
