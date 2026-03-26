# Local Startup Guide

This guide shows both ways to start locally:

- with the script (`scripts/start-local-mac.sh`)
- manually (without the script)

All commands below are relative to the project root directory (`advanced-software-engineering`).

## Prerequisites

- Docker + Docker Compose
- .NET SDK
- 3 terminal windows (1x server, 2x clients)

## Option A: Start with script

### macOS / Linux

```bash
./scripts/start-local-mac.sh
```

Optional check (no windows opened):

```bash
./scripts/start-local-mac.sh --dry-run
```

### Windows

`scripts/start-local-mac.sh` is a Bash script. Use one of these:

- Git Bash
- WSL

Example (Git Bash / WSL):

```bash
./scripts/start-local-mac.sh
```

If you use plain PowerShell/CMD, use Option B (manual).

## Option B: Start manually

### 1) Start server (Docker Compose)

#### macOS / Linux (bash or zsh)

```bash
FILE_LOG=false CONSOLE_LOG=true docker compose -f compose.yaml up --build -d
```

#### Windows PowerShell

```powershell
$env:FILE_LOG="false"
$env:CONSOLE_LOG="true"
docker compose -f compose.yaml up --build -d
```

### 2) Start client #1

#### macOS / Linux (bash or zsh)

```bash
FILE_LOG=false CONSOLE_LOG=false dotnet run --project chess-client/chess-client.csproj
```

#### Windows PowerShell

```powershell
$env:FILE_LOG="false"
$env:CONSOLE_LOG="false"
dotnet run --project chess-client/chess-client.csproj
```

### 3) Start client #2

Run the same client command from step 2 in a second terminal.

## Useful commands

### Check containers

```bash
docker compose -f compose.yaml ps
```

### Stop server containers

```bash
docker compose -f compose.yaml down
```




