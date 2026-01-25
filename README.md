# turnierplan.NET

*An open-source tournament planning application for football clubs*

[![.github/workflows/validate.yaml](https://github.com/turnierplan-NET/turnierplan.NET/actions/workflows/validate.yaml/badge.svg)](https://github.com/turnierplan-NET/turnierplan.NET/actions/workflows/validate.yaml) [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=turnierplan-NET_turnierplan.NET&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=turnierplan-NET_turnierplan.NET) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=turnierplan-NET_turnierplan.NET&metric=bugs)](https://sonarcloud.io/summary/new_code?id=turnierplan-NET_turnierplan.NET) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=turnierplan-NET_turnierplan.NET&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=turnierplan-NET_turnierplan.NET) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=turnierplan-NET_turnierplan.NET&metric=coverage)](https://sonarcloud.io/summary/new_code?id=turnierplan-NET_turnierplan.NET) [![NuGet Version](https://img.shields.io/nuget/v/Turnierplan.Adapter)](https://www.nuget.org/packages/Turnierplan.Adapter)

## Introduction

**turnierplan.NET** is mostly written in C# using [.NET](https://dotnet.microsoft.com/). This includes the core logic, the backend API and database connection as well as all publicly visible web pages. In addition, it serves the *turnierplan.NET portal*, the client application for authenticated users, based on the [Angular](https://angular.dev/) framework. Some screenshots can be seen in the [section at the end](#screenshots).

> [!NOTE]  
> The user interface is currently only available in German 🇩🇪

## Installation

If you want to install **turnierplan.NET** on your server, please visit the [Installation guide](https://docs.turnierplan.net/installation).

## Documentation

Visit the **turnierplan.NET** documentation using the following link: [docs.turnierplan.net](https://docs.turnierplan.net)

The documentation sources are located in the `docs` directory. See the [docs readme](docs/README.md) for further information on how to edit and build the documentation.

## Development

This section describes how to set up the development environment. First, you need to install the following tools:

- .NET 10.0 SDK
- node.js v24.x and npm
- your favourite IDE

To run the application from source, follow these steps:

1. Open the `src/Turnierplan.slnx` solution and navigate to the docker compose file located under `Solution Items`. Run the `turnierplan.database` docker compose service. This will start up the PostgreSQL database for local development.
2. Navigate to the `Turnierplan.App` project and run the `Turnierplan.App` launch configuration. This will start the backend using port `45000`.
3. Open a terminal and navigate to the `src/Turnierplan.App/Client` directory. Run `npm install` to install the node dependencies. Next, you can start the client application by typing `npm run start`.
4. Access the client application using [http://localhost:45001](http://localhost:45001) and log in using default credentials. The user name is `admin` and the password is `P@ssw0rd`.

When running locally, the API documentation can be viewed by opening [http://localhost:45000/scalar](http://localhost:45000/scalar).

> [!NOTE]  
> The solution must be built first before the client application can be started. This is because the client application startup depends on OpenAPI files generated during the solution build.
