using Turnierplan.Core.Entity;

namespace Turnierplan.Core.Document;

public sealed class Document : Entity<long>, IEntityWithPublicId
{
    public Document(Tournament.Tournament tournament, DocumentType type, string name, string configuration)
    {
        tournament._documents.Add(this);

        Id = 0;
        PublicId = new PublicId.PublicId();
        Tournament = tournament;
        Type = type;
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
        Name = name;
        Configuration = configuration;
        GenerationCount = 0;
    }

    internal Document(long id, PublicId.PublicId publicId, DocumentType type, DateTime createdAt, DateTime lastModifiedAt, string name, string configuration, int generationCount)
    {
        Id = id;
        PublicId = publicId;
        Type = type;
        CreatedAt = createdAt;
        LastModifiedAt = lastModifiedAt;
        Name = name;
        Configuration = configuration;
        GenerationCount = generationCount;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public Tournament.Tournament Tournament { get; internal set; } = null!;

    public DocumentType Type { get; }

    public DateTime CreatedAt { get; private set; }

    public DateTime LastModifiedAt { get; private set; }

    public string Name { get; set; }

    /// <remarks>
    /// The configuration is stored as a JSON <c>string</c> rather than being converted
    /// to/from by EF. This is done because the actual class which represents the config
    /// is not known at the time of deserialization.
    /// </remarks>
    public string Configuration { get; private set; }

    public int GenerationCount { get; private set; }

    public DateTime? LastGeneration { get; private set; }

    public void UpdateConfiguration(string configuration)
    {
        Configuration = configuration;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void IncreaseGenerationCount()
    {
        GenerationCount++;
        LastGeneration = DateTime.UtcNow;
    }
}
