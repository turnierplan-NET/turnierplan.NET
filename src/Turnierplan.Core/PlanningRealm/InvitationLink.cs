using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class InvitationLink : Entity<long>, IEntityWithPublicId
{
    internal readonly List<InvitationLinkEntry> _entries = new();

    internal InvitationLink(long id, PublicId.PublicId publicId, string name, string? title, string? description, string colorCode, DateTime? validUntil, string? contactPerson, string? contactEmail, string? contactTelephone)
    {
        Id = id;
        PublicId = publicId;
        Name = name;
        Title = title;
        Description = description;
        ColorCode = colorCode;
        ValidUntil = validUntil;
        ContactPerson = contactPerson;
        ContactEmail = contactEmail;
        ContactTelephone = contactTelephone;
    }

    internal InvitationLink(string name)
    {
        Id = 0;
        PublicId = new PublicId.PublicId();
        Name = name;
        ColorCode = GenerateRandomColorCode();
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public string Name { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string ColorCode { get; set; }

    public DateTime? ValidUntil { get; }

    public string? ContactPerson { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactTelephone { get; set; }

    public Image.Image? PrimaryLogo { get; internal set; }

    public Image.Image? SecondaryLogo { get; internal set; }

    public List<ExternalLink> ExternalLinks { get; set; } = new();

    public IReadOnlyList<InvitationLinkEntry> Entries => _entries.AsReadOnly();

    public sealed record ExternalLink(string Name, string Url);

    private static string GenerateRandomColorCode()
    {
        string[] parts =
        [
            Random.Shared.Next(2) == 0 ? "ff" : "bb",
            Random.Shared.Next(2) == 0 ? "55" : "99",
            "00"
        ];

        Random.Shared.Shuffle(parts);

        return string.Join(string.Empty, parts);
    }
}
