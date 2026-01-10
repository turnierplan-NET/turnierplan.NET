using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage.Local;

internal sealed class LocalImageStorage : ILocalImageStorage
{
    private readonly LocalImageStorageLogger _logger;
    private readonly string _storagePath;

    public LocalImageStorage(ILogger<LocalImageStorage> logger, IOptions<LocalImageStorageOptions> options)
    {
        _logger = new LocalImageStorageLogger(logger);

        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.StoragePath);
        _storagePath = Path.GetFullPath(options.Value.StoragePath);

        _logger.UsingDirectoryForLocalImageStorage(_storagePath);

        Directory.CreateDirectory(_storagePath);

        if (!Directory.Exists(_storagePath))
        {
            _logger.DirectoryCouldNotBeCreated();
        }
    }

    public string GetFullImageUrl(Image image)
    {
        return $"/images/{image.CreatedAt.Year}/{GetImageFileName(image)}";
    }

    public Task<bool> SaveImageAsync(Image image, MemoryStream imageData)
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
                    _logger.DirectoryForImageCouldNotBeCreated(imageDirectoryPath);

                    return Task.FromResult(false);
                }
            }

            _logger.WritingImageToFile(filePath);

            using var destination = new FileStream(filePath, FileMode.Create);
            imageData.CopyTo(destination);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.FailedToWriteImage(ex, filePath);

            return Task.FromResult(false);
        }
    }

    public Task<Stream> GetImageAsync(Image image)
    {
        return Task.FromResult<Stream>(new FileStream(GetImageFullPath(image), FileMode.Open));
    }

    public Task<bool> DeleteImageAsync(Image image)
    {
        var filePath = GetImageFullPath(image);

        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.FailedToDeleteImage(ex, filePath);

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

    public void Dispose()
    {
    }

    private string GetImageFullPath(Image image)
    {
        return Path.Combine(_storagePath, image.CreatedAt.Year.ToString(CultureInfo.InvariantCulture), GetImageFileName(image));
    }

    private static string GetImageFileName(Image image)
    {
        return $"{image.ResourceIdentifier}.{image.FileType}";
    }
}
