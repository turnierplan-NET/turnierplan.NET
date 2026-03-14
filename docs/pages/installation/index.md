---
icon: lucide/server
---

# Installation

Dieser Artikel beschreibt verschiedene Möglichkeiten, **turnierplan.NET** mithilfe des offiziellen Container-Images bereitzustellen.

## Container-Image

Innerhalb der GitHub-Organisation wird für jedes [Release](https://github.com/turnierplan-NET/turnierplan.NET/releases) das offizielle Container-Image veröffentlicht: [ghcr.io/turnierplan-net/turnierplan](https://github.com/turnierplan-NET/turnierplan.NET/pkgs/container/turnierplan)

Um turnierplan.NET lokal zu testen, kann der folgende Befehl verwendet werden:

```shell
docker run -p 80:8080 -e Database__InMemory="true" ghcr.io/turnierplan-net/turnierplan:latest
```

Die Weboberfläche kann über [localhost:80](http://localhost:80) erreicht werden. Der Benutzername und das initiale Passwort für den Administratorbenutzer werden in den Container-Logs angezeigt. Nach einem Neustart des Containers gehen allerdings alle Daten verloren, da eine in-memory Datenbank verwendet wird.

## Deployment

Für ein produktives Deployment gibt es nachfolgende Möglichkeiten basierend auf dem Container-Image. Weitere Methoden werden in der Zukunft aufgelistet.

### Docker Compose

Für die Installation auf einem lokalen Rechner oder einer VM kann im einfachsten Fall Docker Compose verwendet werden. Nachfolgend ein Minimalbeispiel für eine `docker-compose.yaml`:

```yaml
services:
  turnierplan.database:
    image: postgres:latest
    environment:
      - POSTGRES_PASSWORD=P@ssw0rd
      - POSTGRES_DB=turnierplan
    volumes:
      - turnierplan-database-data:/var/lib/postgresql
    networks:
      - turnierplan
    restart: unless-stopped

  turnierplan.application:
    image: ghcr.io/turnierplan-net/turnierplan:latest
    depends_on:
      - turnierplan.database
    environment:
      - Turnierplan__ApplicationUrl=http://localhost
      - Database__ConnectionString=Host=turnierplan.database;Database=turnierplan;Username=postgres;Password=P@ssw0rd
    volumes:
      - turnierplan-application-data:/var/turnierplan
    networks:
      - turnierplan
    restart: unless-stopped
    ports:
      - '80:8080'

volumes:
  turnierplan-application-data:
  turnierplan-database-data:

networks:
  turnierplan:
```

Statt dem `latest`-Tag sollte immer eine spezifische Version für die Images der Datenbank und von turnierplan.NET verwendet werden. Dies ermöglicht eine bessere Kontrolle, welche Updates wann eingespielt werden. Die neuste Version ist auf der [Release-Seite](https://github.com/turnierplan-NET/turnierplan.NET/releases) der Repository auffindbar.

Die URL, welche letztendlich für den Zugriff auf turnierplan.NET verwendet wird, sollte in der Umgebungsvariable `Turnierplan__ApplicationUrl` spezifiziert werden. Falls bspw. eine Domain `example.com` verwendet wird, sollte der Wert der Umgebungsvariable `https://example.com` sein. Falls turnierplan.NET im lokalen Netzwerk gehostet wird, könnte der Wert bspw. `http://192.168.0.187` sein. Es muss natürlich das korrekte Protokoll (HTTP vs. HTTPS) verwendet werden.

Die Volume-Mounts sind im [nachfolgenden Abschnitt](#volume-mounts) näher beschrieben. Je nach Konfiguration kann das Volume-Mount vom turnierlpan.NET-Container auch überflüssig sein.

Beim ersten Starten der Anwendung werden alle Datenbankmigrationen durchgeführt und es wird ein initialer Administrator-Benutzer angelegt. Die Zugangsdaten werden in den Container-Logs angezeigt. Mit diesen Zugangsdaten kann sich in der Weboberfläche unter [localhost:80](http://localhost:80) (bzw. je nach Konfiguration mit entsprechender Domain) eingeloggt werden.

!!! danger
    **Wichtige Sicherheitshinweise**:

    - Das Datenbankpasswort `POSTGRES_PASSWORD` in der Compose-Datei sollte durch ein zufällig generiertes Passwort ersetzt werden. Dementsprechend muss dann auch der Connection-String der Anwendung angepasst werden.
    - Der turnierplan.NET-Server sollte niemals ohne Reverse Proxy mit SSL-Terminierung im Internet erreichbar sein. Hierfür kann z.B. [nginx](https://nginx.org/) verwendet werden.

## Volume Mounts

Die turnierplan.NET-Anwendung speichert diverse Dateien in folgendem Verzeichnis innerhalb vom Container: `/var/turnierplan`. Bei einer Standardkonfiguration sollte dieses Verzeichnis in einem Docker Volume oder vergleichbar persistiert werden. Die folgenden Daten werden innerhalb vom o.g. Verzeichnis gespeichert:

- **Bild-Uploads**: Sofern keine anderweitige Speicherung von Bildern (wie z.B. S3) konfiguriert ist.
- **JWT Signatur-Schlüssel**: Sofern kein Schlüssel via Umgebungsvariable spezifiziert wird, generiert die Anwendung beim ersten Start einen zufälligen symmetrischen SHA512 Schlüssel zur Signatur der JWT-Tokens. Wenn der Schlüssel nicht persistiert wird, wird er bei jedem Start des Servers neu generiert und alle zuvor ausgestellten JWT-Tokens werden ungültig.

## Konfiguration

turnierplan.NET bietet zahlreiche Konfigurationsmöglichkeiten zur Anbindung von Externen System sowie zur Individualisierung. Alle Optionen können mit Umgebungsvariablen gesetzt werden und sind in der [Konfigurationsanleitung](./configuration) aufgelistet und beschrieben.

## Aktualisierung

Die verwendete Version von turnierplan.NET kann jederzeit auf eine neuere aktualisiert werden. Etwaige Datenbankmigrationen werden beim ersten Start sequenziell angewandt - auch wenn Versionen übersprungen werden. Allerdings sollten vor jeder Aktualisierung *immer* die [Release-Notes](https://github.com/turnierplan-NET/turnierplan.NET/releases) gelesen werden! Es kann jederzeit nicht-rückwärtskompatible Änderungen geben.

Zudem wird empfohlen, vor jeder Aktualisierung der turnierplan.NET-Anwendung *oder* der verwendeten PostgreSQL-Version ein Datenbankupdate zu erstellen. Dies kann z.B. mit dem [pg_dump](https://www.postgresql.org/docs/current/app-pgdump.html)-Tool gemacht werden.

## Fehlerbehebung

Nachfolgend beschrieben sind Fehler, welche bei einer Neuinstallation auftreten können.

### Verbindung per HTTP

Beim Zugriff auf einen nicht-lokalen turnierplan.NET-Server via HTTP sollte standardmäßig ein Fehler *401 Unauthorized* erscheinen. Dies liegt daran, dass turnierplan.NET für die Authentifizierung nach dem Login Cookies verwendet, welche standardmäßig als *secure* ausgestellt werden. Dies hat zur Folge, dass Browser den Cookie nur bei lokalen Verbindungen oder über HTTPS mitschicken. Um turnierplan.NET dennoch verwenden zu können, muss die `Identity__UseInsecureCookies` auf `true` gesetzt werden. Siehe auch [Konfiguration der Authentifizierung](configuration.md#authentifizierung).

!!! warning
    Die Verwendung von HTTP-Verbindungen über das Internet ist **absolut nicht empfohlen**, da persönliche Daten und Passwörter somit unverschlüsselt übertragen werden würden. Zudem ist nicht ausgeschlossen, dass Teile der Webanwendung nicht korrekt funktionieren, falls sie auf HTTPS-exklusiven Browser-APIs basieren (bspw. Zwischenablage oder *crypto*).
