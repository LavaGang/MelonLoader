#!/usr/bin/env bash
set -euo pipefail
if [[ "$(uname -s)" != Darwin ]]; then
    echo "This regression requires macOS, .NET 9 and Xcode command-line tools." >&2
    exit 1
fi
test_dir="$(cd "$(dirname "$0")" && pwd)"
scratch="$(mktemp -d "${TMPDIR:-/tmp/}melonloader-env.XXXXXX")"
trap 'rm -rf "$scratch"' EXIT
dotnet_command="${DOTNET:-dotnet}"
mkdir -p "$scratch/fixture with spaces"
fixture_dir="$scratch/fixture with spaces"
clang -Wall -Wextra -Werror "$test_dir/fixture.c" -o "$fixture_dir/child"
clang -Wall -Wextra -Werror -dynamiclib "$test_dir/injected.c" -o "$fixture_dir/first.dylib"
clang -Wall -Wextra -Werror -dynamiclib "$test_dir/injected.c" -o "$fixture_dir/second.dylib"
for mode in true false; do
    echo "Testing OSX=$mode"
    "$dotnet_command" build "$test_dir/ExecutablePackage.csproj" --nologo \
        -p:TestOSX="$mode" -p:BaseIntermediateOutputPath="$scratch/obj-$mode/" \
        -o "$scratch/bin-$mode"
    "$dotnet_command" "$scratch/bin-$mode/ExecutablePackage.dll" \
        "$fixture_dir/child" "$fixture_dir/first.dylib" "$fixture_dir/second.dylib"
done
