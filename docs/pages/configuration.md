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

| Umgebungsvariable                  | Beschreibung                                                                                                                                                                  | Standard  |
|------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------|
| `Turnierplan__InstanceName`        | Dieser Name wird in der Kopfzeile und Fußzeile von den öffentlichen Seiten angezeigt. Falls nicht spezifiert, wird der Text `turnierplan.NET` angezeigt.                      | -         |
| `Turnierplan__LogoUrl`             | Die URL für das Vereins-/Firmenlogo, welches in der Kopfzeile von öffentlichen Seiten angezeigt werden soll. Falls nicht spezifiert, wird das turnierplan.NET-Logo angezeigt. | -         |
| `Turnierplan__ImprintUrl`          | Die URL für den Verweis auf ein externes Impressum, welches bspw. auf Ihrer Vereins-/Firmenseite gehostet ist.                                                                | -         |
| `Turnierplan__PrivacyUrl`          | Die URL für den Verweis auf eine externe Datenschutz-Seite, welche bspw. auf Ihrer Vereins-/Firmenseite gehostet ist.                                                         | -         |
| `Turnierplan__ImageMaxSize`        | Die maximale Dateigröße für Bild-Uploads in Bytes. Der Standard-Wert entspricht 8 MiB (8 &middot; 1024 &middot; 1024)                                                         | `8388608` |
| `Turnierplan__ImageQuality`        | Die Qualitätseinstellung für Bild-Uploads. Ein Wert von `100` entspricht einer verlustfreien Komprimierung. Verwendet wird das `webp`-Format.                                 | `80`      |
