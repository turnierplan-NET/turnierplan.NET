using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Organization;

public sealed class Organization : Entity<long>, IEntityWithPublicId, IEntityWithOwner
{
    internal readonly List<ApiKey.ApiKey> _apiKeys = new();
    internal readonly List<Folder.Folder> _folders = new();
    internal readonly List<Image.Image> _images = new();
    internal readonly List<Tournament.Tournament> _tournaments = new();
    internal readonly List<Venue.Venue> _venues = new();

    public Organization(User.User owner, string name)
    {
        owner._organizations.Add(this);

        Id = 0;
        PublicId = new PublicId.PublicId();
        CreatedAt = DateTime.UtcNow;
        Name = name;
        OwnerId = owner.Id;
    }

    internal Organization(long id, PublicId.PublicId publicId, DateTime createdAt, string name, Guid ownerId)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
        OwnerId = ownerId;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public Guid OwnerId { get; }

    public IReadOnlyList<ApiKey.ApiKey> ApiKeys => _apiKeys.AsReadOnly();

    public IReadOnlyList<Folder.Folder> Folders => _folders.AsReadOnly();

    public IReadOnlyList<Image.Image> Images => _images.AsReadOnly();

    public IReadOnlyList<Tournament.Tournament> Tournaments => _tournaments.AsReadOnly();

    public IReadOnlyList<Venue.Venue> Venues => _venues.AsReadOnly();
}
