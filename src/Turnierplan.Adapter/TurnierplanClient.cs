using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using Turnierplan.Adapter.Models;

namespace Turnierplan.Adapter;

/// <summary>
/// Provides an abstraction for accessing the turnierplan.NET public API in the context of a .NET application.
/// </summary>
public sealed partial class TurnierplanClient : IDisposable
{
    private const string TurnierplanVersionHeaderName = "x-turnierplan-version";

    private static readonly string __turnierplanAdapterVersion;
    private static readonly JsonSerializerOptions __serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    static TurnierplanClient()
    {
        __turnierplanAdapterVersion = typeof(TurnierplanClient).Assembly.GetName().Version?.ToString()
                               ?? throw new InvalidOperationException("Could not determine Turnierplan.Adapter version from assembly name.");
    }

    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;
    private readonly bool _disableIdVerification;
    private readonly bool _disableVersionVerification;

    /// <summary>
    /// Initializes a new instance of the <see cref="TurnierplanClient"/> class, using an
    /// existing <see cref="HttpClient"/> and the specified options. Any modifications to the
    /// specified <see cref="TurnierplanClientOptions"/> instance are not recognized after
    /// this constructor has exited.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use.</param>
    /// <param name="options">The options to use.</param>
    /// <remarks>The specified <see cref="HttpClient"/> is not disposed by this class.</remarks>
    public TurnierplanClient(HttpClient httpClient, TurnierplanClientOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKeySecret);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.UserAgent);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApplicationUrl);

        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.ApplicationUrl);

        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key-Secret", options.ApiKeySecret);
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, options.UserAgent);

        _disableIdVerification = options.DisableIdVerification;
        _disableVersionVerification = options.DisableVersionVerification;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TurnierplanClient"/> class and the
    /// specified options. This constructor will create a new <see cref="HttpClient"/>
    /// instance which will be disposed automatically by this class. Any modifications to the
    /// specified <see cref="TurnierplanClientOptions"/> instance are not recognized after
    /// this constructor has exited.
    /// </summary>
    /// <param name="options">The options to use.</param>
    public TurnierplanClient(TurnierplanClientOptions options)
        : this(new HttpClient(), options)
    {
        _disposeHttpClient = true;
    }

    /// <summary>
    /// Fetches a single tournament from the API and returns the deserialized <see cref="Tournament"/>.
    /// </summary>
    /// <param name="tournamentId">The ID of the tournament to request.</param>
    /// <returns>An instance of the <see cref="Tournament"/> class which contains the data returned by the API.</returns>
    /// <exception cref="TurnierplanClientException">
    /// <list type="bullet">
    /// <item>Thrown if the API returns a non-200 status code or if the response body can not be deserialized.</item>
    /// <item>Thrown if <see cref="TurnierplanClientOptions.DisableIdVerification"/> is <c>false</c> and the specified <paramref name="tournamentId"/> is not a valid ID.</item>
    /// <item>Thrown if <see cref="TurnierplanClientOptions.DisableVersionVerification"/> is <c>false</c> and the version of the server does not match the version of the <c>Turnierplan.Adapter</c> library.</item>
    /// </list>
    /// </exception>
    /// <remarks>To be considered valid, the ID must match the following pattern: <c>[A-Za-z0-9_-]{11}</c></remarks>
    public async Task<Tournament> GetTournament(string tournamentId)
    {
        VerifyId(tournamentId);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tournaments/{tournamentId}");
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        VerifyServerVersion(response);

        return await Deserialize<Tournament>(response).ConfigureAwait(false);
    }

    /// <summary>
    /// Fetches a list of all tournaments in a specific folder from the API and returns the deserialized list with
    /// <c>0..n</c> entries of the type <see cref="TournamentHeader"/>.
    /// </summary>
    /// <param name="folderId">The ID of the folder to request.</param>
    /// <returns>A list of <see cref="TournamentHeader"/> instances which contains the data returned by the API.</returns>
    /// <exception cref="TurnierplanClientException">
    /// <list type="bullet">
    /// <item>Thrown if the API returns a non-200 status code or if the response body can not be deserialized.</item>
    /// <item>Thrown if <see cref="TurnierplanClientOptions.DisableIdVerification"/> is <c>false</c> and the specified <paramref name="folderId"/> is not a valid ID.</item>
    /// <item>Thrown if <see cref="TurnierplanClientOptions.DisableVersionVerification"/> is <c>false</c> and the version of the server does not match the version of the <c>Turnierplan.Adapter</c> library.</item>
    /// </list>
    /// </exception>
    /// <remarks>To be considered valid, the ID must match the following pattern: <c>[A-Za-z0-9_-]{11}</c></remarks>
    public async Task<List<TournamentHeader>> GetTournaments(string folderId)
    {
        VerifyId(folderId);

        var query = new QueryBuilder { { "folderId", folderId } };
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tournaments{query}");
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        VerifyServerVersion(response);

        return await Deserialize<List<TournamentHeader>>(response).ConfigureAwait(false);
    }

    /// <summary>
    /// If this instance was created using a pre-existing <see cref="HttpClient"/> instance, this method does nothing.
    /// If this instance was created without specifying a pre-existing <see cref="HttpClient"/> instance, this method
    /// will dispose the internally created <see cref="HttpClient"/>.
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private void VerifyId(string id)
    {
        if (_disableIdVerification)
        {
            return;
        }

        if (!ValidIdRegex().IsMatch(id))
        {
            throw new TurnierplanClientException("The specified ID does not match the expected format. Specify a correct ID or disable this check using 'DisableIdVerification'.");
        }
    }

    private void VerifyServerVersion(HttpResponseMessage response)
    {
        if (_disableVersionVerification)
        {
            return;
        }

        if (!response.Headers.TryGetValues(TurnierplanVersionHeaderName, out var headerValue))
        {
            throw new TurnierplanClientException($"Could not get '{TurnierplanVersionHeaderName}' header from response. Fix the issue or disable the version check using 'DisableServerVersionVerification'.");
        }

        var serverVersion = headerValue.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(serverVersion))
        {
            throw new TurnierplanClientException($"Server returned an empty '{TurnierplanVersionHeaderName}' header. Fix the issue or disable the version check using 'DisableServerVersionVerification'.");
        }

        if (!serverVersion.Equals(__turnierplanAdapterVersion))
        {
            throw new TurnierplanClientException($"Server version '{serverVersion}' does not match the Turnierplan.Adapter version '{__turnierplanAdapterVersion}'. Fix the issue or disable the version check using 'DisableServerVersionVerification'.");
        }
    }

    private static async Task<T> Deserialize<T>(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new TurnierplanClientException($"API returned unexpected status code: {response.StatusCode}");
        }

        var data = await response.Content.ReadFromJsonAsync<T>(__serializerOptions).ConfigureAwait(false);

        if (data is null)
        {
            throw new TurnierplanClientException($"Failed to deserialize API response of type '{typeof(T).Name}'.");
        }

        return data;
    }

    [GeneratedRegex("^[ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-]{11}$")]
    private static partial Regex ValidIdRegex();
}
