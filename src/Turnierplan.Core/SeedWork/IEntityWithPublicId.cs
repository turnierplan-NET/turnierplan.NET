namespace Turnierplan.Core.SeedWork;

public interface IEntityWithPublicId
{
    PublicId.PublicId PublicId { get; }
}
