using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage.Local;

internal sealed partial class LocalImageStorage : ImageStorageBase, IMigratableImageStorage
{
    private readonly LocalImageStorageLogger _logger;
    private readonly string _storagePath;
    private readonly SemaphoreSlim _migrationSemaphore = new(1, 1);
    private readonly bool _skipMigration;

    public LocalImageStorage(ILogger<LocalImageStorage> logger, IOptions<LocalImageStorageOptions> options)
    {
        _logger = new LocalImageStorageLogger(logger);

        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.StoragePath);
        _storagePath = Path.GetFullPath(options.Value.StoragePath);

        _logger.UsingDirectoryForStorage(_storagePath);

        Directory.CreateDirectory(_storagePath);

        if (!Directory.Exists(_storagePath))
        {
            _logger.RootDirectoryDoesNotExistAndCouldNotBeCreated();
            _skipMigration = true;
        }
    }

    public override string GetFullImageUrl(Image image)
    {
        return $"/images/{image.CreatedAt.Year}/{image.CreatedAt.Month:D2}/{GetImageFileName(image)}";
    }

    public override Task<bool> SaveImageAsync(Image image, MemoryStream imageData)
    {
        var filePath = GetImageFullPath(image);

        try
        {
            var imageDirectory = Directory.GetParent(filePath);

            if (imageDirectory is not null && !imageDirectory.Exists)
            {
                var imageDirectoryPath = imageDirectory.FullName;

                Directory.CreateDirectory(imageDirectoryPath);

                if (!Directory.Exists(imageDirectoryPath))
                {
                    _logger.ImageDirectoryCouldNotBeCreated(imageDirectoryPath);

                    return Task.FromResult(false);
                }
            }

            _logger.WritingImageToPath(filePath);

            using var destination = new FileStream(filePath, FileMode.Create);
            imageData.CopyTo(destination);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.FailedToWriteImageToPath(filePath, ex);

            return Task.FromResult(false);
        }
    }

    public override Task<Stream> GetImageAsync(Image image)
    {
        return Task.FromResult<Stream>(new FileStream(GetImageFullPath(image), FileMode.Open));
    }

    public override Task<bool> DeleteImageAsync(Image image)
    {
        var filePath = GetImageFullPath(image);

        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.FailedToDeleteImageFile(filePath, ex);

            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public void MapEndpoint(IApplicationBuilder builder)
    {
        builder.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = "/images",
            FileProvider = new PhysicalFileProvider(_storagePath)
        });
    }

    public async Task MigrateAsync(IImageProvider imageProvider, CancellationToken cancellationToken)
    {
        if (_skipMigration)
        {
            _logger.SkippingMigration();
            return;
        }

        /*
         * This migration copies files from the previous folder structure to the new one:
         *
         * Previous folder structure:
         *      <base path>/2025/5b22d459-58b8-4aac-8ad0-1b0ba5e453b3.webp
         *      <base path>/2026/7e669676-443b-4d01-a12e-676c9c097739.webp
         *
         * New folder structure:
         *      <base path>/2025/07/5b22d459-58b8-4aac-8ad0-1b0ba5e453b3.webp
         *      <base path>/2026/04/7e669676-443b-4d01-a12e-676c9c097739.webp
         *
         * The new folder structure introduces a month-based subdirectory. This is done to be aligned with the
         * other image storage implementations, which should make switching between implementations easier.
         */

        try
        {
            await _migrationSemaphore.WaitAsync(cancellationToken);

            // First, we figure out if a migration needs to be run by checking if any files with the old naming scheme exist.
            var hasOldFiles = Directory.GetDirectories(_storagePath).SelectMany(Directory.GetFiles).Any();
            if (!hasOldFiles)
            {
                _logger.DirectoryStructureIsUpToDate();
                return;
            }

            _logger.MigrationWillBeAttempted();

            // Load images from DB because we need the months of creation
            var images = await imageProvider.GetImagesAsync();

            // Consistency check - there might be an error in the image provider implementation causing invalid data to be returned.
            if (images.Count == 0)
            {
                _logger.ImageProviderReturnedNoImages();
                return;
            }

            // Iterate over all old files and handle them accordingly
            foreach (var directory in Directory.GetDirectories(_storagePath))
            {
                foreach (var file in Directory.GetFiles(directory))
                {
                    var fileName = Path.GetFileName(file);
                    var match = FileNameRegex().Match(fileName);

                    if (!match.Success)
                    {
                        _logger.EncounteredFileWhichDoesNotMatchPattern(Path.GetFullPath(file));
                        continue;
                    }

                    if (!Guid.TryParse(match.Groups["identifier"].Value, out var resourceIdentifier))
                    {
                        _logger.EncounteredFileThatHasNoValidGuidInName(Path.GetFullPath(file));
                        continue;
                    }

                    var image = images.FirstOrDefault(x => x.ResourceIdentifier == resourceIdentifier);

                    if (image is null)
                    {
                        _logger.EncounteredFileWithoutCorrespondingImageFromProvider(Path.GetFullPath(file));
                        continue;
                    }

                    var targetDirectory = Path.Join(_storagePath, image.CreatedAt.Year.ToString(CultureInfo.InvariantCulture), image.CreatedAt.Month.ToString("D2", CultureInfo.InvariantCulture));
                    var targetPath = Path.Join(targetDirectory, fileName);

                    Directory.CreateDirectory(targetDirectory);

                    if (File.Exists(targetPath))
                    {
                        _logger.EncounteredTargetFileThatAlreadyExists(targetPath, file);
                    }

                    File.Move(file, targetPath, overwrite: true);
                    _logger.SuccessfullyMovedFileTo(targetPath, file);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.ExceptionOccurredDuringMigration(ex);
        }
        finally
        {
            _migrationSemaphore.Release();
        }
    }

    private string GetImageFullPath(Image image)
    {
        return Path.Join(
            _storagePath,
            image.CreatedAt.Year.ToString(CultureInfo.InvariantCulture),
            image.CreatedAt.Month.ToString("D2", CultureInfo.InvariantCulture),
            GetImageFileName(image));
    }

    private static string GetImageFileName(Image image)
    {
        return $"{image.ResourceIdentifier}.{image.FileExtension}";
    }

    [GeneratedRegex(@"(?<identifier>[0-9a-f-]{36})\.(?<extension>[a-z]+)$")]
    private static partial Regex FileNameRegex();
}
