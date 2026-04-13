#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
COMPOSE_FILE="$REPO_ROOT/compose.yaml"
CLIENT_PROJECT="$REPO_ROOT/chess-client/chess-client.csproj"
DRY_RUN="${1:-}"
SERVER_LOG_ENV="FILE_LOG=false CONSOLE_LOG=true"
CLIENT_LOG_ENV="FILE_LOG=false CONSOLE_LOG=false"
OS_NAME="$(uname -s)"

if [[ ! -f "$COMPOSE_FILE" ]]; then
  echo "Error: compose file not found: $COMPOSE_FILE" >&2
  exit 1
fi

if [[ ! -f "$CLIENT_PROJECT" ]]; then
  echo "Error: chess-client project not found: $CLIENT_PROJECT" >&2
  exit 1
fi

launch_terminal_window() {
  local command="$1"

  if [[ "$DRY_RUN" == "--dry-run" ]]; then
    echo "[dry-run] Open terminal window: $command"
    return
  fi

  case "$OS_NAME" in
    Darwin)
      launch_terminal_macos "$command"
      ;;
    Linux)
      launch_terminal_linux "$command"
      ;;
    *)
      echo "Error: unsupported operating system: $OS_NAME" >&2
      exit 1
      ;;
  esac
}

launch_terminal_macos() {
  local command="$1"

  osascript - "$REPO_ROOT" "$command" <<'APPLESCRIPT'
on run argv
  set repoRoot to item 1 of argv
  set terminalCommand to item 2 of argv
  tell application "Terminal"
    activate
    do script "cd " & quoted form of repoRoot & " && " & terminalCommand
  end tell
end run
APPLESCRIPT
}

launch_terminal_linux() {
  local command="$1"
  local full_command

  if [[ -z "${DISPLAY:-}" && -z "${WAYLAND_DISPLAY:-}" ]]; then
    echo "Error: no GUI display found (DISPLAY/WAYLAND_DISPLAY)." >&2
    echo "Please run this script in a graphical Linux session." >&2
    exit 1
  fi

  printf -v full_command 'cd %q && %s; exec bash' "$REPO_ROOT" "$command"

  if command -v x-terminal-emulator >/dev/null 2>&1; then
    x-terminal-emulator -e bash -lc "$full_command"
  elif command -v gnome-terminal >/dev/null 2>&1; then
    gnome-terminal -- bash -lc "$full_command"
  elif command -v konsole >/dev/null 2>&1; then
    konsole -e bash -lc "$full_command"
  elif command -v xfce4-terminal >/dev/null 2>&1; then
    xfce4-terminal --command="bash -lc \"$full_command\""
  elif command -v xterm >/dev/null 2>&1; then
    xterm -e bash -lc "$full_command"
  else
    echo "Error: no supported Linux terminal found." >&2
    echo "Install one of: x-terminal-emulator, gnome-terminal, konsole, xfce4-terminal, or xterm." >&2
    exit 1
  fi
}

echo "Starting Docker Compose services..."

launch_terminal_window "$SERVER_LOG_ENV docker compose -f '$COMPOSE_FILE' up --build -d"

echo "Starting two chess-client instances in separate terminal windows..."
launch_terminal_window "$CLIENT_LOG_ENV dotnet run --project chess-client/chess-client.csproj"
launch_terminal_window "$CLIENT_LOG_ENV dotnet run --project chess-client/chess-client.csproj"

echo "Done. Started one Docker terminal and two client terminals."


