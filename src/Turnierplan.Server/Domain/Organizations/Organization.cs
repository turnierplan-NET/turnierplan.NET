using Turnierplan.Server.Domain.Projects;

namespace Turnierplan.Server.Domain.Organizations;

internal sealed class Organization : EntityBase<long>, IEntityWithPublicId
{
    private readonly List<Project> _projects = [];

    public Organization(string name)
    {
        Id = 0;
        PublicId = new PublicId();
        CreatedAt = DateTime.UtcNow;
        Name = name;
    }

    public Organization(long id, PublicId publicId, DateTime createdAt, string name)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
    }

    public override long Id { get; protected set; }

    public PublicId PublicId { get; }

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public IReadOnlyList<Project> Projects => _projects.AsReadOnly();
}
