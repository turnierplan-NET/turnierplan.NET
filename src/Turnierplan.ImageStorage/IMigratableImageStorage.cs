namespace Turnierplan.ImageStorage;

public interface IMigratableImageStorage
{
    Task MigrateAsync(IImageProvider imageProvider, CancellationToken cancellationToken);
}
