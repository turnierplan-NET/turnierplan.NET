using Microsoft.Extensions.Logging;

namespace Turnierplan.ImageStorage.S3;

internal sealed partial class S3ImageStorageLogger
{
    private readonly ILogger<S3ImageStorage> _logger;

    public S3ImageStorageLogger(ILogger<S3ImageStorage> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Error, "Failed to upload image '{ObjectKey}' to S3. Result status code: {StatusCode}", EventId = 100)]
    public partial void FailedToUploadImage(string objectKey, int statusCode);

    [LoggerMessage(LogLevel.Error, "Failed to upload image '{ObjectKey}' to S3 because of an exception.", EventId = 101)]
    public partial void FailedToUploadImage(Exception exception, string objectKey);

    [LoggerMessage(LogLevel.Error, "Failed to read image '{ObjectKey}' from S3. Result status code: {StatusCode}", EventId = 102)]
    public partial void FailedToReadImage(string objectKey, int statusCode);

    [LoggerMessage(LogLevel.Error, "Failed to delete image '{ObjectKey}' from S3 because of an exception.", EventId = 103)]
    public partial void FailedToDeleteImage(Exception exception, string objectKey);
}
