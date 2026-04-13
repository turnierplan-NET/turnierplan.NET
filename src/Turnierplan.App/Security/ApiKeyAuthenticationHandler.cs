using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Security;

internal sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string AuthenticationTypeName = "TurnierplanApiKeyAuthentication";
    private const string ApiKeyIdHeaderName = "x-api-key";
    private const string ApiKeySecretHeaderName = "x-api-key-secret";

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
        var hasApiKeyIdHeader = Request.Headers.TryGetValue(ApiKeyIdHeaderName, out var apiKeyIdHeaderValue);
        var hasApiKeySecretHeader = Request.Headers.TryGetValue(ApiKeySecretHeaderName, out var apiKeySecretHeaderValue);

        if (!hasApiKeyIdHeader && !hasApiKeySecretHeader)
        {
            return AuthenticateResult.NoResult();
        }

        if (apiKeyIdHeaderValue.Count != 1 || apiKeySecretHeaderValue.Count != 1)
        {
            return AuthenticateResult.Fail("The API key ID and secret headers must each be specified exactly once.");
        }

        var apiKeyId = apiKeyIdHeaderValue.Single();
        var apiKeySecret = apiKeySecretHeaderValue.Single();

        if (string.IsNullOrEmpty(apiKeyId) || string.IsNullOrEmpty(apiKeySecret) || !PublicId.TryParse(apiKeyId, out var apiKeyIdParsed))
        {
            return AuthenticateResult.Fail("There exists no valid API key with the specified ID and secret.");
        }

        var apiKey = await _apiKeyRepository.GetByPublicIdAsync(apiKeyIdParsed.Value);

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

        await _apiKeyRepository.UnitOfWork.SaveChangesAsync();

        var identity = new ClaimsIdentity(authenticationType: AuthenticationTypeName, claims: [
            new Claim(ClaimTypes.PrincipalId, apiKey.PrincipalId.ToString()),
            new Claim(ClaimTypes.PrincipalKind, nameof(PrincipalKind.ApiKey))
        ]);

        var principal = new ClaimsPrincipal([ identity ]);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
