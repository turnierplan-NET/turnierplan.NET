FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ARG DATA_DIRECTORY=/var/turnierplan
RUN mkdir "$DATA_DIRECTORY" && chown "$APP_UID" "$DATA_DIRECTORY"
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
WORKDIR "/src/Turnierplan.App"
RUN dotnet build "./Turnierplan.App.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN curl -fsSL https://deb.nodesource.com/setup_24.x | bash - && apt-get install -y nodejs

# By specifying the publish runtime 'linux-x64', no unnecessary native libraries are included in the image
RUN dotnet publish "./Turnierplan.App.csproj" -c "$BUILD_CONFIGURATION" --runtime linux-x64 -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV Identity__StoragePath=$DATA_DIRECTORY/identity
ENV ImageStorage__StoragePath=$DATA_DIRECTORY/images
ENTRYPOINT ["dotnet", "Turnierplan.App.dll"]
