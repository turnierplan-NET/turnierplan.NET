namespace Turnierplan.Server.Domain;

internal interface IEntityWithPublicId
{
    PublicId PublicId { get; }
}
