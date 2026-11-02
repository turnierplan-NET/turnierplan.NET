namespace Turnierplan.Server.Domain.ApiKeys;

// TODO: Should API keys be owned by an organization or by a user?

internal sealed class ApiKey : EntityBase<long>, IEntityWithPublicId
{
    private readonly List<ApiKeyRequest> _requests = [];

    public ApiKey(string name, string description, DateTime expiryDate)
    {
        Id = 0;
        PublicId = new PublicId();
        PrincipalId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Name = name;
        Description = description;
        SecretHash = string.Empty;
        ExpiryDate = expiryDate;
        IsActive = false;
    }

    public ApiKey(long id, PublicId publicId, Guid principalId, DateTime createdAt, string name, string description, string secretHash, DateTime expiryDate, bool isActive)
    {
        Id = id;
        PublicId = publicId;
        PrincipalId = principalId;
        CreatedAt = createdAt;
        Name = name;
        Description = description;
        SecretHash = secretHash;
        ExpiryDate = expiryDate;
        IsActive = isActive;
    }

    public override long Id { get; protected set; }

    public PublicId PublicId { get; }

    public Guid PrincipalId { get; }

    public DateTime CreatedAt { get; }

    public string Name { get; }

    public string Description { get; }

    public string SecretHash { get; private set; }

    public DateTime ExpiryDate { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;

    public bool IsActive { get; set; }

    public IReadOnlyList<ApiKeyRequest> Requests => _requests.AsReadOnly();

    public void AssignNewSecret(Func<string, string> secretHashFunc, out string plainTextSecret)
    {
        plainTextSecret = GenerateSecret();
        SecretHash = secretHashFunc(plainTextSecret);
    }

    public void SetExpiryDate(DateTime newExpiryDate)
    {
        if (newExpiryDate < ExpiryDate)
        {
            throw new TurnierplanDomainException("The new expiry date must be after the currently set expiry date.");
        }

        ExpiryDate = newExpiryDate;
    }

    public void AddRequest(string path)
    {
        _requests.Add(new ApiKeyRequest(path));
    }

    private static string GenerateSecret()
    {
        const string alphabet = "ABCDEFGHIJKMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const int length = 32;

        var value = Enumerable.Range(0, length)
            .Select(_ => alphabet[Random.Shared.Next(alphabet.Length)])
            .ToArray();

        return new string(value);
    }
}
