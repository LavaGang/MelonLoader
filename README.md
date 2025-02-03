
<p align="center">
  <a href="#"><img src="https://raw.githubusercontent.com/LavaGang/MelonLoader.Installer/master/Resources/ML_Icon.png"></a>
  <a href="#"><img src="https://raw.githubusercontent.com/LavaGang/MelonLoader.Installer/master/Resources/ML_Text.png"></a>
</p>

---

<p align="center">
	<a href="https://github.com/LavaGang/MelonLoader/releases/latest"><img src="https://img.shields.io/github/v/release/LavaGang/MelonLoader?label=latest&style=for-the-badge"></a>
	<a href="https://github.com/LavaGang/MelonLoader/releases"><img src="https://img.shields.io/github/downloads/LavaGang/MelonLoader/total.svg?style=for-the-badge"></a>
	<a href="https://github.com/LavaGang/MelonLoader/graphs/contributors"><img src="https://img.shields.io/github/contributors/LavaGang/MelonLoader?style=for-the-badge"></a>
	<a href="https://discord.gg/2Wn3N2P"><img src="https://img.shields.io/discord/663449315876012052?label=discord&style=for-the-badge&color=blueviolet"></a>
</p>

<p align="center">
	<a href="https://github.com/LavaGang/MelonLoader.Installer/releases/latest/download/MelonLoader.Installer.exe"><img src="https://img.shields.io/github/downloads/LavaGang/MelonLoader.Installer/latest/MelonLoader.Installer.exe?style=for-the-badge&label=Windows%20Installer"></a>
</p>

<p align="center">
	<a href="https://github.com/LavaGang/MelonLoader.Installer/releases/latest/download/MelonLoader.Installer.Linux"><img src="https://img.shields.io/github/downloads/LavaGang/MelonLoader.Installer/latest/MelonLoader.Installer.Linux?style=for-the-badge&label=Linux%20Installer"></a>
</p>

---

## GENERAL INFORMATION:

- Debug Mode is for Development Purposes.  
Use it to help Develop and Debug MelonLoader, Plugins, and Mods.
<br></br>
- All Logs are made in the created `MelonLoader/Logs` folder in your Game's Installation Folder.
- All Plugins get placed in the created `Plugins` folder in your Game's Installation Folder.
- All Mods get placed in the created `Mods` folder in your Game's Installation Folder.
<br></br>
- [The Official Wiki](https://melonwiki.xyz)
<br></br>
- [Proxies](#proxies)
- [Launch Options](#launch-options)
- [Debugging](https://melonwiki.xyz/#/modders/debugging)
<br></br>
- [Linux Support (__WINE / STEAM PROTON / NATIVE__)](https://melonwiki.xyz/#/README?id=linux-instructions)
- [Android & Oculus Quest Support (__WIP__)](https://melonwiki.xyz/#/android/general)
<br></br>

| Usage Guides: |
| - |
| [INSTALLER](#how-to-use-the-installer) |
| [MANUAL USE](#how-to-manually-use-melonloader) |

| Example Projects: |
| - |
| [TestPlugin](https://github.com/LavaGang/TestPlugin) |
| [TestMod](https://github.com/LavaGang/TestMod) |

| Nightly Builds: |
| - |
| [master](https://nightly.link/LavaGang/MelonLoader/workflows/build/master) |
| [alpha-development](https://nightly.link/LavaGang/MelonLoader/workflows/build/alpha-development) |
| [universality](https://nightly.link/LavaGang/MelonLoader/workflows/build/universality) |

---

## ❤️ SPECIAL THANKS TO OUR WONDERFUL PATRONS ❤️

- **Givo**  
- **Florian Fahrenberger**  
- **Python**  
- **SirCoolness**  
- **SlidyDev**  

---

## REQUIREMENTS:

### Il2Cpp Games:
- [.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0#runtime-6.0.15)
> On Windows, the .NET 6.0 Desktop Runtime will be installed automatically

---

## HOW TO USE THE INSTALLER:

1. Follow the Instructions in one of the Guides linked below.

| Installer Guides: |
| - |
| [INSTALL](https://github.com/LavaGang/MelonLoader.Installer/blob/master/README.md#how-to-install-re-install-or-update-melonloader) |
| [UPDATE](https://github.com/LavaGang/MelonLoader.Installer/blob/master/README.md#how-to-install-re-install-or-update-melonloader) |
| [RE-INSTALL](https://github.com/LavaGang/MelonLoader.Installer/blob/master/README.md#how-to-install-re-install-or-update-melonloader) |
| [UN-INSTALL](https://github.com/LavaGang/MelonLoader.Installer/blob/master/README.md#how-to-un-install-melonloader) |

---

## HOW TO MANUALLY USE MELONLOADER:

### UPDATE / RE-INSTALL:

1. Follow the Steps to [UN-INSTALL](#un-install)
2. Follow the Steps to [INSTALL](#install)


### INSTALL:

1. Make sure the Game is Closed and Not Running before attempting to Install.
2. Make sure you have all the [Requirements](#requirements) Installed before attempting to Install.
3. Download MelonLoader [[x86](https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x86.zip)] [[x64](https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x64.zip)]
4. Extract the MelonLoader folder from the MelonLoader Zip Archive to the Game's Installation Folder.
5. Extract version.dll & dobby.dll from the MelonLoader Zip Archive to the Game's Installation Folder.


### UN-INSTALL:

1. Make sure the Game is Closed and Not Running before attempting to UN-INSTALL.
2. Remove the version.dll file from the Game's Installation Folder.
3. Remove the MelonLoader folder from the Game's Installation Folder.

These additional steps below are OPTIONAL if you want to do a FULL UN-INSTALL.

4. Remove the Plugins folder from the Game's Installation Folder.
5. Remove the Mods folder from the Game's Installation Folder.
6. Remove the UserData folder from the Game's Installation Folder.

---

## CONFIG:

MelonLoader has its own config file at `./UserData/Loader.cfg` (you need to run MelonLoader at least once for it to appear).

Here is a list of the current config options (and their default values):
```toml
[loader]
# Disables MelonLoader. Equivalent to the '--no-mods' launch option
disable = false
# Equivalent to the '--melonloader.debug' launch option
debug_mode = true
# Only use this if the game freezes when trying to quit. Equivalent to the '--quitfix' launch option
force_quit = false
# Disables the start screen. Equivalent to the '--melonloader.disablestartscreen' launch option
disable_start_screen = false
# Starts the dotnet debugger (only for Il2Cpp games). Equivalent to the '--melonloader.launchdebugger' launch option
launch_debugger = false
# Sets the loader theme. Currently, the only available themes are "Normal" and "Lemon". Equivalent to the '--melonloader.consolemode' launch option (0 for Normal, 4 for Lemon)
theme = "Normal"

[console]
# Hides warnings from displaying. Equivalent to the '--melonloader.hidewarnings' launch option
hide_warnings = false
# Hides the console. Equivalent to the '--melonloader.hideconsole' launch option
hide_console = false
# Forces the console to always stay on-top of all other applications. Equivalent to the '--melonloader.consoleontop' launch option
console_on_top = false
# Keeps the console title as original. Equivalent to the '--melonloader.consoledst' launch option
dont_set_title = false

[logs]
# Sets the maximum amount of log files in the Logs folder (Default: 10). Equivalent to the '--melonloader.maxlogs' launch option
max_logs = 10

[unityengine]
# Overrides the detected UnityEngine version. Equivalent to the '--melonloader.unityversion' launch option
version_override = ""
# Disables the console log cleaner (only applies to Il2Cpp games). Equivalent to the '--melonloader.disableunityclc' launch option
disable_console_log_cleaner = false
# Forces the Il2Cpp Assembly Generator to run without contacting the remote API. Equivalent to the '--melonloader.agfoffline' launch option
force_offline_generation = false
# Forces the Il2Cpp Assembly Generator to use the specified regex. Equivalent to the '--melonloader.agfregex' launch option
force_generator_regex = ""
# Forces the Il2Cpp Assembly Generator to use the specified Il2Cpp dumper version. Equivalent to the '--melonloader.agfvdumper' launch option
force_il2cpp_dumper_version = ""
# Forces the Il2Cpp Assembly Generator to always regenerate assemblies. Equivalent to the '--melonloader.agfregenerate' launch option
force_regeneration = false
# Enables the CallAnalyzer processor for Cpp2IL. Equivalent to the '--cpp2il.callanalyzer' launch option
enable_cpp2il_call_analyzer = false
# Enables the NativeMethodDetector processor for Cpp2IL. Equivalent to the '--cpp2il.nativemethoddetector' launch option
enable_cpp2il_native_method_detector = false
```

---

## LAUNCH OPTIONS:

- These are additional Launch Options that MelonLoader adds to the Game.
- These can be used to manipulate how MelonLoader works.

| Argument | Description |
| - | - |
| --no-mods | Launches the Game without loading any Plugins or Mods |
| --quitfix | Fixes the Hanging Process Issue with some Games |
| --melonloader.consolemode | Changes the Theme Display Mode of the Console [ Default = 0 ] |
| --melonloader.consoleontop | Forces the Console to always stay on-top of all other Applications |
| --melonloader.consoledst | Keeps the Console Title as Original |
| --melonloader.hideconsole | Hides the Console |
| --melonloader.hidewarnings | Hides Warnings from Displaying |
| --melonloader.debug | Debug Mode |
| --melonloader.maxlogs | Max Log Files [ Default: 10 ] [ NoCap: 0 ] |
| --melonloader.loadmodeplugins | Load Mode for Plugins [ Default: 0 ] |
| --melonloader.loadmodemods | Load Mode for Mods [ Default: 0 ] |
| --melonloader.basedir | Changes the Proxy's Load Directory for the Bootstrap |
| --melonloader.disablestartscreen | Disable the Start Screen |


- These ones below are Unity Engine specific Launch Options.

| Argument | Description |
| - | - |
| --melonloader.unityversion | Allows you to Specify the Version of Unity Engine |
| --melonloader.agfoffline | Forces Assembly Generator to Run without Contacting the Remote API |
| --melonloader.agfregenerate | Forces Regeneration of Assembly |
| --melonloader.agfregex | Forces Assembly Generator to use a Specified Regex |
| --melonloader.agfvdumper | Forces Assembly Generator to use a Specified Version of Dumper |
| --melonloader.disableunityclc | Disable Unity Console Log Cleaner |


- These ones below are Cpp2IL specific Launch Options.

| Argument | Description |
| - | - |
| --cpp2il.callanalyzer | Enables CallAnalyzer processor |
| --cpp2il.nativemethoddetector | Enables NativeMethodDetector processor |

---

## PROXIES:

MelonLoader uses a proxy DLL to trick the game into loading itself on startup. This only applies for Windows.

- The Proxy DLL is able to be Renamed to the Compatible File Names below.
- By Default the Proxy is named as "version.dll".
- For most Games the Default File Name should work perfectly fine.
- Some Games may have you use a different Proxy File Name depending on the Architecture, Operating System, version of the Engine used by the Game, etc.

| File Names: |
| - |
| version.dll |
| winhttp.dll |
| winmm.dll |
| dinput.dll |
| dinput8.dll |
| dsound.dll |
| d3d8.dll |
| d3d9.dll |
| d3d10.dll |
| d3d11.dll |
| d3d12.dll |
| ddraw.dll |
| msacm32.dll |

---

## LICENSING & CREDITS:

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/LICENSE.md) for the full License.

Third-party Libraries used as Source Code and/or bundled in Binary Form:
- [Dobby](https://github.com/jmpews/Dobby) is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/jmpews/Dobby/blob/master/LICENSE) for the full License.
- [Mono](https://github.com/Unity-Technologies/mono) is licensed under multiple licenses. See [LICENSE](https://github.com/Unity-Technologies/mono/blob/unity-master/LICENSE) for full details.
- [HarmonyX](https://github.com/BepInEx/HarmonyX) is licensed under the MIT License. See [LICENSE](https://github.com/BepInEx/HarmonyX/blob/master/LICENSE) for the full License.
- [MonoMod](https://github.com/MonoMod/MonoMod) is licensed under the MIT License. See [LICENSE](https://github.com/MonoMod/MonoMod/blob/master/LICENSE) for the full License.
- [Mono.Cecil](https://github.com/jbevain/cecil) is licensed under the MIT License. See [LICENSE](https://github.com/jbevain/cecil/blob/master/LICENSE.txt) for the full License.
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) is licensed under the MIT License. See [LICENSE](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md) for the full License.
- [TinyJSON](https://github.com/pbhogan/TinyJSON) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/MelonLoader/TinyJSON/LICENSE.md) for the full License.
- [Tomlet](https://github.com/SamboyCoding/Tomlet) is licensed under the MIT License. See [LICENSE](https://github.com/SamboyCoding/Tomlet/blob/master/LICENSE) for the full License.
- [AsmResolver](https://github.com/Washi1337/AsmResolver) is licensed under the MIT License. See [LICENSE](https://github.com/Washi1337/AsmResolver/blob/master/LICENSE.md) for the full License.
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/MelonLoader/SharpZipLib/LICENSE.txt) for the full License.
- [Semver](https://github.com/maxhauser/semver) is licensed under the MIT License. See [LICENSE](https://github.com/maxhauser/semver/blob/master/License.txt) for the full License.
- [Illusion Plugin Architecture](https://github.com/Eusth/IPA) is licensed under the MIT License. See [LICENSE](https://github.com/Eusth/IPA/blob/master/LICENSE) for the full License.
- [MuseDashModLoader](https://github.com/mo10/MuseDashModLoader) is licensed under the MIT License. See [LICENSE](https://github.com/mo10/MuseDashModLoader/blob/master/LICENSE) for the full License.
- [mgGif](https://github.com/gwaredd/mgGif) is licensed under the MIT License. See [LICENSE](https://github.com/gwaredd/mgGif/blob/main/LICENSE) for the full License.
- [AssetsTools.NET](https://github.com/nesrak1/AssetsTools.NET) is licensed under the MIT License. See [LICENSE](https://github.com/nesrak1/AssetsTools.NET/blob/master/LICENSE) for the full License.
- [AssetRipper.VersionUtilities](https://github.com/AssetRipper/VersionUtilities) is licensed under the MIT License. See [LICENSE](https://github.com/AssetRipper/VersionUtilities/blob/master/License.md) for the full License.
- Steam Library, VDF, and ACF Parsing from [SteamFinder.cs](https://github.com/Umbranoxio/BeatSaberModInstaller/blob/master/BeatSaberModManager/Dependencies/SteamFinder.cs) by [Umbranoxio](https://github.com/Umbranoxio) and [Dalet](https://github.com/Dalet).
- [bHapticsLib](https://github.com/HerpDerpinstine/bHapticsLib) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/bHapticsLib/blob/master/LICENSE.md) for the full License. 
- [IndexRange](https://github.com/bgrainger/IndexRange) is licensed under the MIT License. See [LICENSE](https://github.com/bgrainger/IndexRange/blob/master/LICENSE) for the full License.  
- [ValueTupleBridge](https://github.com/OrangeCube/MinimumAsyncBridge) is licensed under the MIT License. See [LICENSE](https://github.com/OrangeCube/MinimumAsyncBridge/blob/master/LICENSE) for the full License.  
- [WebSocketDotNet](https://github.com/SamboyCoding/WebSocketDotNet) is licensed under the MIT License. See [LICENSE](https://github.com/SamboyCoding/WebSocketDotNet/blob/master/LICENSE) for the full License.
- [Pastel](https://github.com/silkfire/Pastel) is licensed under the MIT License. See [LICENSE](https://github.com/silkfire/Pastel/blob/master/LICENSE) for the full License.
- [Il2CppInterop](https://github.com/BepInEx/Il2CppInterop) is licensed under the LGPLv3 License. See [LICENSE](https://github.com/BepInEx/Il2CppInterop/blob/master/LICENSE) for the full License.

External Libraries and Tools that are downloaded and used at Runtime:
- [Cpp2IL](https://github.com/SamboyCoding/Cpp2IL) is licensed under the MIT License. See [LICENSE](https://github.com/SamboyCoding/Cpp2IL/blob/master/LICENSE) for the full License.
- Unity Runtime Libraries from [MelonLoader.UnityDependencies](https://github.com/LavaGang/MelonLoader.UnityDependencies) are part of Unity Software.  
Their usage is subject to [Unity Terms of Service](https://unity3d.com/legal/terms-of-service), including [Unity Software Additional Terms](https://unity3d.com/legal/terms-of-service/software).
- [.NET Runtime](https://github.com/dotnet/runtime) is licensed under the MIT License. See [LICENSE](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT) for the full License.

See [MelonLoader Wiki](https://melonwiki.xyz/#/credits) for the full Credits.

MelonLoader is not sponsored by, affiliated with or endorsed by Unity Technologies or its affiliates.  
"Unity" is a trademark or a registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.
