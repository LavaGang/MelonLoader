
## GENERAL INFORMATION:

- Debug Mode is for Debugging MelonLoader, Plugins, Mods.
- All Logs are made in the created MelonLoader/Logs folder in your Game's Installation Folder.
- All Plugins get placed in the created Plugins folder in your Game's Installation Folder.
- All Mods get placed in the created Mods folder in your Game's Installation Folder.  
- [Proxies](#proxies)
- [Launch Options](#launch-options)
- [dnSpy Debugger Usage](#dnspy-debugger-usage)
- [Wine / Steam Proton Usage](#wine--steam-proton-usage)
- GUIDES: [INSTALLER](#how-to-use-the-installer) | [MANUAL USE](#how-to-manually-use-melonloader)
- Example Projects: [TestPlugin](https://github.com/LavaGang/TestPlugin) & [TestMod](https://github.com/LavaGang/TestMod)

---

## REQUIREMENTS:

- [.NET Framework 3.5 Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=21)
- [.NET Framework 4.7.2 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- Microsoft Visual C++ 2015-2019 Re-distributable [[x86](https://aka.ms/vs/16/release/vc_redist.x86.exe)] [[x64](https://aka.ms/vs/16/release/vc_redist.x64.exe)]

---

## HOW TO USE THE INSTALLER:

- [INSTALL](https://github.com/LavaGang/MelonLoader.Installer/blob/master/README.md#how-to-install-re-install-or-update-melonloader) | [UPDATE](https://github.com/LavaGang/MelonLoader.Installer/blob/master/README.md#how-to-install-re-install-or-update-melonloader) | [RE-INSTALL](https://github.com/LavaGang/MelonLoader.Installer/blob/master/README.md#how-to-install-re-install-or-update-melonloader)
- [UN-INSTALL](https://github.com/LavaGang/MelonLoader.Installer/blob/master/README.md#how-to-un-install-melonloader)

---

## HOW TO MANUALLY USE MELONLOADER:

### UPDATE / RE-INSTALL:

1. Follow the Steps to [UN-INSTALL](#un-install)
2. Follow the Steps to [INSTALL](#install)


### INSTALL:

1. Make sure the Game is Closed and Not Running before attempting to Install.
2. Make sure you have all the [Requirements](#requirements) Installed before attempting to Install.
3. Download MelonLoader [[x86](https://github.com/LavaGang/MelonLoader/releases/download/v0.3.0/MelonLoader.x86.zip)] [[x64](https://github.com/LavaGang/MelonLoader/releases/download/v0.3.0/MelonLoader.x64.zip)]
4. Extract the MelonLoader folder from the MelonLoader Zip Archive to the Game's Installation Folder.
5. Extract version.dll from the MelonLoader Zip Archive to the Game's Installation Folder.


### UN-INSTALL:

1. Make sure the Game is Closed and Not Running before attempting to UN-INSTALL.
2. Remove the version.dll file from the Game's Installation Folder.
3. Remove the MelonLoader folder from the Game's Installation Folder.

These additional steps below are OPTIONAL if you want to do a FULL UN-INSTALL.

4. Remove the Plugins folder from the Game's Installation Folder.
5. Remove the Mods folder from the Game's Installation Folder.
6. Remove the UserData folder from the Game's Installation Folder.

---

## PROXIES:

- You are able to rename the Proxy DLL to the list of allowed DLLs below.
- These are mainly for compatibility reasons.

| Name |
| - |
| version.dll |
| winmm.dll |
| winhttp.dll |

---

## LOAD MODES:

- Load Mode Launch Options are a way to dictate how you want Mods or Plugins to Load.
- These are the Values that those Launch Options accept and what each of them do.

| Value | Action |
| - | - |
| 0 | Load Only if the File doesn't have the ".dev.dll" Extension |
| 1 | Load Only if the File has the ".dev.dll" Extension |
| 2 | Load All |

---

## WINE / STEAM PROTON USAGE:

- COMING SOON

---

## DNSPY DEBUGGER USAGE:

- COMING SOON

---

## LAUNCH OPTIONS:

- These are additional Launch Options that MelonLoader adds to the Game.
- These can be used to manipulate how MelonLoader works.
- After First Launch with MelonLoader these Launch Options can also be configured in MelonLoader/LaunchOptions.ini  
If a Launch Option is in your Game's Command Line it will ignore its value in the INI file.

| Argument | Description |
| - | - |
| --no-mods | Launch game without loading any Plugins Mods |
| --quitfix | Fixes the Hanging Process Issue with some Games |
| --melonloader.consoleontop | Forces the Console to always stay on-top of all other Applications |
| --melonloader.consoledst | Keeps the Console Title as Original |
| --melonloader.hideconsole | Hides the Console |
| --melonloader.hidewarnings | Hides Warnings from Displaying |
| --melonloader.debug | Debug Mode |
| --melonloader.dab | Debug Analytics Blocker |
| --melonloader.magenta | Magenta Console Color |
| --melonloader.rainbow | Rainbow Console Color |
| --melonloader.randomrainbow | Random Rainbow Console Color |
| --melonloader.maxlogs | Max Log Files [ Default: 10 ] [ Disable: 0 ] |
| --melonloader.maxwarnings | Max Warnings per Log File [ Default: 100 ] [ Disable: 0 ] |
| --melonloader.maxerrors | Max Errors per Log File [ Default: 100 ] [ Disable: 0 ] |
| --melonloader.loadmodeplugins | Load Mode for Plugins [ Default: 0 ] |
| --melonloader.loadmodemods | Load Mode for Mods [ Default: 0 ] |
| --melonloader.agfregenerate | Forces Regeneration of Assembly |
| --melonloader.agfvunity | Forces use a Specified Version of Unity Dependencies |
| --melonloader.agfvdumper | Forces use a Specified Version of Dumper |
| --melonloader.agfvunhollower | Forces use a Specified Version of Il2CppAssemblyUnhollower |

---

## LICENSING & CREDITS:

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/alpha-development/LICENSE.md) for the full License.

Third-party libraries used as Source Code and/or bundled in Binary Form:
- [Research Detours Package](https://github.com/microsoft/Detours) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/alpha-development/Detours/LICENSE.md) for the full License.
- [Mono](https://github.com/Unity-Technologies/mono) is licensed under multiple licenses. See [LICENSE](https://github.com/Unity-Technologies/mono/blob/unity-master/LICENSE) for full details.
- [HarmonyX](https://github.com/BepInEx/HarmonyX) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/alpha-development/MelonLoader/Libs/HarmonyX/LICENSE) for the full License.
- [MonoMod](https://github.com/MonoMod/MonoMod) is licensed under the MIT License. See [LICENSE](https://github.com/MonoMod/MonoMod/blob/master/LICENSE) for the full License.
- [TinyJSON](https://github.com/pbhogan/TinyJSON) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/alpha-development/MelonLoader/Libs/TinyJSON/LICENSE.md) for the full License.
- [Tomlyn](https://github.com/xoofx/Tomlyn) is licensed under the BSD 2-Clause "Simplified" License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/alpha-development/MelonLoader/Libs/Tomlyn/license.txt) for the full License.
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/alpha-development/MelonLoader/Libs/SharpZipLib/LICENSE.txt) for the full License.
- [UnityEngine.Il2CppAssetBundleManager](https://github.com/LavaGang/UnityEngine.Il2CppAssetBundleManager) is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/UnityEngine.Il2CppAssetBundleManager/blob/master/LICENSE.md) for the full License.
- [UnityEngine.Il2CppImageConversionManager](https://github.com/LavaGang/UnityEngine.Il2CppImageConversionManager) is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/UnityEngine.Il2CppImageConversionManager/blob/master/LICENSE.md) for the full License.
- [bHaptics Haptic Library](https://github.com/bhaptics/haptic-library) is licensed under All rights reserved Copyright (c) 2016-2021 bHaptics Inc.  
See [Terms and Conditions](https://www.bhaptics.com/legals/terms-and-conditions) for the full License. Express permission from the company.

External libraries and tools downloaded and used at runtime:
- [Cpp2IL](https://github.com/SamboyCoding/Cpp2IL) is licensed under the MIT License. See [LICENSE](https://github.com/SamboyCoding/Cpp2IL/blob/master/LICENSE) for the full License.
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) is licensed under the MIT License. See [LICENSE](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) for the full License.
- [Il2CppAssemblyUnhollower](https://github.com/knah/Il2CppAssemblyUnhollower) is licensed under the GNU Lesser General Public License v3.0. See [LICENSE](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) for the full License.
- Unity Runtime Libraries from [Unity-Runtime-Libraries](https://github.com/LavaGang/Unity-Runtime-Libraries) are part of Unity Software.  
Their usage is subject to [Unity Terms of Service](https://unity3d.com/legal/terms-of-service), including [Unity Software Additional Terms](https://unity3d.com/legal/terms-of-service/software).

See [MelonLoader Wiki](https://melonwiki.xyz/#/credits) for the full Credits.

MelonLoader is not sponsored by, affiliated with or endorsed by Unity Technologies or its affiliates.  
"Unity" is a trademark or a registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.
