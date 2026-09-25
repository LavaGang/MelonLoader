# macOS native host regression

Run `bash tests/MacHost/run.sh` from the repository root with the .NET 10 SDK
and Xcode command-line tools installed. On Apple Silicon, the default x64 run
requires Rosetta; `bash tests/MacHost/run.sh arm64` tests the same platform helpers
natively without claiming support for a complete arm64 loader build.

The C executable loads a NativeAOT shared library containing the production
argument parser and environment class. It checks both basedir syntaxes, empty
and Unicode values, and discovery of a Unity-shaped app bundle with the loader
outside that bundle. A normal managed executable cannot reproduce the missing
host argv in a NativeAOT library. No game files are needed.
