<p align="center">
  <a href="#"><img src="https://cdn.discordapp.com/attachments/738116823035150356/904704321856352296/FL_Logo.png"></a>
  <a href="#"><img src="https://cdn.discordapp.com/attachments/738116823035150356/904711558712684584/FL_Text.png"></a>
</p>


## DISCLAIMER!:
- Now open source!
- FreedomLoader is a fork of [MelonLoader](https://github.com/lavagang/MelonLoader)
- Changes Include:
- * Allowing Obfuscated Mods
- * Removing certain Mods from the URL Blocklist
- * Bypassing Integrity Checks by VRCMG

## PLEASE NOTE:
FreedomLoader is intended to give you the freedom of running any mod, wether it is obfuscated or not.

This comes with a few Security Concerns:
* Obfuscated Mods can hide basically anything they want inside of them.
* You may run the risk of getting your Accounts compromised.
* This goes beyond just the game your modding, nothing prevents a mod (In the official MelonLoader too) from getting your discord token, or sniffing around your browser's password storage. The difference being on MelonLoader, only mods where you can read the source code (either by being open source, or using software like DnSpy) can run. This makes FreedomLoader inherently less secure.
* We do still block malicious URL's (except for certain Mods, because otherwise they couldn't even run)

## GENERAL INFORMATION:

- Debug Mode is for Development Purposes. Use it to help Develop and Debug MelonLoader, Plugins, and Mods.
- All Logs are made in the created MelonLoader/Logs folder in your Game's Installation Folder.
- All Plugins get placed in the created Plugins folder in your Game's Installation Folder.
- All Mods get placed in the created Mods folder in your Game's Installation Folder.
<br></br>
- [The Official Wiki](https://melonwiki.xyz)
- [Proxies](#proxies)
- [Launch Options](#launch-options)
- [dnSpy Debugger Usage](#dnspy-debugger-usage)
- [Wine / Steam Proton Usage](#wine--steam-proton-usage)
- [Android & Oculus Quest Support (__WIP__)](https://melonwiki.xyz/#/android/general)

| Usage Guides: |
| - |
| [INSTALLER](#how-to-use-the-installer) |
| [MANUAL USE](#how-to-manually-use-melonloader) |

| Example Projects: |
| - |
| [TestPlugin](https://github.com/LavaGang/TestPlugin) |
| [TestMod](https://github.com/LavaGang/TestMod) |

---

## REQUIREMENTS:

- [.NET Framework 3.5 Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=21)
- [.NET Framework 4.7.2 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- Microsoft Visual C++ 2015-2019 Re-distributable [[x86](https://aka.ms/vs/16/release/vc_redist.x86.exe)] [[x64](https://aka.ms/vs/16/release/vc_redist.x64.exe)]

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

- The Proxy DLL is able to be Renamed to the Compatible File Names below.
- By Default the Proxy is named as "version.dll".
- For most Unity Games the Default File Name should work perfectly fine.
- Some Unity Games may have you use a different Proxy File Name depending on the Architecture, Operating System, version of the Unity Engine used by the Game, etc.

| File Names: |
| - |
| psapi.dll |
| version.dll |
| winhttp.dll |
| winmm.dll |

---

## LOAD MODES:

- Load Mode Launch Options are a way to dictate how you want Mods or Plugins to Load.
- Below is the Compatible Values and what each of them do.

| Value | Action |
| - | - |
| 0 | Load Only if the File doesn't have the ".dev.dll" Extension |
| 1 | Load Only if the File has the ".dev.dll" Extension |
| 2 | Load All |

---

## CONSOLE DISPLAY MODES:

- Console Display Modes are built in Themes for the Console
- Below is the Compatible Values and what each of them do.

| Value | Mode |
| - | - |
| 0 | Normal |
| 1 | Magenta |
| 2 | Rainbow |
| 3 | Random Rainbow |

---

## WINE / STEAM PROTON USAGE:

- GUIDE COMING SOON

---

## DNSPY DEBUGGER USAGE:

1. Go into dnSpy, then in the top, select `Debug` -> `Start Debugging` and select `Unity` as the debug engine.
2. Then, for the executable, select your game's executable
3. And in the `Arguments` box, add `--melonloader.debug` and any other arguments you want
4. Finally, to add breakpoints, simply drag and drop the mod you want to debug into dnSpy's Assembly Explorer, then add breakpoints regularly.

You can read more about attaching the dnSpy debugger in the [MelonLoader wiki](https://melonwiki.xyz/#/modders/debugging).

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
| --melonloader.dab | Debug Analytics Blocker |
| --melonloader.maxlogs | Max Log Files [ Default: 10 ] [ NoCap: 0 ] |
| --melonloader.maxwarnings | Max Warnings per Log File [ Default: 100 ] [ NoCap: 0 ] [ Disabled: -1 ] |
| --melonloader.maxerrors | Max Errors per Log File [ Default: 100 ] [ NoCap: 0 ] [ Disabled: -1 ] |
| --melonloader.loadmodeplugins | Load Mode for Plugins [ Default: 0 ] |
| --melonloader.loadmodemods | Load Mode for Mods [ Default: 0 ] |
| --melonloader.agfoffline | Forces Assembly Generator to Run without Contacting the Remote API |
| --melonloader.agfregenerate | Forces Regeneration of Assembly |
| --melonloader.agfvunity | Forces Assembly Generator to use a Specified Version of Unity Dependencies |
| --melonloader.agfvdumper | Forces Assembly Generator to use a Specified Version of Dumper |
| --melonloader.agfvunhollower | Forces Assembly Generator to use a Specified Version of Il2CppAssemblyUnhollower |
| --melonloader.basedir | Changes the Proxy's Load Directory for the Bootstrap |

---

## LICENSING & CREDITS:

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/LICENSE.md) for the full License.

Third-party Libraries used as Source Code and/or bundled in Binary Form:
- [Research Detours Package](https://github.com/microsoft/Detours) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/Bootstrap/Base/MSDetours/LICENSE.md) for the full License.
- [Mono](https://github.com/Unity-Technologies/mono) is licensed under multiple licenses. See [LICENSE](https://github.com/Unity-Technologies/mono/blob/unity-master/LICENSE) for full details.
- [HarmonyX](https://github.com/BepInEx/HarmonyX) is licensed under the MIT License. See [LICENSE](https://github.com/BepInEx/HarmonyX/blob/master/LICENSE) for the full License.
- [MonoMod](https://github.com/MonoMod/MonoMod) is licensed under the MIT License. See [LICENSE](https://github.com/MonoMod/MonoMod/blob/master/LICENSE) for the full License.
- [Mono.Cecil](https://github.com/jbevain/cecil) is licensed under the MIT License. See [LICENSE](https://github.com/jbevain/cecil/blob/master/LICENSE.txt) for the full License.
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) is licensed under the MIT License. See [LICENSE](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md) for the full License.
- [TinyJSON](https://github.com/pbhogan/TinyJSON) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/MelonLoader/Libs/TinyJSON/LICENSE.md) for the full License.
- [Tomlet](https://github.com/SamboyCoding/Tomlet) is licensed under the MIT License. See [LICENSE](https://github.com/SamboyCoding/Tomlet/blob/master/LICENSE) for the full License.
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/MelonLoader/Libs/SharpZipLib/LICENSE.txt) for the full License.
- [UnityEngine.Il2CppAssetBundleManager](https://github.com/LavaGang/UnityEngine.Il2CppAssetBundleManager) is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/UnityEngine.Il2CppAssetBundleManager/blob/master/LICENSE.md) for the full License.
- [UnityEngine.Il2CppImageConversionManager](https://github.com/LavaGang/UnityEngine.Il2CppImageConversionManager) is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/UnityEngine.Il2CppImageConversionManager/blob/master/LICENSE.md) for the full License.
- [Illusion Plugin Architecture](https://github.com/Eusth/IPA) is licensed under the MIT License. See [LICENSE](https://github.com/Eusth/IPA/blob/master/LICENSE) for the full License.
- [MuseDashModLoader](https://github.com/mo10/MuseDashModLoader) is licensed under the MIT License. See [LICENSE](https://github.com/mo10/MuseDashModLoader/blob/master/LICENSE) for the full License.
- [bHaptics Haptic Library](https://github.com/bhaptics/haptic-library) is licensed under All rights reserved Copyright (c) 2016-2021 bHaptics Inc.  
See [Terms and Conditions](https://www.bhaptics.com/legals/terms-and-conditions) for the full License. We have Express Permission from bHaptics.

External Libraries and Tools that are downloaded and used at Runtime:
- [Cpp2IL](https://github.com/SamboyCoding/Cpp2IL) is licensed under the MIT License. See [LICENSE](https://github.com/SamboyCoding/Cpp2IL/blob/master/LICENSE) for the full License.
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) is licensed under the MIT License. See [LICENSE](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) for the full License.
- [Il2CppAssemblyUnhollower](https://github.com/knah/Il2CppAssemblyUnhollower) is licensed under the GNU Lesser General Public License v3.0. See [LICENSE](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) for the full License.
- Unity Runtime Libraries from [Unity-Runtime-Libraries](https://github.com/LavaGang/Unity-Runtime-Libraries) are part of Unity Software.  
Their usage is subject to [Unity Terms of Service](https://unity3d.com/legal/terms-of-service), including [Unity Software Additional Terms](https://unity3d.com/legal/terms-of-service/software).

See [MelonLoader Wiki](https://melonwiki.xyz/#/credits) for the full Credits.

MelonLoader is not sponsored by, affiliated with or endorsed by Unity Technologies or its affiliates.  
"Unity" is a trademark or a registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.
