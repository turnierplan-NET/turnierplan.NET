using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Planning;

public sealed class InvitationLink : Entity<long>, IEntityWithPublicId
{
    internal readonly List<InvitationLinkEntry> _entries = new();

    internal InvitationLink(long id, PublicId.PublicId publicId, string? title, string? description, string colorCode, DateTime? validUntil, string? contactPerson, string? contactEmail, string? contactTelephone)
    {
        Id = id;
        PublicId = publicId;
        Title = title;
        Description = description;
        ColorCode = colorCode;
        ValidUntil = validUntil;
        ContactPerson = contactPerson;
        ContactEmail = contactEmail;
        ContactTelephone = contactTelephone;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

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
}
