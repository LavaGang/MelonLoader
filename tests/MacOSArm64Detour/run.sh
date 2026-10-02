#!/usr/bin/env bash
set -euo pipefail
test_dir="$(cd "$(dirname "$0")" && pwd)"
repository="$(cd "$test_dir/../.." && pwd)"
dotnet_command="${DOTNET:-dotnet}"
python3 "$test_dir/test_jit_copy.py" "$repository"
"$dotnet_command" restore "$test_dir/ManagedDetourSizes.csproj" --ignore-failed-sources
"$dotnet_command" run --project "$test_dir/ManagedDetourSizes.csproj" --no-restore -- "$repository/MelonLoader/Core.cs"
