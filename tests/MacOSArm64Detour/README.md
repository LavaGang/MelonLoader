# macOS ARM64 detour regression

Run on Apple Silicon with the Xcode command-line tools and .NET 10 SDK:

```sh
DOTNET=/absolute/path/to/dotnet bash tests/MacOSArm64Detour/run.sh
```

The native fixture compiles the production `MLMacOSJitCopy` function and checks
invalid ranges, overlap, RX and mixed-protection destinations, exact protection
restoration, `MAP_JIT`, preservation of a caller-owned JIT write scope, and
concurrent helper writers. The managed fixture compares the wrapper's copy
lengths with MonoMod's actual ARM writer and verifies the 16-byte ARM64 absolute
jump sequence. It uses no game files and changes no system security settings.

The native mutex serializes calls through `MLMacOSJitCopy`; it does not suspend
arbitrary threads executing the destination or serialize unrelated patching
libraries. Callers still need a lifecycle point where the target is not running.
