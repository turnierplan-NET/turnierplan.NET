using Microsoft.Extensions.Options;
using Turnierplan.App.Options;
using Turnierplan.PdfRendering;

namespace Turnierplan.App.Helpers;

internal sealed class ApplicationUrlProvider : IApplicationUrlProvider
{
    private readonly IOptionsMonitor<TurnierplanOptions> _options;
    private readonly ApplicationUrlProviderLogger _logger;

    public ApplicationUrlProvider(IOptionsMonitor<TurnierplanOptions> options, ILogger<ApplicationUrlProvider> logger)
    {
        _options = options;
        _logger = new ApplicationUrlProviderLogger(logger);
    }

    public string GetApplicationUrl()
    {
        var url = _options.CurrentValue.ApplicationUrl;

        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.ApplicationUrlNotSpecified();

            return string.Empty;
        }

        return url.TrimEnd('/');
    }
}
