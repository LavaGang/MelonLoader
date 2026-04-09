#!/bin/bash
#
# MelonLoader launch wrapper for macOS Steam games.
#
# Why this exists:
#   When Steam on macOS launches a .app bundle, macOS LaunchServices creates
#   a fresh process that does not inherit the DYLD_INSERT_LIBRARIES env var
#   needed to inject MelonLoader. This wrapper sidesteps LaunchServices by
#   execing the inner binary directly with the env vars set, so the game
#   process inherits DYLD_INSERT_LIBRARIES the normal Unix way.
#
# Installation:
#   Install MelonLoader into the game folder (either via MelonLoader.Installer
#   or by extracting the macOS build zip). The resulting layout should be:
#
#     <steamapps>/common/<GameName>/
#     ├── <GameName>.app/
#     ├── MelonLoader.Bootstrap.dylib
#     ├── melonloader-launch.sh
#     └── MelonLoader/
#
#   Then set the game's Steam Launch Options to:
#     "/full/absolute/path/to/melonloader-launch.sh" %command%
#
#   Note: the path must be absolute. Steam on macOS does not resolve
#   relative paths in Launch Options relative to the game's common folder,
#   so "./melonloader-launch.sh" will fail with a generic launch error.
#
# The script auto-detects the .app bundle sitting next to it and resolves
# the game binary via Info.plist's CFBundleExecutable, so no per-game edits
# are required.

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BOOTSTRAP_PATH="$SCRIPT_DIR/MelonLoader.Bootstrap.dylib"

# Find the one .app bundle next to this script.
shopt -s nullglob
APPS=("$SCRIPT_DIR"/*.app)
shopt -u nullglob
if [ ${#APPS[@]} -ne 1 ]; then
    echo "melonloader-launch: expected exactly one .app bundle in $SCRIPT_DIR, found ${#APPS[@]}; launching without MelonLoader" >&2
    exec "$@"
fi
APP="${APPS[0]}"

# Resolve the game binary via CFBundleExecutable.
BINARY_NAME=$(/usr/libexec/PlistBuddy -c "Print CFBundleExecutable" "$APP/Contents/Info.plist" 2>/dev/null || true)
BINARY="$APP/Contents/MacOS/$BINARY_NAME"

if [ -z "$BINARY_NAME" ] || [ ! -x "$BINARY" ] || [ ! -f "$BOOTSTRAP_PATH" ]; then
    echo "melonloader-launch: missing binary ($BINARY) or bootstrap dylib ($BOOTSTRAP_PATH); launching without MelonLoader" >&2
    exec "$@"
fi

# If Steam handed us the .app bundle path (absolute or relative), redirect
# to the inner binary. Compare via realpath-style resolution so "./Foo.app"
# and "/abs/path/to/Foo.app" both match.
if [ -d "${1:-}" ] && [ "${1%.app}" != "$1" ]; then
    FIRST_ARG_ABS="$(cd "$1" && pwd)"
    if [ "$FIRST_ARG_ABS" = "$APP" ]; then
        shift
        set -- "$BINARY" "$@"
    fi
fi

# Both the dylib and the managed MelonLoader/ folder live in SCRIPT_DIR
# alongside the .app; point DYLD_LIBRARY_PATH there so the managed side's
# bare-filename NativeLibrary.Load("MelonLoader.Bootstrap.dylib") resolves.
export DYLD_LIBRARY_PATH="$SCRIPT_DIR"

# Prepend the bootstrap to Steam's own injected dylibs (overlay, steamloader)
# rather than replacing them, so the Steam overlay keeps working.
if [ -n "$STEAM_DYLD_INSERT_LIBRARIES" ]; then
    export DYLD_INSERT_LIBRARIES="$BOOTSTRAP_PATH:$STEAM_DYLD_INSERT_LIBRARIES"
else
    export DYLD_INSERT_LIBRARIES="$BOOTSTRAP_PATH"
fi

exec "$@"
