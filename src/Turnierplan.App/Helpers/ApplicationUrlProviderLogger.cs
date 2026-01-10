namespace Turnierplan.App.Helpers;

internal sealed partial class ApplicationUrlProviderLogger
{
    private readonly ILogger<ApplicationUrlProvider> _logger;

    public ApplicationUrlProviderLogger(ILogger<ApplicationUrlProvider> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Warning, "The 'ApplicationUrl' is not specified. Please check your application configuration.", EventId = 101)]
    public partial void ApplicationUrlNotSpecified();
}
