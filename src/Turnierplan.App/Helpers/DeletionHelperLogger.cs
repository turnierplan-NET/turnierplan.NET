namespace Turnierplan.App.Helpers;

internal sealed partial class DeletionHelperLogger
{
    private readonly ILogger<DeletionHelper> _logger;

    public DeletionHelperLogger(ILogger<DeletionHelper> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Error, "Image with id '{ImageId}' was successfully deleted from image storage but the deletion from the database failed.", EventId = 100)]
    public partial void ImageDeletedButDatabaseRemovalFailed(Exception exception, long imageId);

    [LoggerMessage(LogLevel.Error, "Failed to delete image with id '{ImageId}' from image storage while deleting organization with id '{OrganizationId}'.", EventId = 101)]
    public partial void ImageDeletionFromStorageFailed(long imageId, long organizationId);
}
