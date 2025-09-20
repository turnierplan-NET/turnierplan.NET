using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Extensions;
using Turnierplan.Adapter.Models;

namespace Turnierplan.Adapter;

/// <summary>
/// Provides an abstraction for accessing the turnierplan.NET API in the context of a .NET application.
/// </summary>
public sealed class TurnierplanClient : IDisposable
{
    private const string ApiKeyIdHeaderName = "X-Api-Key";
    private const string ApiKeySecretHeaderName = "X-Api-Key-Secret";
    private const string TurnierplanVersionHeaderName = "X-Turnierplan-Version";

    private static readonly string __turnierplanAdapterVersion =
        typeof(TurnierplanClient).Assembly.GetName().Version?.ToString()
            ?? throw new InvalidOperationException("Could not determine Turnierplan.Adapter version from assembly name.");

    private static readonly JsonSerializerOptions __serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TurnierplanClient"/> class, using an existing
    /// <see cref="HttpClient"/> and the specified options.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use.</param>
    /// <param name="options">The options to use.</param>
    /// <remarks>
    /// The specified <see cref="HttpClient"/> is not disposed when this <see cref="TurnierplanClient"/> is disposed.
    /// </remarks>
    public TurnierplanClient(HttpClient httpClient, TurnierplanClientOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKeySecret);

        _httpClient = httpClient;
        _httpClient.BaseAddress = options.ApplicationUri;

        _httpClient.DefaultRequestHeaders.Add(ApiKeyIdHeaderName, options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add(ApiKeySecretHeaderName, options.ApiKeySecret);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TurnierplanClient"/> class using the specified options. This
    /// constructor will create a new <see cref="HttpClient"/> instance which will be disposed automatically by this class.
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
    /// <item>Thrown if the version of the server does not match the version of the <c>Turnierplan.Adapter</c> library.</item>
    /// </list>
    /// </exception>
    public async Task<Tournament> GetTournament(string tournamentId)
    {
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
    /// <item>Thrown if the version of the server does not match the version of the <c>Turnierplan.Adapter</c> library.</item>
    /// </list>
    /// </exception>
    public async Task<List<TournamentHeader>> GetTournaments(string folderId)
    {
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

    private static void VerifyServerVersion(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues(TurnierplanVersionHeaderName, out var headerValue))
        {
            throw new TurnierplanClientException($"Could not get '{TurnierplanVersionHeaderName}' header from response.");
        }

        var serverVersion = headerValue.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(serverVersion))
        {
            throw new TurnierplanClientException($"Server returned an empty '{TurnierplanVersionHeaderName}' header.");
        }

        if (!serverVersion.Equals(__turnierplanAdapterVersion))
        {
            throw new TurnierplanClientException($"Server version '{serverVersion}' does not match the Turnierplan.Adapter version '{__turnierplanAdapterVersion}'.");
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
}
