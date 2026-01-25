# Installation

**turnierplan.NET** comes as a pre-built container image which can be deployed with minimal configuration. The image is available on GitHub: [ghcr.io/turnierplan-net/turnierplan](https://github.com/turnierplan-NET/turnierplan.NET/pkgs/container/turnierplan)

In the simplest case, you can configure the container to use an in-memory data store. Note that this in-memory store is only meant for quick testing and is *not stable* for production! 

```shell
docker run -p 80:8080 -e Turnierplan__ApplicationUrl="http://localhost" -e Database__InMemory="true" ghcr.io/turnierplan-net/turnierplan:latest
```

A PostgreSQL database can be configured by specifying the `Database__ConnectionString` environment variable:

```shell
docker run -p 80:8080 -e Turnierplan__ApplicationUrl="http://localhost" -e Database__ConnectionString="<connection_string>" ghcr.io/turnierplan-net/turnierplan:latest
```

The credentials of the initial admin user are displayed in the container logs.

> [!CAUTION]
> In a production environment, you should immediately change the administrator password to a secure one!

### Persisting Data

To persist the **turnierplan.NET** application data, create a Docker volume mapping to the `/var/turnierplan` folder inside the container.

> [!CAUTION]
> This folder contains the JWT signing key for issued access/refresh tokens.

### Environment Variables

For a basic installation, the following environment variables *must* be set:

| Environment Variable          | Description                                                  |
|-------------------------------|--------------------------------------------------------------|
| `Turnierplan__ApplicationUrl` | The URL used to access the website.                          |
| `Database__ConnectionString`  | The PostgreSQL connection string with read/write permission. |

The following environment variables *can* be set if you want to enable specific features or modify default behavior:

| Environment Variable                    | Description                                                                                                                                                                      | Default      |
|-----------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------|
| `ApplicationInsights__ConnectionString` | Can be set if you wish that your instance sends telemetry data to [Azure Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview). | -            |
| `Identity__AccessTokenLifetime`         | Defines the lifetime of issued JWT access tokens.                                                                                                                                | `00:03:00`   |
| `Identity__RefreshTokenLifetime`        | Defines the lifetime of issued JWT refresh tokens.                                                                                                                               | `1.00:00:00` |
| `Turnierplan__InstanceName`             | The instance name is displayed in the header/footer of the public pages. If not specified, `turnierplan.NET` will be shown instead.                                              | -            |
| `Turnierplan__LogoUrl`                  | The URL of the custom logo to be displayed in the header of the public pages. If not specified, the turnierplan.NET logo will be shown instead.                                  | -            |
| `Turnierplan__ImprintUrl`               | The URL of your external imprint page if you want it to be linked on the public pages.                                                                                           | -            |
| `Turnierplan__PrivacyUrl`               | The URL of your external privacy page if you want it to be linked on the public pages.                                                                                           | -            |

> The token lifetimes must be specified as .NET `TimeSpan` strings. For example `00:03:00` means 3 minutes or `1.00:00.00` means 1 day.

### Docker Compose Example

You can use the following docker compose file to get a complete instance running on your machine:

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

> [!TIP]
> It is recommended to *not* use the `latest` tag. Rather, pin your docker services to a specific image version.

### Storing images in AWS S3

If you prefer to store uploaded images in an AWS S3 or S3-compatible bucket, add the following environment variables to your deployment:

| Environment Variable            | Description                                      |
|---------------------------------|--------------------------------------------------|
| `ImageStorage__Type`            | The image storage type, **must** be `S3`.        |
| `ImageStorage__RegionEndpoint`  | The AWS region endpoint, such as `eu-central-1`. |
| `ImageStorage__ServiceUrl`      | The service URL when using a non-AWS S3 bucket.  |
| `ImageStorage__AccessKey`       | The access key identifier.                       |
| `ImageStorage__AccessKeySecret` | The access key secret.                           |
| `ImageStorage__BucketName`      | The name of the bucket.                          |

The access key must have permissions to create, read and delete objects.

> [!NOTE]
> The `RegionEndpoint` and `ServiceUrl` variables are *mutually exclusive*. Use the former if you are using an AWS S3 bucket and use the latter if you use a S3-compatible bucket from a third party.

### Storing images in Azure Blob Storage

If you prefer to store uploaded images in Microsoft [Azure Blob Storage](https://azure.microsoft.com/en-us/products/storage/blobs/), add the following environment variables to your deployment:

| Environment Variable               | Description                                  |
|------------------------------------|----------------------------------------------|
| `ImageStorage__Type`               | The image storage type, **must** be `Azure`. |
| `ImageStorage__StorageAccountName` | The name of the storage account.             |
| `ImageStorage__ContainerName`      | The name of the blob container.              |

By default, a [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) will be used. Therefore, if you use Azure Managed Identities, you won't have to do any further configuration. In addition, this implementation supports two additional means of authentication listed below.

When using Entra ID based authentication, the managed identity / app registration must have permission to create/read/delete blobs in the storage account. This can be achieved by assigning the [Storage Blob Data Contributor](https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles/storage#storage-blob-data-contributor) role.

#### Authenticating using access key

Refer to the [documentation](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-keys-manage?tabs=azure-portal) on how to view and manage the access keys.

The following environment variables must be set to enable access key authentication:

| Environment Variable          | Description                                      |
|-------------------------------|--------------------------------------------------|
| `ImageStorage__UseAccountKey` | Set to `true` to use account key authentication. |
| `ImageStorage__AccountKey`    | The value of the account key.                    |

#### Authenticate using client secret

If you have an Entra ID app registration with the necessary permissions on the storage account, you can set the following environment variables to enable client secret authentication:

| Environment Variable            | Description                                             |
|---------------------------------|---------------------------------------------------------|
| `ImageStorage__UseClientSecret` | Set to `true` to use client credentials authentication. |
| `ImageStorage__TenantId`        | The tenant id where the app registration resides.       |
| `ImageStorage__ClientId`        | The client id of the *app registration*.                |
| `ImageStorage__ClientSecret`    | The value of the client secret.                         |
