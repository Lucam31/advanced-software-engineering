# Quickstart

## Requirements

- Docker and Docker Compose
- .NET 9.0 SDK for local development

> Make sure Docker is running before starting the project.

## Start from a GitHub Release

1. Download and unzip the release archive.
2. Open the extracted folder.
3. Run:

```bash
chmod +x ./start-local.sh
./start-local.sh
```

## Start from the Repository

Use this when developing locally. It builds the server from the source code.

### One-command start

```bash
./scripts/start-local.sh
```

### Manual start

```bash
docker compose -f compose.dev.yaml up --build -d
dotnet run --project chess-client/chess-client.csproj
```

Run the client command twice to open two client instances.
