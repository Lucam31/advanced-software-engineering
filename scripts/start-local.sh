#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
OS_NAME="$(uname -s)"

WORK_ROOT=""
COMPOSE_FILE=""
CLIENT_COMMAND=""

if [[ -f "$REPO_ROOT/compose.dev.yaml" && -f "$REPO_ROOT/chess-client/chess-client.csproj" ]]; then
  WORK_ROOT="$REPO_ROOT"
  COMPOSE_FILE="$WORK_ROOT/compose.dev.yaml"
  CLIENT_COMMAND="dotnet run --project chess-client/chess-client.csproj"
elif [[ -f "$SCRIPT_DIR/compose.release.yaml" ]]; then
  WORK_ROOT="$SCRIPT_DIR"
  COMPOSE_FILE="$WORK_ROOT/compose.release.yaml"

  if [[ -f "$WORK_ROOT/chess-client" ]]; then
    chmod +x "$WORK_ROOT/chess-client" >/dev/null 2>&1 || true
    CLIENT_COMMAND="./chess-client"
  else
    echo "Error: client binary not found in release folder: $WORK_ROOT/chess-client" >&2
    exit 1
  fi
else
  echo "Error: could not detect project layout." >&2
  echo "Expected repo files ($REPO_ROOT/compose.dev.yaml) or release files ($SCRIPT_DIR/compose.release.yaml)." >&2
  exit 1
fi

if [[ ! -f "$COMPOSE_FILE" ]]; then
  echo "Error: compose file not found: $COMPOSE_FILE" >&2
  exit 1
fi

launch_terminal_window() {
  local command="$1"

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

# macOS prefers iTerm2; fall back to Terminal.app when iTerm2 is unavailable.
launch_terminal_macos() {
  local command="$1"

  if osascript -e 'id of application "iTerm"' >/dev/null 2>&1; then
    osascript - "$WORK_ROOT" "$command" <<'APPLESCRIPT'
on run argv
  set repoRoot to item 1 of argv
  set terminalCommand to item 2 of argv
  tell application "iTerm"
    activate
    if (count of windows) is 0 then
      create window with default profile
    else
      tell current window
        create tab with default profile
      end tell
    end if
    tell current session of current tab of current window
      write text "cd " & quoted form of repoRoot & " && " & terminalCommand
    end tell
  end tell
end run
APPLESCRIPT
    return
  fi

  osascript - "$WORK_ROOT" "$command" <<'APPLESCRIPT'
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

  printf -v full_command 'cd %q && %s; exec bash' "$WORK_ROOT" "$command"
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

echo "Starting Docker Compose services..."

launch_terminal_window "docker compose -f '$COMPOSE_FILE' up --build -d"

echo "Starting two chess-client instances in separate terminal windows..."
launch_terminal_window "$CLIENT_COMMAND"
launch_terminal_window "$CLIENT_COMMAND"

echo "Done. Started one Docker terminal and two client terminals."


