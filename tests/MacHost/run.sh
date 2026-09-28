#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")"
test_root="$(pwd -P)"
test_arch="${1:-x86_64}"
case "$test_arch" in
	x86_64) test_rid=osx-x64 ;;
	arm64) test_rid=osx-arm64 ;;
	*) echo 'Use x86_64 or arm64' >&2; exit 1 ;;
esac
output="$test_root/bin/native-$test_arch"
dotnet publish MacHost.csproj -c Release -r "$test_rid" -p:RuntimeIdentifiers="$test_rid" -o "$output"
app="$output/音楽 Test Game.app"
basedir="$output/Separate café Loader"
mkdir -p "$app/Contents/MacOS" "$app/Contents/Resources/Data" "$basedir"
xcrun clang -arch "$test_arch" host.c -o "$app/Contents/MacOS/host"
for style in separate equals; do
	if [ "$style" = separate ]; then
		set -- --melonloader.basedir "$basedir"
	else
		set -- "--melonloader.basedir=$basedir"
	fi
	"$app/Contents/MacOS/host" "$output/MacHost.dylib" "$app" "$basedir" \
		"$@" --test.empty= --test.equals=a=b --test.unicode '音楽 café' --test.flag
done
