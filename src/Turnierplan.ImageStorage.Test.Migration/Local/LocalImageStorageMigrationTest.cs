using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Turnierplan.Core.Image;
using Turnierplan.Core.PublicId;
using Turnierplan.ImageStorage.Extensions;
using Xunit;
using BindingFlags = System.Reflection.BindingFlags;

namespace Turnierplan.ImageStorage.Test.Migration.Local;

public sealed class LocalImageStorageMigrationTest : IDisposable
{
    private const string ImageExtension = "jpg";

    private const int LogEventIdUsingDirectoryForStorage = 1;
    private const int LogEventIdDirectoryStructureIsUpToDate = 8;
    private const int LogEventIdMigrationWillBeAttempted = 9;
    private const int LogEventIdImageProviderReturnedNoImages = 10;
    private const int LogEventIdEncounteredFileWithoutCorrespondingImageFromProvider = 13;
    private const int LogEventIdSuccessfullyMovedFileTo = 15;

    private readonly string _basePath;
    private readonly TestLoggerProvider _loggerProvider;
    private readonly StateForTestImageProvider _imageProviderState;
    private readonly ServiceProvider _serviceProvider;

    public LocalImageStorageMigrationTest()
    {
        _basePath = Path.Join(Path.GetTempPath(), $"{nameof(LocalImageStorageMigrationTest)}-{Guid.NewGuid()}");

        var config = new ConfigurationBuilder().AddInMemoryCollection([
            new KeyValuePair<string, string?>("ImageStorage:Type", "Local"),
            new KeyValuePair<string, string?>("ImageStorage:StoragePath", _basePath)
        ]).Build().GetSection("ImageStorage");

        var services = new ServiceCollection();

        _loggerProvider = new TestLoggerProvider();
        services.AddLogging(builder => builder.AddProvider(_loggerProvider));

        _imageProviderState = new StateForTestImageProvider();
        services.AddSingleton(_imageProviderState);
        services.AddTurnierplanImageStorage<TestImageProvider>(config);

        _serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();

        if (Directory.Exists(_basePath))
        {
            Directory.Delete(_basePath, recursive: true);
        }
    }

    [Fact]
    public async Task Migration___With_Empty_Directory_And_No_Images___Works_As_Expected()
    {
        await RunMigrationAsync();

        ExpectLogMessages(LogEventIdUsingDirectoryForStorage, LogEventIdDirectoryStructureIsUpToDate);
    }

    [Fact]
    public async Task Migration___With_Some_Images_In_Old_Structure___Works_As_Expected()
    {
        var image1 = AddNewImage(new DateTime(2025, 05, 03));
        var image2 = AddNewImage(new DateTime(2025, 05, 15));
        var image3 = AddNewImage(new DateTime(2026, 02, 17));

        await RunMigrationAsync();

        ExpectLogMessages(LogEventIdUsingDirectoryForStorage, LogEventIdMigrationWillBeAttempted, LogEventIdSuccessfullyMovedFileTo);
        ExpectLogMessageExists(LogEventIdSuccessfullyMovedFileTo, $@"^Successfully moved image file '.*{image1.ResourceIdentifier}\.{ImageExtension}' to '.*{image1.ResourceIdentifier}\.{ImageExtension}'\.$");
        ExpectLogMessageExists(LogEventIdSuccessfullyMovedFileTo, $@"^Successfully moved image file '.*{image2.ResourceIdentifier}\.{ImageExtension}' to '.*{image2.ResourceIdentifier}\.{ImageExtension}'\.$");
        ExpectLogMessageExists(LogEventIdSuccessfullyMovedFileTo, $@"^Successfully moved image file '.*{image3.ResourceIdentifier}\.{ImageExtension}' to '.*{image3.ResourceIdentifier}\.{ImageExtension}'\.$");

        // Images should all be moved to the new structure - the old files should have been deleted
        CheckImageFileInOldStructure(image1, expectExists: false);
        CheckImageFileInOldStructure(image2, expectExists: false);
        CheckImageFileInOldStructure(image3, expectExists: false);
        CheckImageFileInNewStructure(image1, expectExists: true);
        CheckImageFileInNewStructure(image2, expectExists: true);
        CheckImageFileInNewStructure(image3, expectExists: true);
    }

    [Fact]
    public async Task Migration___With_Some_Images_In_Old_Structure_But_Image_Is_Missing_One_Specific_Image___Works_As_Expected()
    {
        var image1 = AddNewImage(new DateTime(2025, 05, 03));
        var image2 = AddNewImage(new DateTime(2025, 05, 15));
        var image3 = AddNewImage(new DateTime(2026, 02, 17));

        _imageProviderState.Images.Remove(image2);

        await RunMigrationAsync();

        ExpectLogMessages(LogEventIdUsingDirectoryForStorage, LogEventIdMigrationWillBeAttempted, LogEventIdSuccessfullyMovedFileTo, LogEventIdEncounteredFileWithoutCorrespondingImageFromProvider);
        ExpectLogMessageExists(LogEventIdSuccessfullyMovedFileTo, $@"^Successfully moved image file '.*{image1.ResourceIdentifier}\.{ImageExtension}' to '.*{image1.ResourceIdentifier}\.{ImageExtension}'\.$");
        ExpectLogMessageExists(LogEventIdSuccessfullyMovedFileTo, $@"^Successfully moved image file '.*{image3.ResourceIdentifier}\.{ImageExtension}' to '.*{image3.ResourceIdentifier}\.{ImageExtension}'\.$");
        ExpectLogMessageExists(LogEventIdEncounteredFileWithoutCorrespondingImageFromProvider, $@"^Encountered a file in the image storage directory for which there exists no corresponding entry from the image provider: '.*{image2.ResourceIdentifier}\.{ImageExtension}'$");

        // Images 1 & 3 are moved to the new structure, while image 2 remains in the old structure
        CheckImageFileInOldStructure(image1, expectExists: false);
        CheckImageFileInOldStructure(image2, expectExists: true);
        CheckImageFileInOldStructure(image3, expectExists: false);
        CheckImageFileInNewStructure(image1, expectExists: true);
        CheckImageFileInNewStructure(image2, expectExists: false);
        CheckImageFileInNewStructure(image3, expectExists: true);
    }

    [Fact]
    public async Task Migration___With_Some_Images_In_Old_Structure_But_Image_Provider_Returns_No_Images___Works_As_Expected()
    {
        var image1 = AddNewImage(new DateTime(2025, 05, 03));
        var image2 = AddNewImage(new DateTime(2025, 05, 15));
        var image3 = AddNewImage(new DateTime(2026, 02, 17));

        _imageProviderState.Images.Clear();

        await RunMigrationAsync();

        ExpectLogMessages(LogEventIdUsingDirectoryForStorage, LogEventIdMigrationWillBeAttempted, LogEventIdImageProviderReturnedNoImages);

        // Images should still exist in the old structure
        CheckImageFileInOldStructure(image1, expectExists: true);
        CheckImageFileInOldStructure(image2, expectExists: true);
        CheckImageFileInOldStructure(image3, expectExists: true);
        CheckImageFileInNewStructure(image1, expectExists: false);
        CheckImageFileInNewStructure(image2, expectExists: false);
        CheckImageFileInNewStructure(image3, expectExists: false);
    }

    private async Task RunMigrationAsync()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var migrator = scope.ServiceProvider.GetRequiredService<ImageStorageMigrator>();

        await migrator.MigrateAsync(TestContext.Current.CancellationToken);
    }

    private Image AddNewImage(DateTime createdAt)
    {
        var ctor = typeof(Image).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, [typeof(long), typeof(Guid), typeof(PublicId), typeof(DateTime), typeof(string), typeof(string), typeof(long), typeof(ushort), typeof(ushort)]);
        var id = (long)_imageProviderState.Images.Count + 1;
        var image = (Image)ctor!.Invoke([id, Guid.NewGuid(), PublicId.Empty, createdAt, $"Image{_imageProviderState.Images.Count + 1}", ImageExtension, (long)1, (ushort)1, (ushort)1]);

        _imageProviderState.Images.Add(image);

        CreateImageFile(image.CreatedAt.Year, GetImageFileName(image), image.Name);

        return image;
    }

    private void CreateImageFile(int year, string fileName, string contents)
    {
        var directory = Path.Join(_basePath, $"{year}");
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Join(directory, fileName), contents);
    }

    private void CheckImageFileInOldStructure(int year, string fileName, bool expectExists = true)
    {
        var path = Path.Join(_basePath, $"{year}", fileName);
        File.Exists(path).Should().Be(expectExists);
    }

    private void CheckImageFileInOldStructure(Image image, bool expectExists = true)
    {
        CheckImageFileInOldStructure(image.CreatedAt.Year, GetImageFileName(image), expectExists);
    }

    private void CheckImageFileInNewStructure(int year, int month, string fileName, bool expectExists = true)
    {
        var path = Path.Join(_basePath, $"{year}", $"{month:D2}", fileName);
        File.Exists(path).Should().Be(expectExists);
    }

    private void CheckImageFileInNewStructure(Image image, bool expectExists = true)
    {
        CheckImageFileInNewStructure(image.CreatedAt.Year, image.CreatedAt.Month, GetImageFileName(image), expectExists);
    }

    private void ExpectLogMessages(params int[] expectedEventIds)
    {
        var actualEventIds = _loggerProvider.Logger.Messages.Keys;
        actualEventIds.Except(expectedEventIds).Should().BeEmpty(because: "all expected log event IDs should be present at least once in the actual log messages");
        expectedEventIds.Except(actualEventIds).Should().BeEmpty(because: "the actual log messages should only contain expected log event IDs");
    }

    private void ExpectLogMessageExists(int eventId, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        _loggerProvider.Logger.Messages.TryGetValue(eventId, out var list).Should().BeTrue();
        var regex = new Regex(pattern);
        list.Should().ContainSingle(x => regex.IsMatch(x));
    }

    private static string GetImageFileName(Image image) => $"{image.ResourceIdentifier}.{ImageExtension}";

    private sealed class TestLogger : ILogger
    {
        public readonly Dictionary<int, List<string>> Messages = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => throw new NotSupportedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Messages.TryAdd(eventId.Id, []);
            Messages[eventId.Id].Add(formatter(state, exception));
        }
    }

    private sealed class TestLoggerProvider : ILoggerProvider
    {
        public readonly TestLogger Logger = new();

        public ILogger CreateLogger(string name) => Logger;

        public void Dispose() { }
    }

    private sealed class StateForTestImageProvider
    {
        public readonly List<Image> Images = [];
    }

    private sealed class TestImageProvider(StateForTestImageProvider state) : IImageProvider
    {
        public Task<IReadOnlyCollection<Image>> GetImagesAsync() => Task.FromResult<IReadOnlyCollection<Image>>(state.Images.AsReadOnly());
    }
}
