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
    //  - max. Mannschaften pro Anmeldung                                                  | Sollte dies evtl. für jeden Link individuell festgelegt werden können? Eher nein?
    //  - max. Mannschaften insgesamt                                                      | Sollte dies evtl. für jeden Link individuell festgelegt werden können? Eher nein?
    //  - Anmeldung offen/geschlossen -> wird auf der Anmeldeseite entsprechend angezeigt  | Sollte dies evtl. für jeden Link individuell festgelegt werden können? Eher nein?
    // später Bezug zu den verknüpften Turnieren
}

// Zwischen Turnierklasse & Anmeldungslink ggf. ein "Zwischenobjekt" um "max Mannschaften insg." & "offen/geschlosen" & "anmeldeschluss" dynamischer abzubilden?

public sealed class Anmeldungslink
{
    public PublicId.PublicId PublicId;
    // Logo Image (+ sponsor logo?)
    // URL für Durchführungsbedingungen/Anmeldebedingungen, evtl. auch markdown/HTML?
    // opt. E-Mail / Telefon (allg. Kontaktdaten) für Rückfragen
    // Anmeldeschluss | Sollte dies evtl. für jede Altersklasse indivudell sein? Eher nein?
    public List<Turnierklasse> Turnierklassen = new();
}

public sealed class Anmeldung
{
    public int ShortId; // a short ID, random 4 digit & unique in the context of a "Planungsumgebung" -> in addition to the primary key ID
    public Anmeldungslink Anmeldungslink; // nullable, wenn anmeldung "manuell" angelegt wird
    public DateTime CreatedAt;
    public string Name;
    public string EMail;
    public string Telephone;
    public List<AnmeldungsMannschaft> Mannschaften = new();
    public string Comment; // wird beim absenden der anmeldung optional angegeben
    public string Notes; // kann vom angemeldeten nutzer optional verwendet werden für eigene notizen
}

public sealed class AnmeldungsMannschaft
{
    public Turnierklasse Turnierklasse;
    public string Verein;
    // später Bezug zum "bestehenden" Team
}

/*
 * sonstiges:
 * - captcha im anmeldeformular
 * - wenn max. mannschaften pro anmeldung = 1, steht im formular keine eingabe für die anzahl zur verfügung
 */
