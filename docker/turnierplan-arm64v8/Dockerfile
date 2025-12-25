FROM mcr.microsoft.com/dotnet/aspnet:10.0.1-alpine3.23-arm64v8 AS base

# Where are the files stored inside the container? This folder should be mapped as a volume
ARG DATA_DIRECTORY=/var/turnierplan

FROM mcr.microsoft.com/dotnet/sdk:10.0.101-noble-arm64v8 AS build

# Copy the source files and build the solution.
COPY src /src/
WORKDIR "/src/Turnierplan.App"
RUN dotnet build "./Turnierplan.App.csproj" -c Release -o /app/build

FROM build AS publish

# Install nodejs, this is required for the client app build which is part of the dotnet publish.
RUN curl -fsSL https://deb.nodesource.com/setup_24.x | bash - && apt-get install -y nodejs

# Publish the solution. By specifying the publish runtime 'linux-arm64', no unnecessary native libraries are included in the image
RUN dotnet publish "./Turnierplan.App.csproj" -c Release --runtime linux-arm64 -o /app/publish /p:UseAppHost=false

FROM base AS final

# Install some packages:
#  - 'gcompat'    to run QuestPDF dependencies on Alpine Linux
#  - 'krb5-libs'  to enable Npgsql GSS session encryption
#  - 'tzdata'     contains relevant time zone information
RUN apk add gcompat krb5-libs tzdata

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
