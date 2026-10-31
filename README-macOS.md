# MelonLoader on macOS Apple Silicon (arm64)

This branch runs MelonLoader **natively on Apple Silicon** for Il2Cpp Unity games, without Rosetta for the game itself. It was developed and tested against **Bloons TD 6** (Steam).

> **Status: experimental.** The loader, mod lifecycle, Harmony patching and class injection work. Large mods can still crash; see [Known issues](#known-issues). Contributions are very welcome.

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
| BTD6 Mod Helper 3.6.10 | ⚠️ loads after an AnyCPU patch, then crashes during profile load |

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

BTD6 Mod Helper also downloads `Plugins/UpdaterPlugin.dll` on first run, and that may replace your patched copy with the x64 original. Remove the updater plugin if you've patched Mod Helper.

## Known issues

These are open problems. The causes marked *suspected* haven't been confirmed, and investigation or PRs are welcome.

1. **BTD6 Mod Helper crashes while the player profile loads.** `SIGSEGV` in `il2cpp::vm::Object::Box` → `memmove(NULL)`, reached from `JsonContract.InvokeOnDeserialized` → `RuntimeMethodInfo.Invoke` → `Runtime::InvokeWithThrow` → three frames of JIT-generated code. *Suspected:* an Il2CppInterop-generated trampoline handles a value-type argument the x64 way. `[OnDeserialized]` callbacks take a 16-byte `StreamingContext` by value, which AAPCS64 passes in registers rather than by reference. The patched or injected method involved hasn't been identified yet.

2. **`InjectorHelpers.Setup` sometimes runs unpatched.** After many `ClassInjector.RegisterTypeInIl2Cpp` calls, stack traces show `InjectorHelpers.Setup()` without the `DMD<…>` wrapper. The original method then applies `MetadataCache_GetTypeInfoFromTypeDefinitionIndex_Hook`, which MelonLoader removes on macOS, and its x86 scanner throws `Sequence contains no elements`. This breaks `DelegateSupport.ConvertDelegate` calls (seen in Mod Helper's `LocalizationHelper.Initialize`). *Suspected:* tiered JIT recompiles the method and the Harmony detour is lost. Possible fixes: mark the hook as applied on macOS so the original `Setup` skips it, or resolve its target by symbol (`il2cpp::vm::GlobalMetadata::GetTypeInfoFromTypeDefinitionIndex(int)` exists in BTD6's `GameAssembly.dylib`).

3. **No xref cache in generated assemblies on arm64.** Il2CppInterop's xref scanner only decodes x86 code, so the generator runs with `NoXrefCache = true`. Mods that use `XrefScanner` at runtime won't work, and names generated for obfuscated methods may differ from Windows builds.

4. **The ClrMD delegate sanity check is skipped on macOS.** `DataTarget.CreateSnapshotAndAttach` isn't supported on Darwin, so a mod that passes a raw managed method pointer to `NativeHook` isn't caught and wrapped on macOS.

5. **The game version shows as `UNKNOWN`** because AssetsTools hits an `EndOfStreamException` while reading it. This is harmless.

6. **Crashes when the game is killed with `SIGTERM`** (Discord SDK abort, Metal render thread, Il2Cpp thread pool). Seen only when a test harness kills the process, with no MelonLoader frames on the crashing thread.

## How it works (changes in this branch)

- **Bootstrap:** the arm64 portable .NET 6 runtime download is enabled. `DYLD_INSERT_LIBRARIES` and `DYLD_LIBRARY_PATH` are cleaned by file name, and unset when empty, so child processes such as the x86_64 Cpp2IL don't inherit the arm64-only dylib.
- **MonoMod:** `MacOSArm64NativeDetourPlatform` is used under .NET 6 as well as Mono. MonoMod's own detection left `ILHook` throwing `NullReferenceException`.
- **Il2CppAssemblyGenerator:** `NoXrefCache = true` on arm64, which skips the x86-only metadata-init scan in Pass 16 while keeping its GC write-barrier check.
- **Mach-O symbol resolver** (`NativeUtils/MachOParser`):
  - It reads `LC_SYMTAB` from `GameAssembly.dylib`, including local symbols.
  - It computes the ASLR slide from two exports and checks that they agree.
  - It resolves the class-injection hook targets by mangled name (`GenericMethod::GetMethod`, `Class::GetFieldDefaultValue`, `Class::FromIl2CppType`, `Class::FromName`, `Class::Init`).
  - **Branch-thunk hardening:** many il2cpp exports and wrappers are a single `b` instruction, and Dobby's 16-byte arm64 patch would overwrite the functions after them. Targets follow up to 3 unconditional branches and must have at least 16 bytes before the next symbol, or they're refused.
- **ICall injection:** the hook attaches without the ClrMD check on macOS, hooks `InternalCalls::Resolve` rather than the 4-byte `il2cpp_resolve_icall` stub, and keeps its detour delegate alive.
