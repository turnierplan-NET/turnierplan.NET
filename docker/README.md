# turnierplan.NET &middot; Docker Images

Currently, there exists one Dockerfile with the `dotnet/aspnet:10.0-alpine` base image for the amd64 architecture. To build the container image, run the following command from the *repository root*:

```shell
docker build -t turnierplan:dev -f docker/turnierplan-amd64/Dockerfile .
```

> ![WARNING] Manually built container images should not be used in production! This is because the relevant GitHub workflow performs other important steps before actually building the images. 

The resulting container image can be run as described in the [main readme](../README.md). The following minimal example uses an in-memory data store and does not mount any volumes:

```shell
docker run -p 80:8080 -e Turnierplan__ApplicationUrl="http://localhost" -e Database__InMemory="true" turnierplan:dev
```
