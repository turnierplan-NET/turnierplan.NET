using Turnierplan.Server.Domain.Organizations;

namespace Turnierplan.Server.Domain.Projects;

internal sealed class Project : EntityBase<long>, IEntityWithPublicId
{
    public Project(string name)
    {
        Id = 0;
        PublicId = new PublicId();
        CreatedAt = DateTime.UtcNow;
        Name = name;
    }

    public Project(long id, PublicId publicId, DateTime createdAt, string name)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
    }

    public override long Id { get; protected set; }

    public Organization Organization { get; private set; } = null!;

    public PublicId PublicId { get; }

    public DateTime CreatedAt { get; }

    public string Name { get; set; }
}
