namespace Turnierplan.App.Security;

internal sealed partial class SigningKeyProviderLogger
{
    private readonly ILogger<SigningKeyProvider> _logger;

    public SigningKeyProviderLogger(ILogger<SigningKeyProvider> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Critical, "The directory for identity storage does not exist and could not be created.", EventId = 101)]
    public partial void IdentityDirectoryCreationFailed();
}
