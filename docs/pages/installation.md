# Installation

**turnierplan.NET** comes as a pre-built container image which can be deployed with minimal configuration. The image is available on GitHub: [ghcr.io/turnierplan-net/turnierplan](https://github.com/turnierplan-NET/turnierplan.NET/pkgs/container/turnierplan)

## Getting Started

In the simplest case, you can configure the container to use an in-memory data store. Note that this in-memory store is only meant for quick testing and is obviously not suitable for a production environment.

```shell
docker run -p 80:8080 -e Database__InMemory="true" ghcr.io/turnierplan-net/turnierplan:latest
```

You should see the following output. The credentials of the initial admin user are displayed in the container logs. You can now open up <a href="http://localhost" target="_blank">http://localhost</a> in your browser and log in using those credentials.

```
  __                                                     ___                                        __
 /\ \__                        __                       /\_ \                                      /\ \__
 \ \ ,_\  __  __  _ __    ___ /\_\     __   _ __   _____\//\ \      __      ___         ___      __\ \ ,_\
  \ \ \/ /\ \/\ \/\`'__\/' _ `\/\ \  /'__`\/\`'__\/\ '__`\\ \ \   /'__`\  /' _ `\     /' _ `\  /'__`\ \ \/
   \ \ \_\ \ \_\ \ \ \/ /\ \/\ \ \ \/\  __/\ \ \/ \ \ \L\ \\_\ \_/\ \L\.\_/\ \/\ \  __/\ \/\ \/\  __/\ \ \_
    \ \__\\ \____/\ \_\ \ \_\ \_\ \_\ \____\\ \_\  \ \ ,__//\____\ \__/.\_\ \_\ \_\/\_\ \_\ \_\ \____\\ \__\
     \/__/ \/___/  \/_/  \/_/\/_/\/_/\/____/ \/_/   \ \ \/ \/____/\/__/\/_/\/_/\/_/\/_/\/_/\/_/\/____/ \/__/
                                                     \ \_\
                                                      \/_/   v2025.4.0

info: Turnierplan.ImageStorage.Local.LocalImageStorage[0]
      Using the following directory for local image storage: '/var/turnierplan/images'
info: Turnierplan.App.DatabaseMigrator[0]
      An initial user was created: You can log in using "admin" and the password "53fe6bac-1050-4801-bb11-be2dbd479d66". IMMEDIATELY change this password when running in a production environment!
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://[::]:8080
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /app
```

## Persisting Data

The application stores the following data in the `/var/turnierplan` directory:

- `/var/turnierplan/identity/jwt-signing-key.bin` - The SHA512 signature key used to sign and verify JWT tokens.
- `/var/turnierplan/images/**` - If not configured otherwise (see [section below](#storing-images)), this folder will contain all uploaded image files.

Therefore, there should always be a volume mapping for this path.

## Environment Variables

For a basic installation, the following environment variables must be set:

| Environment Variable          | Description                                                  |
|-------------------------------|--------------------------------------------------------------|
| `Turnierplan__ApplicationUrl` | The URL used to access the website.                          |
| `Database__ConnectionString`  | The PostgreSQL connection string with read/write permission. |

The following environment variables can be set if you want to enable specific features or modify default behavior:

| Environment Variable                    | Description                                                                                                                                                                      | Default      |
|-----------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------|
| `ApplicationInsights__ConnectionString` | Can be set if you wish that your instance sends telemetry data to [Azure Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview). | -            |
| `Identity__AccessTokenLifetime`         | Defines the lifetime of issued JWT access tokens.                                                                                                                                | `00:03:00`   |
| `Identity__RefreshTokenLifetime`        | Defines the lifetime of issued JWT refresh tokens.                                                                                                                               | `1.00:00:00` |
| `Turnierplan__InstanceName`             | The instance name is displayed in the header/footer of the public pages. If not specified, the string `turnierplan.NET` will be shown instead.                                   | -            |
| `Turnierplan__LogoUrl`                  | The URL of the custom logo to be displayed in the header of the public pages. If not specified, the turnierplan.NET logo will be shown instead.                                  | -            |
| `Turnierplan__ImprintUrl`               | The URL of your external imprint page if you want it to be linked on the public pages.                                                                                           | -            |
| `Turnierplan__PrivacyUrl`               | The URL of your external privacy page if you want it to be linked on the public pages.                                                                                           | -            |
| `Turnierplan__ImageMaxSize`             | The maximum allowed file size when uploading an image file. The default value equates to 8 MiB (8 &middot; 1024 &middot; 1024)                                                   | `8388608`    |
| `Turnierplan__ImageQuality`             | Uploaded images are re-compressed using the `webp` format with the specified quality. A value of `100` will result in lossless compression being uesd.                           | `80`         |

!!! note
    The token lifetimes must be specified as .NET `TimeSpan` strings. For example `00:03:00` means 3 minutes or `1.00:00.00` means 1 day.

## Docker Compose

A minimal recommended configuration for a production environment is shown in the following docker compose example:

```yaml
services:
  turnierplan.database:
    image: postgres:latest
    environment:
      - POSTGRES_PASSWORD=P@ssw0rd
      - POSTGRES_DB=turnierplan
    volumes:
      - turnierplan-database-data:/var/lib/postgresql/data
    networks:
      - turnierplan
    restart: unless-stopped

  turnierplan.app:
    image: ghcr.io/turnierplan-net/turnierplan:latest
    depends_on:
      - turnierplan.database
    environment:
      - Turnierplan__ApplicationUrl=http://localhost
      - Database__ConnectionString=Host=turnierplan.database;Database=turnierplan;Username=postgres;Password=P@ssw0rd
    volumes:
      - turnierplan-app-data:/var/turnierplan
    networks:
      - turnierplan
    restart: unless-stopped
    ports:
      - '80:8080'

volumes:
  turnierplan-database-data:
  turnierplan-app-data:

networks:
  turnierplan:
```

!!! tip
    It is recommended to not use the latest tag. Rather, pin your docker services to a specific image version.

Feel free to modify the environment variables or add additional ones as described in the [environment variables](#environment-variables) section above.

## Storing Images

By default, all uploaded image files are stored in the `/var/turnierplan/images` directory. Whilst this is a very simple solution, it also means that the turnierplan.NET backend will serve all image files which can potentially lead to high load on the application server. Alternatively, you can configure the application to save image files to an external storage service. This way, clients can load the images directly from the external service.

!!! warning
    The differnt implementations do not necessarily use the same folder structure to organizie the files. Because of this, migrating from one image storage implementation to another can be difficult - so choose wisely!

The following implementations are currently available:

- **Local** - The default, which saves images in a local folder as described above
- **AWS S3** (or compatible)
- **Azure Blob Storage**

### Configuring AWS S3

To store uploaded images in an AWS S3 or S3-compatible bucket, add the following environment variables to your deployment:

| Environment Variable            | Description                                      |
|---------------------------------|--------------------------------------------------|
| `ImageStorage__Type`            | The image storage type, **must** be `S3`.        |
| `ImageStorage__RegionEndpoint`  | The AWS region endpoint, such as `eu-central-1`. |
| `ImageStorage__ServiceUrl`      | The service URL when using a non-AWS S3 bucket.  |
| `ImageStorage__AccessKey`       | The access key identifier.                       |
| `ImageStorage__AccessKeySecret` | The access key secret.                           |
| `ImageStorage__BucketName`      | The name of the bucket.                          |

The access key must have permissions to create, read and delete objects. 

The `RegionEndpoint` and `ServiceUrl` variables are *mutually exclusive*. Use the former if you are using an AWS S3 bucket and use the latter if you use a S3-compatible bucket from a third party.

### Configuring Azure Blob Storage

To store uploaded images in an [Azure Blob Storage](https://azure.microsoft.com/en-us/products/storage/blobs/) container, add the following environment variables to your deployment:

| Environment Variable               | Description                                  |
|------------------------------------|----------------------------------------------|
| `ImageStorage__Type`               | The image storage type, **must** be `Azure`. |
| `ImageStorage__StorageAccountName` | The name of the storage account.             |
| `ImageStorage__ContainerName`      | The name of the blob container.              |

By default, a [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) will be used. Therefore, if you use Azure Managed Identities, you won't have to do any further configuration. In addition, this implementation supports two additional means of authentication listed below.

When using Entra ID based authentication, the managed identity / app registration must have permission to create/read/delete blobs in the storage account. This can be achieved by assigning the [Storage Blob Data Contributor](https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles/storage#storage-blob-data-contributor) role.

#### Access key authentication

Refer to the [documentation](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-keys-manage?tabs=azure-portal) on how to view and manage the access keys.

The following environment variables must be set to enable access key authentication:

| Environment Variable          | Description                                      |
|-------------------------------|--------------------------------------------------|
| `ImageStorage__UseAccountKey` | Set to `true` to use account key authentication. |
| `ImageStorage__AccountKey`    | The value of the account key.                    |

#### Client secret authentication

If you have an Entra ID app registration with the necessary permissions on the storage account, you can set the following environment variables to enable client secret authentication:

| Environment Variable            | Description                                             |
|---------------------------------|---------------------------------------------------------|
| `ImageStorage__UseClientSecret` | Set to `true` to use client credentials authentication. |
| `ImageStorage__TenantId`        | The tenant id where the app registration resides.       |
| `ImageStorage__ClientId`        | The client id of the *app registration*.                |
| `ImageStorage__ClientSecret`    | The value of the client secret.                         |
