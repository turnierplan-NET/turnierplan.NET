using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Turnierplan.App.Options;

namespace Turnierplan.App.Security;

internal interface ISigningKeyProvider
{
    string GetSigningAlgorithm();

    Task<SymmetricSecurityKey> GetSigningKeyAsync(CancellationToken cancellationToken);
}

internal sealed class SigningKeyProvider : ISigningKeyProvider
{
    private const int SigningKeySizeBytes = 512 / 8;
    private const string SigningAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly IdentityOptions _options;
    private readonly ILogger<SigningKeyProvider> _logger;

    private bool _initializationAttempted;
    private SymmetricSecurityKey? _signingKey;

    public SigningKeyProvider(IOptions<IdentityOptions> options, ILogger<SigningKeyProvider> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    public string GetSigningAlgorithm()
    {
        return SigningAlgorithm;
    }

    public async Task<SymmetricSecurityKey> GetSigningKeyAsync(CancellationToken cancellationToken)
    {
        if (_signingKey is not null)
        {
            return _signingKey;
        }

        try
        {
            await _semaphore.WaitAsync(cancellationToken);

            // Signing key could have been set while waiting for the semaphore
            if (_signingKey is not null)
            {
                return _signingKey;
            }

            // Only attempt to initialize the sining key once
            if (!_initializationAttempted)
            {
                await InitializeSigningKey(cancellationToken);

                _initializationAttempted = true;
            }

            // If signing key is still null, the initialization failed
            return _signingKey ?? throw new InvalidOperationException("The signing key is not available: Check the startup logs for error details.");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task InitializeSigningKey(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_options.SigningKey))
        {
            _logger.LogInformation("Initializing signing key from app config.");
            InitializeSigningKeyFromAppConfig();
        }
        else if (!string.IsNullOrWhiteSpace(_options.StoragePath))
        {
            _logger.LogInformation("Initializing signing key from file store.");
            await InitializeSigningKeyWithFileStore(cancellationToken);
        }
        else
        {
            _logger.LogCritical("Either signing key or storage path must be specified.");
        }
    }

    private void InitializeSigningKeyFromAppConfig()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.SigningKey);

        byte[] signingKey;

        try
        {
            signingKey = Convert.FromBase64String(_options.SigningKey);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Signing key from app configuration could not be decoded.");
            return;
        }

        if (signingKey.Length == SigningKeySizeBytes)
        {
            _signingKey = new SymmetricSecurityKey(signingKey);
        }
        else
        {
            _logger.LogCritical("Signing key from app configuration must be {ExpectedSize} bytes long, but it is {ActualSize}.", SigningKeySizeBytes, signingKey.Length);
        }
    }

    private async Task InitializeSigningKeyWithFileStore(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.StoragePath);

        var storagePath = Path.GetFullPath(_options.StoragePath);

        Directory.CreateDirectory(storagePath);

        if (!Directory.Exists(storagePath))
        {
            _logger.LogCritical("The directory for identity storage does not exist and could not be created.");
            return;
        }

        var signingKeyFile = Path.Join(storagePath, "jwt-signing-key.bin");
        byte[] signingKey;

        if (File.Exists(signingKeyFile))
        {
            try
            {
                signingKey = await File.ReadAllBytesAsync(signingKeyFile, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Signing key could not be loaded from file: {SigningKeyFilePath}", signingKeyFile);
                return;
            }
        }
        else
        {
            signingKey = RandomNumberGenerator.GetBytes(SigningKeySizeBytes);

            try
            {
                await File.WriteAllBytesAsync(signingKeyFile, signingKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Signing key could not be written to file: {SigningKeyFilePath}", signingKeyFile);
            }
        }

        _signingKey = new SymmetricSecurityKey(signingKey);
    }
}
