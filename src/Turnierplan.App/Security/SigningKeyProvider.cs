using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Turnierplan.App.Options;

namespace Turnierplan.App.Security;

internal interface ISigningKeyProvider
{
    string GetSigningAlgorithm();

    SymmetricSecurityKey GetSigningKey();
}

internal sealed class SigningKeyProvider : ISigningKeyProvider
{
    private readonly SymmetricSecurityKey _signingKey;

    public SigningKeyProvider(IOptions<IdentityOptions> options, ILogger<SigningKeyProvider> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.StoragePath);

        var storagePath = Path.GetFullPath(options.Value.StoragePath);

        Directory.CreateDirectory(storagePath);

        if (!Directory.Exists(storagePath))
        {
            var customLogger = new SigningKeyProviderLogger(logger);
            customLogger.IdentityDirectoryCreationFailed();
        }

        var signingKeyFile = Path.Join(storagePath, "jwt-signing-key.bin");

        byte[] signingKey;

        if (File.Exists(signingKeyFile))
        {
            signingKey = File.ReadAllBytes(signingKeyFile);
        }
        else
        {
            signingKey = RandomNumberGenerator.GetBytes(512 / 8);
            File.WriteAllBytes(signingKeyFile, signingKey);
        }

        _signingKey = new SymmetricSecurityKey(signingKey);
    }

    public string GetSigningAlgorithm() => "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";

    public SymmetricSecurityKey GetSigningKey() => _signingKey;
}
