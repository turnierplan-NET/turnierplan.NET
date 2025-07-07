namespace Turnierplan.Core.NewFeature;

// EntityWithPublicId
// EntityWithRoleAssignments
public sealed class Planungsumgebung
{
    // Organization (parent org)
    public string Name;
    public DateTime CreatedAt;
    public List<Turnierklasse> Turnierklassen = new();
    public List<Anmeldungslink> Anmeldungslinks = new();
    public List<Anmeldung> Anmeldungen = new();
}

public sealed class Turnierklasse
{
    public string Name;
    // Konfigurationsoptionen:
    //  - max. Mannschaften pro Anmeldung
    // später Bezug zu den verknüpften Turnieren
}

public sealed class Anmeldungslink
{
    public PublicId.PublicId PublicId;
    // Logo Image (+ sponsor logo?)
    // URL für Anmeldebedingungen, evtl. auch markdown/HTML?
    public List<Turnierklasse> Turnierklassen = new();
}

public sealed class Anmeldung
{
    public Anmeldungslink Anmeldungslink; // nullable, wenn anmeldung "manuell" angelegt wird
    public DateTime CreatedAt;
    public string Name;
    public string EMail;
    public string Telephone;
    public List<AnmeldungsMannschaft> Mannschaften = new();
}

public sealed class AnmeldungsMannschaft
{
    public Turnierklasse Turnierklasse;
    public string Verein;
    // später Bezug zum "bestehenden" Team
}
