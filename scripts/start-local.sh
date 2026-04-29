#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
COMPOSE_FILE="$REPO_ROOT/compose.yaml"
CLIENT_PROJECT="$REPO_ROOT/chess-client/chess-client.csproj"
OS_NAME="$(uname -s)"

# Ensure the expected project files are present before starting anything.
if [[ ! -f "$COMPOSE_FILE" ]]; then
  echo "Error: compose file not found: $COMPOSE_FILE" >&2
  exit 1
fi

if [[ ! -f "$CLIENT_PROJECT" ]]; then
  echo "Error: chess-client project not found: $CLIENT_PROJECT" >&2
  exit 1
fi

# Open the requested command in a new terminal window.
launch_terminal_window() {
  local command="$1"

  # Pick the correct launcher for the current operating system.
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

# macOS uses Terminal.app to open a new shell with the requested command.
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

# Linux uses the first available graphical terminal emulator.
launch_terminal_linux() {
  local command="$1"
  local full_command

  if [[ -z "${DISPLAY:-}" && -z "${WAYLAND_DISPLAY:-}" ]]; then
    echo "Error: no GUI display found (DISPLAY/WAYLAND_DISPLAY)." >&2
    echo "Please run this script in a graphical Linux session." >&2
    exit 1
  fi

  printf -v full_command 'cd %q && %s; exec bash' "$REPO_ROOT" "$command"
  if command -v kitty >/dev/null 2>&1; then
    setsid kitty --detach bash -lc "$full_command" >/dev/null 2>&1 &
  elif command -v alacritty >/dev/null 2>&1; then
    setsid alacritty -e bash -lc "$full_command" >/dev/null 2>&1 &
  elif command -v x-terminal-emulator >/dev/null 2>&1; then
    setsid x-terminal-emulator -e bash -lc "$full_command" >/dev/null 2>&1 &
  elif command -v gnome-terminal >/dev/null 2>&1; then
    setsid gnome-terminal -- bash -lc "$full_command" >/dev/null 2>&1 &
  elif command -v konsole >/dev/null 2>&1; then
    setsid konsole -e bash -lc "$full_command" >/dev/null 2>&1 &
  elif command -v xfce4-terminal >/dev/null 2>&1; then
    setsid xfce4-terminal --command="bash -lc \"$full_command\"" >/dev/null 2>&1 &
  elif command -v xterm >/dev/null 2>&1; then
    setsid xterm -e bash -lc "$full_command" >/dev/null 2>&1 &
  else
    echo "Error: no supported Linux terminal found." >&2
    echo "Install one of: kitty, alacritty, x-terminal-emulator, gnome-terminal, konsole, xfce4-terminal, or xterm." >&2
    exit 1
  fi
}

# Start the backend services first so the clients can connect afterwards.
echo "Starting Docker Compose services..."

launch_terminal_window "docker compose -f '$COMPOSE_FILE' up --build -d"

# Launch two client instances so the local multiplayer flow can be tested.
echo "Starting two chess-client instances in separate terminal windows..."
launch_terminal_window "dotnet run --project chess-client/chess-client.csproj"
launch_terminal_window "dotnet run --project chess-client/chess-client.csproj"

echo "Done. Started one Docker terminal and two client terminals."


