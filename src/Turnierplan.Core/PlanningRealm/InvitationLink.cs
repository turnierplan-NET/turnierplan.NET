using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Image;
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

    public PlanningRealm PlanningRealm { get; internal set; } = null!;

    public string Name { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string ColorCode { get; set; }

    public DateTime? ValidUntil { get; set; }

    public string? ContactPerson { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactTelephone { get; set; }

    public Image.Image? PrimaryLogo { get; internal set; }

    public Image.Image? SecondaryLogo { get; internal set; }

    public List<ExternalLink> ExternalLinks { get; set; } = new();

    public IReadOnlyList<InvitationLinkEntry> Entries => _entries.AsReadOnly();

    public void SetPrimaryLogo(Image.Image? primaryLogo)
    {
        CheckImageTypeAndSetImage(primaryLogo, () => PrimaryLogo = primaryLogo);
    }

    public void SetSecondaryLogo(Image.Image? secondaryLogo)
    {
        CheckImageTypeAndSetImage(secondaryLogo, () => SecondaryLogo = secondaryLogo);
    }

    private void CheckImageTypeAndSetImage(Image.Image? provided, Action apply)
    {
        if (provided is null)
        {
            apply();
            return;
        }

        if (provided.Organization != PlanningRealm.Organization)
        {
            throw new TurnierplanException("Cannot assign an image from another organization.");
        }

        if (provided.Type != ImageType.SquareLargeLogo)
        {
            throw new TurnierplanException($"Cannot assign image because the image's type is not the expected type '{ImageType.SquareLargeLogo}'.");
        }

        apply();
    }

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

    public sealed record ExternalLink(string Name, string Url);
}
