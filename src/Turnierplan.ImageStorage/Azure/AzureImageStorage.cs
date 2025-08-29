using Azure.Core;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage.Azure;

internal sealed class AzureImageStorage : IImageStorage
{
    private readonly ILogger<AzureImageStorage> _logger;
    private readonly BlobContainerClient _client;
    private readonly string _storageAccountUrl;
    private readonly string _containerName;

    public AzureImageStorage(IOptions<AzureImageStorageOptions> options, ILogger<AzureImageStorage> logger)
    {
        _logger = logger;

        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.StorageAccountName);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.ContainerName);

        _storageAccountUrl = $"https://{options.Value.StorageAccountName}.blob.core.windows.net";
        var storageAccountUri = new Uri(_storageAccountUrl);

        _logger.LogInformation("Initializing Azure Blob Storage client for storage account '{StorageAccountUrl}'.", _storageAccountUrl);

        BlobServiceClient blobServiceClient;

        if (options.Value.UseAccountKey == true)
        {
            if (options.Value.UseClientSecret == true)
            {
                throw new ArgumentException($"'{nameof(AzureImageStorageOptions.UseAccountKey)}' and '{nameof(AzureImageStorageOptions.UseClientSecret)}' cannot both be set to true.");
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.AccountKey);

            _logger.LogInformation("Using account key authentication for Azure Blob Storage");

            var credential = new StorageSharedKeyCredential(options.Value.StorageAccountName, options.Value.AccountKey);
            blobServiceClient = new BlobServiceClient(storageAccountUri, credential);
        }
        else
        {
            TokenCredential credential;

            if (options.Value.UseClientSecret == true)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.TenantId);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.ClientId);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.ClientSecret);

                _logger.LogInformation("Using ClientSecretCredential for Azure Blob Storage");

                credential = new ClientSecretCredential(options.Value.TenantId, options.Value.ClientId, options.Value.ClientSecret);
            }
            else
            {
                _logger.LogInformation("Using DefaultAzureCredential for Azure Blob Storage");

                credential = new DefaultAzureCredential();
            }

            blobServiceClient = new BlobServiceClient(storageAccountUri, credential);
        }

        _client = blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
        _containerName = options.Value.ContainerName;
    }

    public string GetFullImageUrl(Image image)
    {
        var blobName = GetBlobName(image);

        return $"{_storageAccountUrl}/{_containerName}/{blobName}";
    }

    public async Task<bool> SaveImageAsync(Image image, MemoryStream imageData)
    {
        var blobName = GetBlobName(image);
        var blobClient = _client.GetBlobClient(blobName);

        try
        {
            await blobClient.UploadAsync(imageData).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image '{BlobName}' to Azure Blob Storage because of an exception.", blobName);
        }

        return false;
    }

    public async Task<Stream> GetImageAsync(Image image)
    {
        var blobName = GetBlobName(image);
        var blobClient = _client.GetBlobClient(blobName);

        try
        {
            var response = await blobClient.DownloadAsync().ConfigureAwait(false);

            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read image '{BlobName}' from Azure Blob Storage because of an exception.", blobName);

            throw new InvalidOperationException("Failed to read image from Azure Blob Storage.", ex);
        }
    }

    public async Task<bool> DeleteImageAsync(Image image)
    {
        var blobName = GetBlobName(image);
        var blobClient = _client.GetBlobClient(blobName);

        try
        {
            await blobClient.DeleteIfExistsAsync().ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image '{BlobName}' from Azure Blob Storage because of an exception.", blobName);
        }

        return false;
    }

    private static string GetBlobName(Image image)
    {
        return $"{image.CreatedAt.Year}/{image.CreatedAt.Month:D2}/{image.ResourceIdentifier}.{image.FileType}";
    }
}
