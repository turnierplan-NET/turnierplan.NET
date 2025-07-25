using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.App.Security;

internal sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyIdHeaderName = "x-api-key";
    private const string ApiKeySecretHeaderName = "x-api-key-secret";
    private const string TurnierplanVersionHeaderName = "x-turnierplan-version";

    private static readonly string __turnierplanVersion =
        typeof(ApiKeyAuthenticationHandler).Assembly.GetName().Version?.ToString()
            ?? throw new InvalidOperationException("Could not determine turnierplan.NET version from assembly name.");

    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IPasswordHasher<ApiKey> _secretHasher;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyRepository apiKeyRepository,
        IPasswordHasher<ApiKey> secretHasher)
        : base(options, logger, encoder)
    {
        _apiKeyRepository = apiKeyRepository;
        _secretHasher = secretHasher;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string apiKeyId, apiKeySecret;

        try
        {
            apiKeyId = Request.Headers[ApiKeyIdHeaderName][0]!;
            apiKeySecret = Request.Headers[ApiKeySecretHeaderName][0]!;
        }
        catch
        {
            return AuthenticateResult.NoResult();
        }

        if (string.IsNullOrEmpty(apiKeyId) || string.IsNullOrEmpty(apiKeySecret) || !PublicId.TryParse(apiKeyId, out var apiKeyIdParsed))
        {
            return AuthenticateResult.Fail("There exists no valid API key with the specified ID and secret.");
        }

        var apiKey = await _apiKeyRepository.GetByPublicIdAsync(apiKeyIdParsed.Value).ConfigureAwait(false);

        if (apiKey is null || !apiKey.IsActive || apiKey.IsExpired)
        {
            return AuthenticateResult.Fail("There exists no valid API key with the specified ID and secret.");
        }

        var secretVerifyResult = _secretHasher.VerifyHashedPassword(apiKey, apiKey.SecretHash, apiKeySecret);

        if (secretVerifyResult is PasswordVerificationResult.Failed)
        {
            return AuthenticateResult.Fail("There exists no valid API key with the specified ID and secret.");
        }

        apiKey.AddRequest(new ApiKeyRequest($"{Request.Path}{Request.QueryString}"));

        await _apiKeyRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

        var identity = new ClaimsIdentity(claims: [
            new Claim(ClaimTypes.PrincipalId, apiKey.PrincipalId.ToString()),
            new Claim(ClaimTypes.PrincipalKind, nameof(PrincipalKind.ApiKey))
        ]);

        var principal = new ClaimsPrincipal([ identity ]);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        // Add the version as response header so that the Turnierplan.Adapter, if used, can detect a potential version mismatch
        Context.Response.Headers.Append(TurnierplanVersionHeaderName, __turnierplanVersion);

        return AuthenticateResult.Success(ticket);
    }
}
