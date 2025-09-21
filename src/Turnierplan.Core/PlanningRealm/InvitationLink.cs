using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Image;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class InvitationLink : Entity<long>, IEntityWithPublicId
{
    internal readonly List<InvitationLinkEntry> _entries = new();

    internal InvitationLink(long id, PublicId.PublicId publicId, string name, string? title, string? description, string colorCode, bool isActive, DateTime? validUntil, string? contactPerson, string? contactEmail, string? contactTelephone)
    {
        Id = id;
        PublicId = publicId;
        Name = name;
        Title = title;
        Description = description;
        IsActive = isActive;
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
        ColorCode = "aaaaaa";
        IsActive = true;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public PlanningRealm PlanningRealm { get; internal set; } = null!;

    public string Name { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string ColorCode { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ValidUntil { get; set; }

    public string? ContactPerson { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactTelephone { get; set; }

    public Image.Image? PrimaryLogo { get; internal set; }

    public Image.Image? SecondaryLogo { get; internal set; }

    public List<ExternalLink> ExternalLinks { get; set; } = new();

    public IReadOnlyList<InvitationLinkEntry> Entries => _entries.AsReadOnly();

    public InvitationLinkEntry AddEntry(TournamentClass tournamentClass)
    {
        if (!PlanningRealm._tournamentClasses.Contains(tournamentClass))
        {
            throw new TurnierplanException("Cannot add entry with a tournament class from another planning realm.");
        }

        var invitationLinkEntry = new InvitationLinkEntry(tournamentClass);
        _entries.Add(invitationLinkEntry);

        return invitationLinkEntry;
    }

    public void RemoveEntry(InvitationLinkEntry entry)
    {
        _entries.Remove(entry);
    }

    public void SetPrimaryLogo(Image.Image? primaryLogo)
    {
        CheckImageTypeAndSetImage(primaryLogo, () => PrimaryLogo = primaryLogo);
    }

    public void SetSecondaryLogo(Image.Image? secondaryLogo)
    {
        CheckImageTypeAndSetImage(secondaryLogo, () => SecondaryLogo = secondaryLogo);
    }

    public bool IsValidUntilSurpassed()
    {
        return ValidUntil.HasValue && ValidUntil.Value < DateTime.UtcNow;
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

        if (provided.Type != ImageType.Logo)
        {
            throw new TurnierplanException($"Cannot assign image because the image's type is not the expected type '{ImageType.Logo}'.");
        }

        apply();
    }

    public sealed record ExternalLink(string Name, string Url);
}
