# turnierplan.NET &middot; Docker Images

Currently, there exists one Dockerfile with the `dotnet/aspnet:10.0-alpine` base image for the amd64 architecture. To build the container image, run the following command from the *repository root*:

```shell
docker build -t turnierplan:dev -f docker/turnierplan-amd64/Dockerfile .
```

The resulting container image can be run as described in the [installation guide](https://docs.turnierplan.net/installation/#container-image). The following minimal example uses an in-memory data store and does not mount any volumes:

```shell
docker run -p 80:8080 -e Turnierplan__ApplicationUrl="http://localhost" -e Database__InMemory="true" turnierplan:dev
```
