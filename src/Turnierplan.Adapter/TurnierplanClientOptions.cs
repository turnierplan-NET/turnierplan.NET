namespace Turnierplan.Adapter;

/// <summary>
/// Represents the options necessary for constructing an instance of <see cref="TurnierplanClient"/>.
/// </summary>
public sealed record TurnierplanClientOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TurnierplanClientOptions"/> class with the two required parameters representing the API key to use.
    /// </summary>
    /// <param name="applicationUrl">The base URL of the turnierplan.NET instance that should be queried.</param>
    /// <param name="apiKey">The API key ID as displayed in the turnierplan.NET Portal.</param>
    /// <param name="apiKeySecret">The API key secret as displayed in the turnierplan.NET Portal.</param>
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
    /// The API key ID as displayed in the turnierplan.NET portal.
    /// </summary>
    public string ApiKey { get; }

    /// <summary>
    /// The API key secret as displayed in the turnierplan.NET portal.
    /// </summary>
    public string ApiKeySecret { get; }

    /// <summary>
    /// The value that should be included in the "User-Agent" HTTP header. The default is
    /// to <c>Turnierplan.Adapter</c> and this property may not be empty or white-space.
    /// </summary>
    public string UserAgent { get; init; } = "Turnierplan.Adapter";

    /// <summary>
    /// If set to <c>true</c>, the <see cref="TurnierplanClient"/> does not verify the ID
    /// strings before sending the HTTP requests.
    /// </summary>
    public bool DisableIdVerification { get; init; } = false;

    /// <summary>
    /// If set to <c>true</c>, the <see cref="TurnierplanClient"/> does not verify the server version against
    /// the used version of <c>Turnierplan.Adapter</c>.
    /// </summary>
    public bool DisableVersionVerification { get; init; } = false;
}
