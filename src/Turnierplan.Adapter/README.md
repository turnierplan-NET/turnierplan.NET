# turnierplan.NET - client library

*.NET client library for the [turnierplan.NET](https://github.com/turnierplan-NET/turnierplan.NET) API*

## Introduction

This NuGet package can be used to query the API endpoints provided by the [turnierplan.NET](https://github.com/turnierplan-NET/turnierplan.NET) application.

## Usage

Instantiate the `TurnierplanClientOptions` and provide the base url as well as the credentials:

```cs
var config = new TurnierplanClientOptions("http://localhost:45000", "<ApiKey>", "<ApiKeySecret>");

using var client = new TurnierplanClient(config);

var x = await client.GetTournaments("<FolderId>");
var y = await client.GetTournament("<TournamentId>");
```

## Supported Endpoints

Currently, the `Turnierplan.Adapter` packages supports the following endpoints:

- `GET /api/tournaments/{id}` - get a single tournament including all details
- `GET /api/tournaments?folderId={id}` - gets all tournaments in a folder
