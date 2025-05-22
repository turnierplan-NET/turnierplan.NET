using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Turnierplan.Localization.Test.Unit.Localization;

public sealed class LocalizationProviderTest
{
    [Fact]
    public void LocalizationProvider___When_Created___Loads_Languages_As_Expected()
    {
        var logger = new TestLogger();
        var localizationProvider = new LocalizationProvider(logger);

        // Currently, only warnings and errors are logged. Therefore, we expect the number of total
        // logged messages to be 0 which indicates no problems during initialization occurred.
        logger.NumberOfLogMessages.Should().Be(0);

        localizationProvider.GetAvailableLanguages().Should().BeEquivalentTo(["de"]);

        localizationProvider.TryGetLocalization("de", out var de).Should().BeTrue();
        de.Should().BeOfType<InMemoryLocalization>().Subject.Culture.Should().Be(CultureInfo.GetCultureInfo("de"));
    }

    private class TestLogger : ILogger<LocalizationProvider>
    {
        public int NumberOfLogMessages { get; private set; }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotSupportedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotSupportedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            NumberOfLogMessages++;
        }
    }
}
