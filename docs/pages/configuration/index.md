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

Um Bilder in einem AWS S3 oder S3-kompatiblen Bucket zu speichern, müssen die folgenden Umgebungsvariablen gesetzt werden:

| Umgebungsvariable               | Beschreibung                                                     |
|---------------------------------|------------------------------------------------------------------|
| `ImageStorage__Type`            | Muss `S3` sein.                                                  |
| `ImageStorage__RegionEndpoint`  | Der Name der AWS-Region, bspw. `eu-central-1`.                   |
| `ImageStorage__ServiceUrl`      | Die Service-URL, falls ein S3-kompatibler Bucket verwendet wird. |
| `ImageStorage__AccessKey`       | Der Name vom Access-Key.                                         |
| `ImageStorage__AccessKeySecret` | Der Schlüssel vom Access-Key.                                    |
| `ImageStorage__BucketName`      | Der Bucket-Name.                                                 |

Der verwendete Access-Key benötigt Rechte zum Erstellen, Lesen und Löschen von Objekten.

Die Eigenschaften `RegionEndpoint` und `ServiceUrl` schließen sich *gegenseitig aus*! Ersteres muss verwendet werden, wenn ein AWS S3-Bucket verwendet wird. Letztere muss verwendet werden, wenn ein S3-kompatibler Bucket von einem Drittanbieter verwendet wird.

### Azure Blob Storage

Um Bilder in einem [Azure Blob Storage](https://azure.microsoft.com/en-us/products/storage/blobs/) Container zu speichern, müssen die folgenden Umgebungsvariablen gesetzt werden:

| Umgebungsvariable                  | Beschreibung                                                |
|------------------------------------|-------------------------------------------------------------|
| `ImageStorage__Type`               | Muss `Azure` sein.                                          |
| `ImageStorage__StorageAccountName` | Der Name des Azure Blob Storage Account.                    |
| `ImageStorage__ContainerName`      | Der Name des Containers innerhalb vom o.g. Storage Account. |

Standardmäßig wird ein [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) verwendet. Falls also bspw. der turnierplan.NET-Container innerhalb eines Azure App Service betrieben wird, kann für diesen App Service eine Managed Identity erstellt und auf den Blob Storage Account berechtigt werden. Weitere Konfigurationsmöglichkeiten für Deployment-Szenarien, in denen keine Managed Identities verwendet werden können, sind nachfolgend beschrieben.

Sofern eine Entra ID-basierte Authentifizierung verwendet wird (dies betrifft alle Optionen außer Access Keys), muss die entsprechende Managed Identity bzw. App-Registrierung die Rechte zum Erstellen, Lesen und Löschen von Blobs innerhalb vom Storage Account haben. Dies kann am besten mit der Zuweisung der Rolle [Storage Blob Data Contributor](https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles/storage#storage-blob-data-contributor) erreicht werden.

#### Account Key

Die Erstellung von einem Account Key ist in der [Dokumentation von Microsoft](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-keys-manage?tabs=azure-portal) beschrieben. Um einen Account Key zu verwenden, müssen die folgenden Umgebungsvariablen *zusätzlich* zu den oben genannten gesetzt werden:

| Umgebungsvariable             | Beschreibung                                  |
|-------------------------------|-----------------------------------------------|
| `ImageStorage__UseAccountKey` | Muss `true` sein, um Acount Key zu verwenden. |
| `ImageStorage__AccountKey`    | Der eigentliche Account Key.                  |

#### Client Secret

Hierfür ist eine App-Registrierung innerhalb von Entra ID notwendig, welche wie o.g. die notwendigen Zugriffsrechte auf dem Blob Storage Account hat. Innerhalb der App-Registrierung muss zudem ein Client Secret angelegt werden. Um dieses zu verwenden, müssen die folgenden Umgebungsvariablen *zusätzlich* zu den oben genannten gesetzt werden:

| Umgebungsvariable               | Beschreibung                                                |
|---------------------------------|-------------------------------------------------------------|
| `ImageStorage__UseClientSecret` | Muss `true` sein, um Client Secret zu verwenden.            |
| `ImageStorage__TenantId`        | Die ID des Tenant, wo die App-Registrierung angelegt wurde. |
| `ImageStorage__ClientId`        | Die Client-ID der App-Registrierung.                        |
| `ImageStorage__ClientSecret`    | Der Wert des angelegten Client Secrets.                     |

## Authentifizierung

Die folgenden Einstellungen können gesetzt werden, um die Benutzerauthentifizierung von turnierplan.NET zu konfigurieren:

| Umgebungsvariable                | Beschreibung                                                                                                                                                 | Standard                    |
|----------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------|
| `Identity__AccessTokenLifetime`  | Die Gültigkeitsdauer von ausgestellten Access-Tokens.                                                                                                        | `00:03:00`                  |
| `Identity__RefreshTokenLifetime` | Die Gültigkeitsdauer von ausgestellten Refresh-Tokens. Innerhalb diesem Zeitraum ist kein erneuter Login erforderlich.                                       | `1.00:00:00`                |
| `Identity__StoragePath`          | Das Verzeichnis innerhalb vom Container, wo der Schlüssel zur Signatur ausgesteller Tokens gespeichert wird.                                                 | `/var/turnierplan/identity` |
| `Identity__UseInsecureCookies`   | Kann auf `true` gesetzt werde, um HTTP Cookies ohne *secure* auszustellen. Dies ist erforderlich, wenn nicht mit HTTPS auf turnierplan.NET zugegriffen wird. | `false`                     |

Für ein produktives Deployment sind die Standardwerte ausreichend und müssen nicht geändert werden.

!!! note
    Die Gültigkeitsdauer muss als .NET `TimeSpan` formatiert werden. Das Format ist `HH:mm:ss` bzw. `d.HH:mm:ss` also bspw. `00:03:00` für 3 Minuten oder `1.00:00:00` für 1 Tag.

## Monitoring

Der turnierplan.NET-Server kann Telemetriedaten (Logs, Metrics & Traces) an [Azure Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview) senden:

| Umgebungsvariable                       | Beschreibung                                                           | Standard |
|-----------------------------------------|------------------------------------------------------------------------|----------|
| `ApplicationInsights__ConnectionString` | Kann gesetzt werden, um Daten an Azure Application Insights zu senden. | -        |
