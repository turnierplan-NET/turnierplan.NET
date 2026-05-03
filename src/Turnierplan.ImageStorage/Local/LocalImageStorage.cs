using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage.Local;

internal sealed class LocalImageStorage : ImageStorageBase, IMigratableImageStorage
{
    private readonly ILogger<LocalImageStorage> _logger;
    private readonly string _storagePath;
    private readonly bool _skipMigration;

    public LocalImageStorage(ILogger<LocalImageStorage> logger, IOptions<LocalImageStorageOptions> options)
    {
        _logger = logger;

        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.StoragePath);
        _storagePath = Path.GetFullPath(options.Value.StoragePath);

        _logger.LogInformation("Using the following directory for local image storage: '{LocalImageStoragePath}'", _storagePath);

        Directory.CreateDirectory(_storagePath);

        if (!Directory.Exists(_storagePath))
        {
            _logger.LogCritical("The directory for local image storage does not exist and could not be created.");
            _skipMigration = true;
        }
    }

    public override string GetFullImageUrl(Image image)
    {
        return $"/images/{image.CreatedAt.Year}/{GetImageFileName(image)}";
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
                    _logger.LogCritical("The directory for the image could not be created: '{ImageDirectory}'.", imageDirectoryPath);

                    return Task.FromResult(false);
                }
            }

            _logger.LogDebug("Writing image to: {FilePath}", filePath);

            using var destination = new FileStream(filePath, FileMode.Create);
            imageData.CopyTo(destination);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save image to file to '{FilePath}'.", filePath);

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
            _logger.LogError(ex, "Failed to delete image file '{FilePath}'.", filePath);

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
            _logger.LogCritical("A previous initialization error of local image storage will cause the migrations to be skipped.");
            return;
        }

        try
        {
            // TODO: Don't use version file and detect whether migration is necessary by looking at the files.

            var version = "1";
            var versionFile = Path.Join(_storagePath, ".version");

            if (File.Exists(versionFile))
            {
                version = await File.ReadAllTextAsync(versionFile, cancellationToken);
            }

            if (version.Equals("2"))
            {
                // Latest version, no migrations required
                _logger.LogInformation("Storage of local image storage is up to date.");
                return;
            }

            if (!version.Equals("1"))
            {
                _logger.LogCritical("Version file in local image storage path contains an unknown version '{Version}'.", version);
                return;
            }

            var images = await imageProvider.GetImagesAsync();
            // TODO: Run migration by copying the files & writing new version number
            // TODO: Display info message if any old files still exist
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An unexpected exception occurred while trying to run local image storage migrations.");
        }
    }

    private string GetImageFullPath(Image image)
    {
        return Path.Join(_storagePath, image.CreatedAt.Year.ToString(CultureInfo.InvariantCulture), GetImageFileName(image));
    }

    private static string GetImageFileName(Image image)
    {
        return $"{image.ResourceIdentifier}.{image.FileExtension}";
    }
}
