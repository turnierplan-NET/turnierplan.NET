---
icon: lucide/rocket
---

# Erste Schritte

Nach einer Neuinstallation von turnierplan.NET gibt es zunächst nur einen Administratorbenutzer. Für produktive Anwendungen ist es dringend empfohlen, einen nicht-Adminnutzer anzulegen, und diesen für den täglichen Login zu verwenden. Dies wird im folgenden Abschnitt beschrieben. Es können jedoch auch mit dem Administratorbenutzer Turniere angelegt werden. Dies ist im Abschnitt weiter unten beschrieben.

## Benutzer anlegen

Zunächst muss sich mit den Zugangsdaten des Administratorbenutzers angemeldet werden. Auf der Startseite ist folgend der *Administrator*-Button sichtbar, welcher zum Administrator-Portal führt. Dort kann ein neuer Benutzer angelegt werden. Zwingend angegeben werden müssen der Benutzername und das Passwort des neuen Benutzers. Nachdem ein Benutzer erstellt wurde, muss ihm noch die Berechtigung erteilt werden, Organisationen zu erstellen. Hierfür muss man in der Zeile des Benutzers auf das Bearbeiten-Symbol drücken, den Haken bei *Benutzer darf neue Organisationen anlegen* setzen und speichern. Anschließend kann der Benutzer sich anmelden und eigenständig neue Organisationen erstellen.

## Organisation erstellen

Alle Turniere und andere Objekte, welche innerhalb von turnierplan.NET erstellt werden können, sind immer einer Organisation zugehörig. Eine neue Organisation kann jederzeit auf der Startseite erstellt werden und benötigt lediglich einen Namen.

!!! tip
    Durch die Trennung der Daten in mehrere Organisationen kann gesteuert werden, welche Benutzer innerhalb von welchen Organisationen Daten lesen und Änderungen vornehmen können.

Für den Start genügt zunächst eine einzelne Organisation.

## Turnier erstellen

Ein neues Turnier kann innerhalb einer bestehenden Organisation mit der Schaltfläche *Neues Turnier* erstellt werden:

![Übersichtsseite einer leeren Organisation](images/empty-organization.png)

In der Eingabemaske, welche sich darauf öffnet, müssen folgende Informationen bereitgestellt werden:

- **Name**: Kann frei gewählt werden.
- **Ordner**: Kann optional verwendet werden, um mehrere Turniere zu gruppieren. Dies hat diverse Vorteile, welche separat beschrieben werden.
- **Sichtbarkeit**: Wenn ein Turnier auf *öffentlich* gestellt wird, kann jeder das Turnier mit einem speziellen Link sehen. Wenn das Turnier auf *privat* gestellt wird, kann das Turnier nur als angemeldeter Benutzer gesehen werden.

![Eingabemaske für ein neues Turnier](images/new-tournament.png)

Alle o.g. Informationen können nachträglich geändert werden. Nach der Bestätigung der Eingaben öffnet sich die Konfigurationsseite des neu erstellen Turniers:
