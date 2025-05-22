using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Venue;

public sealed class Venue : Entity<long>, IEntityWithPublicId, IEntityWithOwner
{
    internal List<Tournament.Tournament> _tournaments = new();

    public Venue(Organization.Organization organization, string name, string description)
    {
        organization._venues.Add(this);

        Id = 0;
        PublicId = new PublicId.PublicId();
        Organization = organization;
        CreatedAt = DateTime.UtcNow;
        Name = name;
        Description = description;
    }

    internal Venue(long id, PublicId.PublicId publicId, DateTime createdAt, string name, string description)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
        Description = description;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    Guid IEntityWithOwner.OwnerId => Organization.OwnerId;

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public List<string> AddressDetails { get; set; } = new();

    public List<string> ExternalLinks { get; set; } = new();

    public IReadOnlyList<Tournament.Tournament> Tournaments => _tournaments.AsReadOnly();
}
