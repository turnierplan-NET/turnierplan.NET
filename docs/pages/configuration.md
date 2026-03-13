---
icon: lucide/wrench
---

# Konfiguration

Unabhängig vom Deployment und der verwendeten Hardware/Server bietet turnierplan.NET zahlreiche Konfigurationsmöglichkeiten, welche nachfolgend näher beschrieben sind.

## Erforderliche Einstellungen

Für eine produktive Installation müssen die folgenden Umgebungsvariablen zwingend gesetzt sein:

| Umgebungsvariable             | Beschreibung                                                       |
|-------------------------------|--------------------------------------------------------------------|
| `Turnierplan__ApplicationUrl` | Die URL, welche für den Web-Zugriff auf den Server verwendet wird. |
| `Database__ConnectionString`  | Connection-String für die PostgreSQL-Datenbank.                    |

## Allgemeine Einstellungen

Die folgenden Einstellungen können gesetzt werden, um das allgemeine Aussehen und Verhalten von turnierplan.NET zu konfigurieren:

| Umgebungsvariable                  | Beschreibung                                                                                                                                                                         | Standard  |
|------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------|
| `Turnierplan__InstanceName`        | Dieser Name wird in der Kopfzeile und Fußzeile von den öffentlichen Seiten angezeigt. Falls nicht spezifiert, wird der Text `turnierplan.NET` angezeigt.                             | -         |
| `Turnierplan__LogoUrl`             | Die URL für das Vereins-/Firmenlogo, welches in der Kopfzeile von öffentlichen Seiten angezeigt werden soll. Falls nicht spezifiert, wird das turnierplan.NET-Logo angezeigt.        | -         |
| `Turnierplan__ImprintUrl`          | Die URL für den Verweis auf ein externes Impressum, welches bspw. auf Ihrer Vereins-/Firmenseite gehostet ist.                                                                       | -         |
| `Turnierplan__PrivacyUrl`          | Die URL für den Verweis auf eine externe Datenschutz-Seite, welche bspw. auf Ihrer Vereins-/Firmenseite gehostet ist.                                                                | -         |
| `Turnierplan__ImageMaxSize`        | Die maximale Dateigröße für Bild-Uploads in Bytes. Der Standard-Wert entspricht 8 MiB (8 &middot; 1024 &middot; 1024)                                                                | `8388608` |
| `Turnierplan__ImageQuality`        | Die Qualitätseinstellung für Bild-Uploads. Ein Wert von `100` entspricht einer verlustfreien Komprimierung. Verwendet wird das `webp`-Format.                                        | `80`      |
| `Turnierplan__InitialUserName`     | Der Benutzername für den initalen Administratorbenutzer. Sofern nicht angegeben, wird der Benutzername von der Anwendung vorgegeben und beim ersten Start in der Konsole ausgegeben. | -         |
| `Turnierplan__InitialUserPassword` | Das Passwort für den initialen Administratorbenutzer. Sofern nicht angegeben, wird beim ersten Start der Anwendung ein zufälliges Passwort generiert und in der Konsole ausgegeben.  | -         |

## Monitoring

Der turnierplan.NET-Server kann Telemetriedaten (Logs, Metrics & Traces) an Azure Application Insights senden:

| Umgebungsvariable                       | Beschreibung                                                                                                                                              | Standard |
|-----------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|----------|
| `ApplicationInsights__ConnectionString` | Kann gesetzt werden, um Daten an [Azure Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview) zu senden. | -        |

## Authentifizierung

Die folgenden Einstellungen können gesetzt werden, um die Benutzerauthentifizierung von turnierplan.NET zu konfigurieren:

| Umgebungsvariable                | Beschreibung                                                                                                                                                 | Standard                    |
|----------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------|
| `Identity__AccessTokenLifetime`  | Die Gültigkeitsdauer von ausgestellten Access-Tokens.                                                                                                        | `00:03:00`                  |
| `Identity__RefreshTokenLifetime` | Die Gültigkeitsdauer von ausgestellten Refresh-Tokens. Innerhalb diesem Zeitraum ist kein erneuter Login erforderlich.                                       | `1.00:00:00`                |
| `Identity__StoragePath`          | Das Verzeichnis innerhalb vom Container, wo der Schlüssel zur Signatur ausgesteller Tokens gespeichert wird.                                                 | `/var/turnierplan/identity` |
| `Identity__UseInsecureCookies`   | Kann auf `true` gesetzt werde, um HTTP Cookies ohne *secure* auszustellen. Dies ist erforderlich, wenn nicht mit HTTPS auf turnierplan.NET zugegriffen wird. | `false`                     |

!!! note
    Die Gültigkeitsdauer muss als .NET `TimeSpan` formatiert werden. Das Format ist `HH:mm:ss` bzw. `d.HH:mm:ss` also bspw. `00:03:00` für 3 Minuten oder `1.00:00:00` für 1 Tag.

## Bilder-Uploads

Standardmäßig werden alle Bilder-Uploads als Dateien in einem Container-Verzeichnis gespeichert. Das entsprechende Verzeichnis sollte [als Volume persistiert](http://localhost:8000/installation/#volume-mounts) werden.

| Umgebungsvariable           | Beschreibung                                                                     | Standard                  |
|-----------------------------|----------------------------------------------------------------------------------|---------------------------|
| `ImageStorage__StoragePath` | Das Verzeichnis innerhalb vom Container, hochgeladene Bilder gespeichert werden. | `/var/turnierplan/images` |

Alternativ können externe Services zum Speichern der Bilder konfiguriert werden. Dies hat den Vorteil, dass das Bereitstellen von Bilddateien keine CPU- und Netzwerkresourcen vom turnierplan.NET-Server beansprucht. Aktuell werden die folgenden externen Services unterstützt:

- **AWS S3** (oder kompatibler Dienst)
- **Azure Blob Storage**

!!! warning
    Die nachfolgend vorgestellten Alternativen verwenden nicht zwangsläufig eine identische Verzeichnisstruktur zur Organisation der Dateien. Dadurch wird eine nachträgliche Umstellung ggf. erschwert!

### AWS S3

todo

### Azure Blob Storage

todo

