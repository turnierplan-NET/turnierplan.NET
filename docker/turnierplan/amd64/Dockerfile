FROM mcr.microsoft.com/dotnet/aspnet:10.0.1-noble-amd64 AS base

# Where are the files stored inside the container? This folder should be mapped as a volume
ARG DATA_DIRECTORY=/var/turnierplan

FROM mcr.microsoft.com/dotnet/sdk:10.0.101-noble-amd64 AS build

# Copy the source files and build the solution.
COPY src /src/
WORKDIR "/src/Turnierplan.App"
RUN dotnet build "./Turnierplan.App.csproj" -c Release -o /app/build

FROM build AS publish

# Install nodejs, this is required for the client app build which is part of the dotnet publish.
RUN curl -fsSL https://deb.nodesource.com/setup_24.x | bash - && apt-get install -y nodejs

# Publish the solution. By specifying the publish runtime 'linux-x64', no unnecessary native libraries are included in the image
RUN dotnet publish "./Turnierplan.App.csproj" -c Release --runtime linux-x64 -o /app/publish /p:UseAppHost=false

FROM base AS final

# Since .NET 8, the official container image no longer includes the 'Kerberos' package. Since Npgsql 10.0, an
# error is shown upon startup if that package is not installed. Because of this, we install the package manually.
# The official documentation states you have to install the 'libkrb5-3' package:
#   - https://learn.microsoft.com/en-us/dotnet/core/compatibility/containers/8.0/krb5-libs-package
# However, this seems to have no effect. By digging in the 'dotnet-docker' repository, you can find the Dockerfile
# used for the .NET 6 images which states the 'libgssapi-krb5-2' package which is installed below.
#   - source: https://github.com/dotnet/dotnet-docker/blob/0a4258cc250885be5162d01f178bff5c856291f4/src/runtime-deps/6.0/jammy/amd64/Dockerfile
RUN apt-get update \
    && apt-get -y install libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

# Prepare the directory used by the turnierplan.NET application
RUN mkdir "$DATA_DIRECTORY" && chown "$APP_UID" "$DATA_DIRECTORY"

# Miscellaneous configurations
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy the publish output i.e. the application binaries and the static web assets including the client application
COPY --from=publish /app/publish .

# Set the environment variables based on the data directory
ENV Identity__StoragePath=$DATA_DIRECTORY/identity
ENV ImageStorage__StoragePath=$DATA_DIRECTORY/images

# Entrypoint is the DLL of the Turnierplan.App project
ENTRYPOINT ["dotnet", "Turnierplan.App.dll"]
