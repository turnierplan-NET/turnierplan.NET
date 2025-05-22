using Turnierplan.Core.Exceptions;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Image;

public sealed class Image : Entity<long>, IEntityWithPublicId, IEntityWithOwner
{
    public Image(Organization.Organization organization, string name, ImageType type, string fileType, long fileSize, ushort width, ushort height)
    {
        ValidateImageSize(type, width, height);

        organization._images.Add(this);

        Id = 0;
        ResourceIdentifier = Guid.NewGuid();
        PublicId = new PublicId.PublicId();
        CreatedAt = DateTime.UtcNow;
        Name = name;
        Type = type;
        FileType = fileType;
        FileSize = fileSize;
        Width = width;
        Height = height;
    }

    internal Image(long id, Guid resourceIdentifier, PublicId.PublicId publicId, DateTime createdAt, string name, ImageType type, string fileType, long fileSize, ushort width, ushort height)
    {
        Id = id;
        ResourceIdentifier = resourceIdentifier;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
        Type = type;
        FileType = fileType;
        FileSize = fileSize;
        Width = width;
        Height = height;
    }

    public override long Id { get; protected set; }

    public Guid ResourceIdentifier { get; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    Guid IEntityWithOwner.OwnerId => Organization.OwnerId;

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public ImageType Type { get; }

    public string FileType { get; }

    public long FileSize { get; }

    public ushort Width { get; }

    public ushort Height { get; }

    private static void ValidateImageSize(ImageType type, ushort width, ushort height)
    {
        var constraints = ImageConstraints.GetImageConstraints(type);

        if (!constraints.IsSizeValid(width, height))
        {
            throw new TurnierplanException($"Image with size {width}x{height} does not meet criteria of type {type}: {constraints}");
        }
    }
}
