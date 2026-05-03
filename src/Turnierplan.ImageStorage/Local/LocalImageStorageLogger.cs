using Microsoft.Extensions.Logging;

namespace Turnierplan.ImageStorage.Local;

internal sealed partial class LocalImageStorageLogger(ILogger<LocalImageStorage> logger)
{
    [LoggerMessage(LogLevel.Information, "Using the following directory for local image storage: '{LocalImageStoragePath}'", EventId = 1)]
    public partial void UsingDirectoryForStorage(string localImageStoragePath);

    [LoggerMessage(LogLevel.Critical, "The directory for local image storage does not exist and could not be created.", EventId = 2)]
    public partial void RootDirectoryDoesNotExistAndCouldNotBeCreated();

    [LoggerMessage(LogLevel.Critical, "The directory for the image could not be created: '{ImageDirectory}'.", EventId = 3)]
    public partial void ImageDirectoryCouldNotBeCreated(string imageDirectory);

    [LoggerMessage(LogLevel.Debug, "Writing image to: {FilePath}", EventId = 4)]
    public partial void WritingImageToPath(string filePath);

    [LoggerMessage(LogLevel.Error, "Failed to save image to file to '{FilePath}'.", EventId = 5)]
    public partial void FailedToWriteImageToPath(string filePath, Exception exception);

    [LoggerMessage(LogLevel.Error, "Failed to delete image file '{FilePath}'.", EventId = 6)]
    public partial void FailedToDeleteImageFile(string filePath, Exception exception);

    [LoggerMessage(LogLevel.Critical, "A previous initialization error of local image storage will cause the migrations to be skipped.", EventId = 7)]
    public partial void SkippingMigration();

    [LoggerMessage(LogLevel.Information, "The directory structure is up to date, no migration is needed.", EventId = 8)]
    public partial void DirectoryStructureIsUpToDate();

    [LoggerMessage(LogLevel.Information, "A migration will be attempted because at least one file in the storage directory matches the old file pattern.", EventId = 9)]
    public partial void MigrationWillBeAttempted();

    [LoggerMessage(LogLevel.Warning, "The image provider returned no images even though the image storage folder contains old image files - migration is skipped.", EventId = 10)]
    public partial void ImageProviderReturnedNoImages();

    [LoggerMessage(LogLevel.Warning, "Encountered a file in the image storage directory which does not match the file name pattern: '{ImagePath}'", EventId = 11)]
    public partial void EncounteredFileWhichDoesNotMatchPattern(string imagePath);

    [LoggerMessage(LogLevel.Warning, "Encountered a file in the image storage directory which does not have a valid GUID in the file name: '{ImagePath}'", EventId = 12)]
    public partial void EncounteredFileThatHasNoValidGuidInName(string imagePath);

    [LoggerMessage(LogLevel.Warning, "Encountered a file in the image storage directory for which there exists no corresponding entry from the image provider: '{ImagePath}'", EventId = 13)]
    public partial void EncounteredFileWithoutCorrespondingImageFromProvider(string imagePath);

    [LoggerMessage(LogLevel.Warning, "Encountered an already existing target file '{TargetPath}' while migrating '{SourcePath}' - the move operation will be attempted again.", EventId = 14)]
    public partial void EncounteredTargetFileThatAlreadyExists(string targetPath, string sourcePath);

    [LoggerMessage(LogLevel.Information, "Successfully moved image file '{SourcePath}' to '{TargetPath}'.", EventId = 15)]
    public partial void SuccessfullyMovedFileTo(string sourcePath, string targetPath);

    [LoggerMessage(LogLevel.Critical, "An unexpected exception occurred while trying to run local image storage migrations.", EventId = 16)]
    public partial void ExceptionOccurredDuringMigration(Exception exception);
}
