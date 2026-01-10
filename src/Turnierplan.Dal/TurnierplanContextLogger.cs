using Microsoft.Extensions.Logging;

namespace Turnierplan.Dal;

internal sealed partial class TurnierplanContextLogger
{
    private readonly ILogger<TurnierplanContext> _logger;

    public TurnierplanContextLogger(ILogger<TurnierplanContext> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Information, "Beginning database transaction", EventId = 101)]
    public partial void BeginningTransaction();

    [LoggerMessage(LogLevel.Information, "Committing database transaction", EventId = 102)]
    public partial void CommittingTransaction();

    [LoggerMessage(LogLevel.Information, "Rolling back database transaction", EventId = 103)]
    public partial void RollingBackTransaction();
}
