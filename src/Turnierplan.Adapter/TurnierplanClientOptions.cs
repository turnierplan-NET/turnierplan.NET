namespace Turnierplan.Adapter;

/// <summary>
/// The options required for constructing an instance of <see cref="TurnierplanClient"/>.
/// </summary>
public sealed record TurnierplanClientOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TurnierplanClientOptions"/> class with the specified options.
    /// </summary>
    /// <param name="applicationUrl">The base URL of the turnierplan.NET instance that should be queried.</param>
    /// <param name="apiKey">The id of the API key.</param>
    /// <param name="apiKeySecret">The secret of the API key.</param>
    public TurnierplanClientOptions(string applicationUrl, string apiKey, string apiKeySecret)
    {
        ApplicationUrl = applicationUrl;
        ApiKey = apiKey;
        ApiKeySecret = apiKeySecret;
    }

    /// <summary>
    /// The base URL of the turnierplan.NET instance that should be queried.
    /// </summary>
    public string ApplicationUrl { get; }

    /// <summary>
    /// The id of the API key.
    /// </summary>
    public string ApiKey { get; }

    /// <summary>
    /// The secret of the API key.
    /// </summary>
    public string ApiKeySecret { get; }
}
