namespace Turnierplan.Core.Entity;

public interface IEntityWithPublicId
{
    PublicId.PublicId PublicId { get; }
}
