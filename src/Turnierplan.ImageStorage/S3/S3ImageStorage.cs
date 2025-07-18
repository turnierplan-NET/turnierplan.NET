using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Turnierplan.Core.Image;

namespace Turnierplan.ImageStorage.S3;

internal sealed class S3ImageStorage : IImageStorage
{
    private readonly ILogger<S3ImageStorage> _logger;
    private readonly AmazonS3Client _client;
    private readonly string _bucketName;

    public S3ImageStorage(IOptions<S3ImageStorageOptions> options, ILogger<S3ImageStorage> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.AccessKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.AccessKeySecret);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.BucketName);

        _logger = logger;

        var s3Credentials = new BasicAWSCredentials(options.Value.AccessKey, options.Value.AccessKeySecret);
        var s3Config = new AmazonS3Config();

        var hasValidRegionEndpoint = false;

        if (!string.IsNullOrWhiteSpace(options.Value.RegionEndpoint))
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint);

            if (regionEndpoint.DisplayName.Contains("unknown", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"The specified region endpoint '{options.Value.RegionEndpoint}' seems to be unknown.");
            }

            s3Config.RegionEndpoint = regionEndpoint;
            hasValidRegionEndpoint = true;
        }

        if (!string.IsNullOrWhiteSpace(options.Value.ServiceUrl))
        {
            if (hasValidRegionEndpoint)
            {
                throw new InvalidOperationException($"Please specify either '{nameof(S3ImageStorageOptions.RegionEndpoint)}' or '{nameof(S3ImageStorageOptions.ServiceUrl)}'.");
            }

            s3Config.ServiceURL = options.Value.ServiceUrl;
        }

        _client = new AmazonS3Client(s3Credentials, s3Config);
        _bucketName = options.Value.BucketName;
    }

    public string GetFullImageUrl(Image image)
    {
        var objectKey = GetObjectKey(image);

        return string.IsNullOrWhiteSpace(_client.Config.ServiceURL)
            ? $"https://{_bucketName}.s3.{_client.Config.RegionEndpoint.SystemName}.amazonaws.com/{objectKey}"
            : $"{_client.Config.ServiceURL}/{objectKey}";
    }

    public async Task<bool> SaveImageAsync(Image image, MemoryStream imageData)
    {
        var objectKey = GetObjectKey(image);

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                InputStream = imageData,
                CannedACL = S3CannedACL.PublicRead
            };

            var response = await _client.PutObjectAsync(request).ConfigureAwait(false);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            _logger.LogError("Failed to upload image '{ObjectKey}' to S3. Result status code: {StatusCode}", objectKey, (int)response.HttpStatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image '{ObjectKey}' to S3 because of an exception.", objectKey);
        }

        return false;
    }

    public async Task<Stream> GetImageAsync(Image image)
    {
        var objectKey = GetObjectKey(image);

        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = objectKey
        };

        var response = await _client.GetObjectAsync(request).ConfigureAwait(false);

        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return response.ResponseStream;
        }

        _logger.LogError("Failed to read image '{ObjectKey}' from S3. Result status code: {StatusCode}", objectKey, (int)response.HttpStatusCode);

        throw new InvalidOperationException($"Failed to read image from S3. Status code: {response.HttpStatusCode}");
    }

    public async Task<bool> DeleteImageAsync(Image image)
    {
        var objectKey = GetObjectKey(image);

        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey
            };

            var response = await _client.DeleteObjectAsync(request).ConfigureAwait(false);

            if (response.HttpStatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image '{ObjectKey}' from S3 because of an exception.", objectKey);
        }

        return false;
    }

    private static string GetObjectKey(Image image)
    {
        return $"images/{image.CreatedAt.Year}/{image.CreatedAt.Month:D2}/{image.ResourceIdentifier}.{image.FileType}";
    }
}
