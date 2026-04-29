# Quickstart Guide

## Requirements

- .NET 9.0 SDK
- Docker and Docker Compose

## Running Tests

```bash
dotnet test
```

## Local Startup Guide

This guide shows both ways to start locally:

- with the script (`scripts/start-local.sh`)
- manually (without the script)

All commands below are relative to the project root directory (`advanced-software-engineering`).

> **Important:** Docker must be running before you start the project locally.


### Option A: Start with script

#### Prerequisites
- Docker + Docker Compose
- .NET SDK

#### macOS / Linux

```bash
./scripts/start-local.sh
```

### Option B: Start manually

#### Prerequisites
- Docker + Docker Compose
- .NET SDK
- 3 terminal windows (1x server, 2x clients)

#### 1) Start server (Docker Compose)

##### macOS / Linux (bash or zsh)

```bash
docker compose -f compose.yaml up --build -d
```

#### 2) Start client #1

##### macOS / Linux (bash or zsh)

```bash
dotnet run --project chess-client/chess-client.csproj
```

#### 3) Start client #2

Run the same client command from step 2 in a second terminal.

## Useful Commands

### Check containers

```bash
docker compose -f compose.yaml ps
```

### Stop server containers

```bash
docker compose -f compose.yaml down
```
