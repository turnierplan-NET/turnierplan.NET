using Turnierplan.Server.Domain.Projects;

namespace Turnierplan.Server.Domain.Tournaments;

internal sealed class Tournament : EntityBase<long>, IEntityWithPublicId
{
    public Tournament(string name)
    {
        Id = 0;
        PublicId = new PublicId();
        CreatedAt = DateTime.UtcNow;
        Name = name;
    }

    public Tournament(long id, PublicId publicId, DateTime createdAt, string name)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
    }

    public override long Id { get; protected set; }

    [InheritPermissions]
    public Project Project { get; private set; } = null!;

    public PublicId PublicId { get; }

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public Visibility Visibility { get; set; }

    public bool IsPublic => Visibility is Visibility.Public;
}
