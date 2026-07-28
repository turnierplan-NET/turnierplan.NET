using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using S3ServerLibrary;
using S3ServerLibrary.S3Objects;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.ImageStorage.S3;
using WatsonWebserver.Core;
using Xunit;
using BindingFlags = System.Reflection.BindingFlags;

namespace Turnierplan.ImageStorage.Test.Integration.S3;

public sealed class S3ImageStorageTest : IDisposable
{
    private readonly S3Server _server;
    private readonly TestLogger _logger;

    public S3ImageStorageTest(ITestOutputHelper testOutputHelper)
    {
        var port = Random.Shared.Next(50000, 51000);
        var settings = new S3ServerSettings
        {
            Webserver = new WebserverSettings("localhost", port),
            Logger = testOutputHelper.WriteLine
        };

        _server = new S3Server(settings);
        _server.Start();

        _logger = new TestLogger();
    }

    public void Dispose()
    {
        _server.Dispose();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task S3ImageStorage_With_Non_Aws_Options_Can_Upload_File(bool isAuthenticated)
    {
        var options = new S3ImageStorageOptions
        {
            ServiceUrl = $"http://localhost:{_server.Webserver.Settings.Port}",
            AccessKey = "key",
            AccessKeySecret = "s3cr3t",
            BucketName = "test_bucket"
        };

        var storage = new S3ImageStorage(new OptionsWrapper<S3ImageStorageOptions>(options), _logger);

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
