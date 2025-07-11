namespace Turnierplan.Core.NewFeature;

#if false

// Planungsumgebung => PlanningRealm

// Turnierklasse => TournamentClass

// Turnierklasse/Anmeldungslink => InvitationLinkEntry

// Anmeldungslink => InvitationLink

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

#endif
