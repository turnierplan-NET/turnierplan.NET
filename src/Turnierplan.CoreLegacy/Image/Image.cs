using Turnierplan.Core.Entity;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.Image;

public sealed class Image : Entity<long>, IEntityWithRoleAssignments<Image>, IEntityWithOrganization
{
    internal readonly List<RoleAssignment<Image>> _roleAssignments = [];

    public Image(Organization.Organization organization, string name, string fileExtension, long fileSize, ushort width, ushort height)
    {
        organization._images.Add(this);

        Id = 0;
        ResourceIdentifier = Guid.NewGuid();
        PublicId = new PublicId.PublicId();
        Organization = organization;
        CreatedAt = DateTime.UtcNow;
        Name = name;
        FileExtension = fileExtension;
        FileSize = fileSize;
        Width = width;
        Height = height;
    }

    internal Image(long id, Guid resourceIdentifier, PublicId.PublicId publicId, DateTime createdAt, string name, string fileExtension, long fileSize, ushort width, ushort height)
    {
        Id = id;
        ResourceIdentifier = resourceIdentifier;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
        FileExtension = fileExtension;
        FileSize = fileSize;
        Width = width;
        Height = height;
    }

    public override long Id { get; protected set; }

    public IReadOnlyList<RoleAssignment<Image>> RoleAssignments => _roleAssignments.AsReadOnly();

    public Guid ResourceIdentifier { get; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public string FileExtension { get; }

    public long FileSize { get; }

    public ushort Width { get; }

    public ushort Height { get; }

    public RoleAssignment<Image> AddRoleAssignment(Role role, Principal principal)
    {
        var roleAssignment = new RoleAssignment<Image>(this, role, principal);
        _roleAssignments.Add(roleAssignment);

        return roleAssignment;
    }

    public void RemoveRoleAssignment(RoleAssignment<Image> roleAssignment)
    {
        _roleAssignments.Remove(roleAssignment);
    }
}
