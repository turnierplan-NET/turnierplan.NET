using Microsoft.Extensions.Logging;

namespace Turnierplan.ImageStorage.Local;

internal sealed partial class LocalImageStorageLogger
{
    private readonly ILogger<LocalImageStorage> _logger;

    public LocalImageStorageLogger(ILogger<LocalImageStorage> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Information, "Using the following directory for local image storage: '{LocalImageStoragePath}'", EventId = 101)]
    public partial void UsingDirectoryForLocalImageStorage(string localImageStoragePath);

    [LoggerMessage(LogLevel.Critical, "The directory for local image storage does not exist and could not be created.", EventId = 102)]
    public partial void DirectoryCouldNotBeCreated();

    [LoggerMessage(LogLevel.Critical, "The directory for the image could not be created: '{ImageDirectory}'.", EventId = 103)]
    public partial void DirectoryForImageCouldNotBeCreated(string imageDirectory);

    [LoggerMessage(LogLevel.Debug, "Writing image to: {FilePath}", EventId = 104)]
    public partial void WritingImageToFile(string filePath);

    [LoggerMessage(LogLevel.Error, "Failed to save image to file to '{FilePath}'.", EventId = 105)]
    public partial void FailedToWriteImage(Exception exception, string filePath);

    [LoggerMessage(LogLevel.Error, "Failed to delete image file '{FilePath}'.", EventId = 106)]
    public partial void FailedToDeleteImage(Exception exception, string filePath);
}
