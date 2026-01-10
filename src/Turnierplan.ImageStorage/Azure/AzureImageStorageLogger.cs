using Microsoft.Extensions.Logging;

namespace Turnierplan.ImageStorage.Azure;

internal sealed partial class AzureImageStorageLogger
{
    private readonly ILogger<AzureImageStorage> _logger;

    public AzureImageStorageLogger(ILogger<AzureImageStorage> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Information, "Initializing Azure Blob Storage client for storage account '{StorageAccountUrl}'.", EventId = 100)]
    public partial void InitializingAzureBlobStorageClient(string storageAccountUrl);

    [LoggerMessage(LogLevel.Information, "Using account key authentication for Azure Blob Storage", EventId = 101)]
    public partial void UsingAccountKeyAuthentication();

    [LoggerMessage(LogLevel.Information, "Using ClientSecretCredential for Azure Blob Storage", EventId = 102)]
    public partial void UsingClientSecretCredential();

    [LoggerMessage(LogLevel.Information, "Using DefaultAzureCredential for Azure Blob Storage", EventId = 103)]
    public partial void UsingDefaultAzureCredential();

    [LoggerMessage(LogLevel.Error, "Failed to upload image '{BlobName}' to Azure Blob Storage because of an exception.", EventId = 104)]
    public partial void FailedToUploadImage(Exception exception, string blobName);

    [LoggerMessage(LogLevel.Error, "Failed to read image '{BlobName}' from Azure Blob Storage because of an exception.", EventId = 105)]
    public partial void FailedToReadImage(Exception exception, string blobName);

    [LoggerMessage(LogLevel.Error, "Failed to delete image '{BlobName}' from Azure Blob Storage because of an exception.", EventId = 106)]
    public partial void FailedToDeleteImage(Exception exception, string blobName);
}
