# MelonLoader on macOS Apple Silicon (arm64)

> **Disclaimer:** this Apple Silicon port was vibe coded and done for fun. It isn't an official MelonLoader release. Expect rough edges, and back up your game and saves before using it.
>
> **Tested hardware:** verified working on a MacBook Pro with an M4 Pro chip and 24 GB of RAM, running BTD6 with the BTD6 Mod Helper, Faster Forward and Time Machine mods. Other Macs, games and mods haven't been tested.

This branch runs MelonLoader **natively on Apple Silicon** for Il2Cpp Unity games, without Rosetta for the game itself. It was developed and tested against **Bloons TD 6** (Steam).

> **Status: experimental.** The loader, mod lifecycle, Harmony patching and class injection work, and BTD6 Mod Helper runs. Other mods may still hit arm64-specific problems; see [Known issues](#known-issues). Contributions are very welcome.

## What works

Tested on macOS 26.6.2 (Apple Silicon), BTD6 57.4 (Steam), Unity 6000.3.23f1, MelonLoader 0.7.4 with this branch:

| Area | Status |
| --- | --- |
| Bootstrap injection (`DYLD_INSERT_LIBRARIES` + `DYLD_INTERPOSE`) | ✅ |
| .NET 6 runtime (portable arm64 runtime downloaded automatically) | ✅ |
| Il2Cpp assembly generation (Cpp2IL + Il2CppInterop) | ✅ |
| Support module, class injection (`RegisterTypeInIl2Cpp`) | ✅ |
| ICall injection (e.g. Unity 6's renamed `Transform::SetAsLastSibling`) | ✅ |
| Mod lifecycle: `OnInitializeMelon`, `OnSceneWasLoaded`, `OnUpdate` | ✅ |
| Harmony patches on game methods (e.g. `TitleScreen.Start`, `MainMenu.Open`) | ✅ |
| Il2Cpp structs passed by value to patched methods (e.g. `StreamingContext`) | ✅ with the patched Il2CppInterop (see below) |
| BTD6 Mod Helper 3.6.10 (after an AnyCPU patch) | ✅ profile load, main menu, Mods button, mod browser and downloads |

The release zip ships Il2CppInterop.Runtime and Il2CppInterop.HarmonySupport built from the `macos-arm64-abi-fixes` branch of [BepInEx/Il2CppInterop](https://github.com/BepInEx/Il2CppInterop), based on `f03c8f4` (the commit behind MelonLoader's pinned `1.5.1-ci.845`). A source build of this branch still restores the stock package, so copy those two DLLs into `MelonLoader/net6/` yourself.

## Requirements

- An Apple Silicon Mac.
- **Rosetta 2**: Cpp2IL is only published for x86_64 and runs under Rosetta during first-time assembly generation.
  ```bash
  softwareupdate --install-rosetta --agree-to-license
  ```
- An internet connection on first launch. MelonLoader downloads the .NET 6 arm64 runtime (about 28 MB), Cpp2IL and Unity dependency assemblies.
- To build from source: the .NET 10 SDK and the Xcode command-line tools.

## Building

```bash
dotnet build MelonLoader.sln -c Release -p:Platform=arm64 -p:ForceRID=osx-arm64
```

The output is in `Output/Release/osx-arm64/`: `MelonLoader.Bootstrap.dylib`, `melonloader-launch.sh` and the `MelonLoader/` folder.

## Installing (BTD6 example)

The game folder is `~/Library/Application Support/Steam/steamapps/common/BloonsTD6/`. Close the game first.

1. **Copy the loader** next to `BloonsTD6.app`, not inside it:
   ```
   BloonsTD6/
   ├── BloonsTD6.app/
   ├── MelonLoader.Bootstrap.dylib
   ├── melonloader-launch.sh
   └── MelonLoader/
   ```
   `melonloader-launch.sh` expects exactly one `.app` in that folder, so keep any backup elsewhere (see the next step).

2. **Back up the game bundle.** The next two steps modify it.
   ```bash
   cd ~/Library/Application\ Support/Steam/steamapps/common/BloonsTD6
   ditto BloonsTD6.app ~/BloonsTD6.app.orig
   ```

3. **Thin `GameAssembly.dylib` to arm64.** Cpp2IL's Mach-O reader currently fails on universal binaries (`Stream does not support writing` in `MachOFile.ApplyChainedFixups`).
   ```bash
   cd BloonsTD6.app/Contents/Frameworks
   lipo -thin arm64 GameAssembly.dylib -output GameAssembly.dylib.arm64
   mv GameAssembly.dylib.arm64 GameAssembly.dylib
   cd -
   ```

4. **Ad-hoc re-sign the app.** The shipped signature uses the Hardened Runtime without `allow-jit`, so the .NET runtime is killed (`SIGTRAP` in `pthread_jit_write_protect_np`) as soon as it starts. Signing ad-hoc without `--options runtime` removes the Hardened Runtime.
   ```bash
   codesign --force --deep --sign - BloonsTD6.app
   ```
   This replaces the developer's signature and notarization and drops its entitlements (for BTD6: iCloud, push notifications and associated domains), so features that depend on them may stop working in the modified copy.

5. **Launch through the wrapper.** In Steam, set the game's launch options to the absolute, quoted path:
   ```
   "/Users/<you>/Library/Application Support/Steam/steamapps/common/BloonsTD6/melonloader-launch.sh" %command%
   ```
   The wrapper runs the game binary directly so that macOS doesn't strip `DYLD_INSERT_LIBRARIES`, and opens a Terminal window that follows `MelonLoader/Latest.log` (set `MELONLOADER_NO_TERMINAL=1` to disable it).

The first launch takes about a minute longer while assemblies are generated. Later launches reuse them.

**A Steam update or "Verify integrity of game files" restores the original signed app, which undoes steps 3 and 4.** Repeat them afterwards. To uninstall, delete the loader files, then restore your backup or verify the game files in Steam.

## x64-only mods (e.g. BTD6 Mod Helper)

CoreCLR on arm64 refuses to load assemblies built with `PlatformTarget=x64` and throws `FileLoadException` (HRESULT `0x80131621`), even when they contain only IL. You can check a mod with `file`:

```
PE32+ executable (DLL) ... x86-64 Mono/.Net assembly   ← x64-only, won't load
PE32 executable (DLL) ... Intel 80386 Mono/.Net assembly ← AnyCPU, loads
```

**Mod authors:** build with `<PlatformTarget>AnyCPU</PlatformTarget>`, the default, so your mod works on Apple Silicon.

**Users:** an IL-only mod can be rewritten as AnyCPU with [Mono.Cecil](https://www.nuget.org/packages/Mono.Cecil) in a small .NET 6 console app:

```csharp
using Mono.Cecil;

// args: <input.dll> <output.dll> <reference dirs...>
var resolver = new DefaultAssemblyResolver();
foreach (var dir in args.Skip(2))
    resolver.AddSearchDirectory(dir);

var module = ModuleDefinition.ReadModule(args[0], new ReaderParameters { AssemblyResolver = resolver });
if (!module.Attributes.HasFlag(ModuleAttributes.ILOnly))
    throw new Exception("Not IL-only; this assembly really is architecture-specific");

module.Architecture = TargetArchitecture.I386;
module.Attributes &= ~ModuleAttributes.Required32Bit;
module.Attributes &= ~ModuleAttributes.Preferred32Bit;
module.Write(args[1]);
```

Pass `MelonLoader/Il2CppAssemblies`, `MelonLoader/net6` and the .NET 6 framework folder (`MelonLoader/Dependencies/dotnet/shared/Microsoft.NETCore.App/<version>`) as reference directories. Cecil needs them to write enum-typed default parameter values. Keep the original file: the patched copy is a rewritten assembly, not a byte-for-byte edit.

BTD6 Mod Helper also downloads `Plugins/UpdaterPlugin.dll` on every launch, and that may replace your patched copy with the x64 original. Remove the updater plugin if you've patched Mod Helper. Mods downloaded through Mod Helper's mod browser are often x64-only too (for example FasterForward and TimeMachine), so check and patch them before the next launch.

## Known issues

These are open problems. The causes marked *suspected* haven't been confirmed, and investigation or PRs are welcome.

1. **Tiered compilation is disabled.** Without that, `InjectorHelpers.Setup` ran its original, unpatched code after many `ClassInjector.RegisterTypeInIl2Cpp` calls, and its x86-only scanner threw `Sequence contains no elements`. Turning tiering off made the errors disappear, which fits tier-1 recompilation replacing Harmony's detour, but the exact mechanism hasn't been confirmed. Startup JIT is slower as a result. A narrower fix would keep the detour across recompilation, or resolve `GetTypeInfoFromTypeDefinitionIndex` by symbol so the original `Setup` is harmless.

2. **Game methods smaller than 16 bytes can't be patched from game code.** Dobby's arm64 hook writes 16 bytes, so on a smaller function it would overwrite the start of the next one. BTD6 Mod Helper's patch on the 12-byte `Btd6Player.Save` broke `Btd6Player.Sync` this way, and the game died with `SIGILL` at the first round end. MelonLoader now refuses such hooks and logs `Refusing to patch … because it is too small for an ARM64 detour`, so game code calls the unpatched method, while calls from mods still go through the patch. Dobby's near-branch mode (a 4-byte hook) isn't a fix on macOS: it places its trampolines in zero-filled memory inside existing mappings, which corrupted IL2CPP's GC heap in testing. A real fix needs a trampoline allocator that only uses unmapped memory within ±128 MB of GameAssembly.

3. **Structs larger than 16 bytes returned from patched methods** use a fixed-size struct type created at run time, which Harmony's generated delegate assembly can't reference, so patching such a method fails with `FileLoadException`. Passing structs of any size, and returning ones of up to 16 bytes, works.

4. **No xref cache in generated assemblies on arm64.** Il2CppInterop's xref scanner only decodes x86 code, so the generator runs with `NoXrefCache = true`. Mods that use `XrefScanner` at runtime won't work, and names generated for obfuscated methods may differ from Windows builds.

5. **The ClrMD delegate sanity check is skipped on macOS.** `DataTarget.CreateSnapshotAndAttach` isn't supported on Darwin, so a mod that passes a raw managed method pointer to `NativeHook` isn't caught and wrapped on macOS.

6. **The game version shows as `UNKNOWN`** because AssetsTools hits an `EndOfStreamException` while reading it. This is harmless.

7. **Crashes when the game is killed with `SIGTERM`** (Discord SDK abort, Metal render thread, Il2Cpp thread pool). Seen only when a test harness kills the process, with no MelonLoader frames on the crashing thread.

## How it works (changes in this branch)

- **Bootstrap:** the arm64 portable .NET 6 runtime download is enabled. `DYLD_INSERT_LIBRARIES` and `DYLD_LIBRARY_PATH` are cleaned by file name, and unset when empty, so child processes such as the x86_64 Cpp2IL don't inherit the arm64-only dylib.
- **MonoMod:** `MacOSArm64NativeDetourPlatform` is used under .NET 6 as well as Mono. MonoMod's own detection left `ILHook` throwing `NullReferenceException`.
- **Il2CppAssemblyGenerator:** `NoXrefCache = true` on arm64, which skips the x86-only metadata-init scan in Pass 16 while keeping its GC write-barrier check.
- **Mach-O symbol resolver** (`NativeUtils/MachOParser`):
  - It reads `LC_SYMTAB` from `GameAssembly.dylib`, including local symbols.
  - It computes the ASLR slide from two exports and checks that they agree.
  - It resolves the class-injection hook targets by mangled name (`GenericMethod::GetMethod`, `Class::GetFieldDefaultValue`, `Class::FromIl2CppType`, `Class::FromName`, `Class::Init`).
  - **Branch-thunk hardening:** many il2cpp exports and wrappers are a single `b` instruction, and Dobby's 16-byte arm64 patch would overwrite the functions after them. Targets follow up to 3 unconditional branches and must have at least 16 bytes before the next symbol, or they're refused.
  - **Short-function guard:** every native hook, including mods' Harmony patches, measures its GameAssembly target first and refuses functions smaller than 16 bytes. The measurement covers all of `__TEXT`'s code sections, since Unity 6 puts IL2CPP's generated game code in its own `__TEXT,il2cpp` section.
- **ICall injection:** the hook attaches without the ClrMD check on macOS, hooks `InternalCalls::Resolve` rather than the 4-byte `il2cpp_resolve_icall` stub, and keeps its detour delegate alive.
- **Il2CppInterop struct ABI** (separate Il2CppInterop branch): AAPCS64 passes structs of up to 16 bytes in registers and returns large ones through a buffer in `x8`. The native-to-managed trampolines used the Windows x64 rules (a pointer for every struct, and the return buffer as the first parameter), so a patched `[OnDeserialized]` callback boxed its `StreamingContext` from a null pointer and crashed in `Object::Box`. The trampolines now receive and return these structs by value, using fixed-size struct types compiled into Il2CppInterop.Runtime.
- **Tiered compilation** is off on osx-arm64 (`System.Runtime.TieredCompilation=false` in the runtimeconfig, and `DOTNET_TieredCompilation=0` in `melonloader-launch.sh`), so Harmony detours stay in place.
