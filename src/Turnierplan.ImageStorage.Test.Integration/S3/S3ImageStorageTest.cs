using Amazon.S3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using S3ServerLibrary;
using S3ServerLibrary.S3Objects;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;
using Turnierplan.ImageStorage.S3;
using WatsonWebserver.Core;
using Xunit;
using BindingFlags = System.Reflection.BindingFlags;

namespace Turnierplan.ImageStorage.Test.Integration.S3;

public sealed class S3ImageStorageTest : IDisposable
{
    private readonly int _port;
    private readonly S3Server _server;
    private readonly S3ImageStorageOptions _options;
    private readonly TestLogger _logger;

    public S3ImageStorageTest(ITestOutputHelper testOutputHelper)
    {
        _port = Random.Shared.Next(50000, 51000);

        var settings = new S3ServerSettings
        {
            Webserver = new WebserverSettings("localhost", _port),
            Logger = testOutputHelper.WriteLine
        };

        _options = new S3ImageStorageOptions
        {
            ServiceUrl = $"http://localhost:{_port}",
            AccessKey = "key",
            AccessKeySecret = "s3cr3t",
            BucketName = "test_bucket"
        };

        _server = new S3Server(settings);
        _server.Start();

        _logger = new TestLogger();
    }

    public void Dispose()
    {
        _server.Dispose();
    }

    [Fact]
    public void S3ImageStorage_Throws_Exception_When_Both_ServiceUrl_And_RegionEndpoint_Are_Specified()
    {
        var invalidOptions = _options with { RegionEndpoint = "us-east-1" };
        var action = () => new S3ImageStorage(new OptionsWrapper<S3ImageStorageOptions>(invalidOptions), _logger);
        action.Should().ThrowExactly<InvalidOperationException>().WithMessage("Please specify either 'RegionEndpoint' or 'ServiceUrl'.");
    }

    [Fact]
    public void S3ImageStorage_Throws_Exception_When_Invalid_RegionEndpoint_Is_Specified()
    {
        var invalidOptions = _options with { RegionEndpoint = "test" };
        var action = () => new S3ImageStorage(new OptionsWrapper<S3ImageStorageOptions>(invalidOptions), _logger);
        action.Should().ThrowExactly<InvalidOperationException>().WithMessage("The specified region endpoint 'test' seems to be unknown.");
    }

    [Fact]
    public void S3ImageStorage_GetFullImageUrl_Returns_Correct_Value()
    {
        using var storage = new S3ImageStorage(new OptionsWrapper<S3ImageStorageOptions>(_options), _logger);

        var image = CreateImage(Guid.Parse("969bd4c6-c7bb-4631-8c25-1e196bc77512"), new DateTime(2026, 7, 27), "png");
        storage.GetFullImageUrl(image).Should().Be($"http://localhost:{_port}/images/2026/07/969bd4c6-c7bb-4631-8c25-1e196bc77512.png");

        var image2 = CreateImage(Guid.Parse("88a530a9-0272-4b2d-a495-b876ba30b005"), new DateTime(2025, 12, 27), "jpeg");
        storage.GetFullImageUrl(image2).Should().Be($"http://localhost:{_port}/images/2025/12/88a530a9-0272-4b2d-a495-b876ba30b005.jpeg");

        var awsOptions = _options with { ServiceUrl = null, RegionEndpoint = "us-east-1" };
        using var awsStorage = new S3ImageStorage(new OptionsWrapper<S3ImageStorageOptions>(awsOptions), _logger);

        awsStorage.GetFullImageUrl(image).Should().Be("https://test_bucket.s3.us-east-1.amazonaws.com/images/2026/07/969bd4c6-c7bb-4631-8c25-1e196bc77512.png");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task S3ImageStorage_With_Local_Server_Can_Upload_Image(bool isAuthenticated)
    {
        using var storage = new S3ImageStorage(new OptionsWrapper<S3ImageStorageOptions>(_options), _logger);

        var image = CreateImage(Guid.Parse("969bd4c6-c7bb-4631-8c25-1e196bc77512"), new DateTime(2026, 7, 27), "png");
        var imageData = new MemoryStream([0x00, 0x01, 0x02, 0x03]);

        var writeCalled = false;
        _server.Object.Write = async ctx =>
        {
            writeCalled = true;

            if (!isAuthenticated)
            {
                ctx.Response.StatusCode = 401;
                await ctx.Response.Send(ErrorCode.AccessDenied);
            }

            ctx.Request.Bucket.Should().Be("test_bucket");
            ctx.Request.Key.Should().Be("images/2026/07/969bd4c6-c7bb-4631-8c25-1e196bc77512.png");
        };

        writeCalled.Should().BeFalse();
        await storage.SaveImageAsync(image, imageData);
        writeCalled.Should().BeTrue();

        if (isAuthenticated)
        {
            _logger.Messages.Should().BeEmpty();
        }
        else
        {
            _logger.Messages.Single().Should().Be("Failed to upload image 'images/2026/07/969bd4c6-c7bb-4631-8c25-1e196bc77512.png' to S3 because of an exception.");
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task S3ImageStorage_With_Local_Server_Can_Read_Image(bool isAuthenticated)
    {
        using var storage = new S3ImageStorage(new OptionsWrapper<S3ImageStorageOptions>(_options), _logger);

        var image = CreateImage(Guid.Parse("969bd4c6-c7bb-4631-8c25-1e196bc77512"), new DateTime(2026, 7, 27), "png");

        var readCalled = false;
        _server.Object.Read = async ctx =>
        {
            readCalled = true;

            if (!isAuthenticated)
            {
                ctx.Response.StatusCode = 401;
                await ctx.Response.Send(ErrorCode.AccessDenied);
            }

            ctx.Request.Bucket.Should().Be("test_bucket");
            ctx.Request.Key.Should().Be("images/2026/07/969bd4c6-c7bb-4631-8c25-1e196bc77512.png");

            var data = "Hello, S3!"u8;

            return new S3Object(
                ctx.Request.Key,
                "version-1",
                true,
                DateTime.UtcNow,
                "etag-123",
                data.Length,
                new Owner("admin", "Administrator"),
                [.. data],
                "text/plain"
            );
        };

        readCalled.Should().BeFalse();

        if (!isAuthenticated)
        {
            var func = () => storage.GetImageAsync(image);
            await func.Should().ThrowAsync<AmazonS3Exception>().WithMessage("Access denied.");
            readCalled.Should().BeTrue();

            return;
        }

        await using var result = await storage.GetImageAsync(image);
        readCalled.Should().BeTrue();

        using var temp = new MemoryStream();
        await result.CopyToAsync(temp, TestContext.Current.CancellationToken);

        temp.ToArray().Should().BeEquivalentTo([0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x2c, 0x20, 0x53, 0x33, 0x21], opt => opt.WithStrictOrdering());

        if (isAuthenticated)
        {
            _logger.Messages.Should().BeEmpty();
        }
        else
        {
            _logger.Messages.Single().Should().Be("Failed to upload image 'images/2026/07/969bd4c6-c7bb-4631-8c25-1e196bc77512.png' to S3 because of an exception.");
        }

        readCalled.Should().BeTrue();
    }

    private static Image CreateImage(Guid resourceIdentifier, DateTime createdAt, string extension)
    {
        // Create instances of Image class using reflection so we can specify resourceIdentifier & createdAt via internal ctor.

        var ctor = typeof(Image).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            [typeof(long), typeof(Guid), typeof(PublicId), typeof(DateTime), typeof(string), typeof(string), typeof(long), typeof(ushort), typeof(ushort)]);

        if (ctor is null)
        {
            throw new InvalidOperationException($"Could not find internal '{nameof(Image)}' constructor.");
        }

        var image = ctor.Invoke([0L, resourceIdentifier, PublicId.Empty, createdAt, string.Empty, extension, 0L, (ushort)0, (ushort)0]);

        return image as Image ?? throw new InvalidOperationException($"Could not instantiate '{nameof(Image)}' using reflection.");
    }

    private sealed class TestLogger : ILogger<S3ImageStorage>
    {
        public readonly List<string> Messages = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => throw new NotSupportedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
