namespace Turnierplan.ImageStorage;

internal interface IImageStorageMigration
{
    Task MigrateAsync(CancellationToken cancellationToken);
}
