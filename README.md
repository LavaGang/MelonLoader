### GENERAL INFORMATION:

- Debug Mode is for Debugging MelonLoader, Plugins, Mods.
- All Logs are made in the created MelonLoader/Logs folder in your Game's Installation Folder.
- All Plugins get placed in the created Plugins folder in your Game's Installation Folder.
- All Mods get placed in the created Mods folder in your Game's Installation Folder.
- Example Projects: [TestPlugin](https://github.com/LavaGang/TestPlugin) & [TestMod](https://github.com/LavaGang/TestMod)
- [Il2Cpp dnSpy Debugger Usage](#il2cpp-dnspy-debugger-usage)

---

### REQUIREMENTS:

- [.NET Framework 3.5 Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=21)
- [.NET Framework 4.7.2 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- Microsoft Visual C++ 2015-2019 Re-distributable [[x86](https://aka.ms/vs/16/release/vc_redist.x86.exe)] [[x64](https://aka.ms/vs/16/release/vc_redist.x64.exe)]

---

### AUTOMATED INSTALLATION:

1. Make sure the Game is Closed and Not Running before attempting to Install.
2. Make sure you have all the [Requirements](#requirements) Installed before attempting to Install.
3. Download [MelonLoader.Installer.exe](https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.Installer.exe).
4. Run MelonLoader.Installer.exe.
5. Click the SELECT button.
6. Select and Open the Game's EXE in your Game's Installation Folder.
7. Select which Version of MelonLoader to install using the Drop-Down List.  (Or leave it as-is for the Latest Version.)
8. Click the INSTALL or RE-INSTALL or UPDATE button.

---

### MANUAL INSTALLATION:

1. Make sure the Game is Closed and Not Running before attempting to Install.
2. Make sure you have all the [Requirements](#requirements) Installed before attempting to Install.
3. Download MelonLoader [[x86](https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x86.zip)] [[x64](https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x64.zip)]
4. Extract the MelonLoader folder from MelonLoader.zip to the Game's Installation Folder.
5. Extract version.dll from MelonLoader.zip to the Game's Installation Folder.

---

### AUTOMATED UN-INSTALL:

1. Make sure the Game is Closed and Not Running before attempting to Uninstall.
2. Make sure you have all the [Requirements](#requirements) Installed before attempting to Uninstall.
3. Download [MelonLoader.Installer.exe](https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.Installer.exe).
4. Run MelonLoader.Installer.exe.
5. Click the SELECT button.
6. Select and Open the Game's EXE in your Game's Installation Folder.
7. Click the UN-INSTALL button.

---

### MANUAL UN-INSTALL:

1. Make sure the Game is Closed and Not Running before attempting to Uninstall.
2. Remove the version.dll file from the Game's Installation Folder.
3. Remove the MelonLoader folder from the Game's Installation Folder.

The additional steps below are OPTIONAL if you want to do a Full Uninstall.

4. Remove the Plugins folder from the Game's Installation Folder.
5. Remove the Mods folder from the Game's Installation Folder.

---

### PROXIES:

- You are able to rename the Proxy DLL to the list of allowed DLLs below. These are mainly for compatibility reasons.

| Name |
| - |
| version.dll |
| winmm.dll |
| winhttp.dll |

---

### LOAD MODES:

- Load Mode launch options are a way to dictate how you want Mods or Plugins to Load.

| Value | Action |
| - | - |
| 0 | Load them only if they don't have the ".dev.dll" Extension |
| 1 | Load them only if they have the ".dev.dll" Extension |
| 2 | Load All |

---

### IL2CPP DNSPY DEBUGGER USAGE:

-

---

### LAUNCH OPTIONS:

| Argument | Description |
| - | - |
| --no-mods | Launch game without loading Mods |
| --quitfix | Fixes the Hanging Process Issue with some Games |
| --melonloader.consoleontop | Forces the Console over all other Applications |
| --melonloader.hideconsole | Hides the Normal Console |
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
| --melonloader.loadmodemods  | Load Mode for Mods [ Default: 0 ] |
| --melonloader.agregenerate | Forces Assembly to be Regenerated on Il2Cpp Games |
| --melonloader.agfvunity | Forces use a Specified Version of Unity Dependencies |
| --melonloader.agfvdumper | Forces use a Specified Version of Il2CppDumper |
| --melonloader.agfvunhollower | Forces use a Specified Version of Il2CppAssemblyUnhollower |

---

### LICENSING & CREDITS:

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/LICENSE.md) for the full License.

Third-party libraries used as source code or bundled in binary form:
- [Research Detours Package](https://github.com/microsoft/Detours) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/Detours/LICENSE.md) for the full License.
- [Mono](https://github.com/Unity-Technologies/mono) is licensed under multiple licenses. See [LICENSE](https://github.com/Unity-Technologies/mono/blob/unity-master/LICENSE) for full details.
- [Harmony](https://github.com/pardeike/Harmony) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/MelonLoader/Libs/Harmony/LICENSE) for the full License.
- [TinyJSON](https://github.com/pbhogan/TinyJSON) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/MelonLoader/Libs/TinyJSON/LICENSE.md) for the full License.
- [Tomlyn](https://github.com/xoofx/Tomlyn) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/MelonLoader/Libs/Tomlyn/license.txt) for the full License.
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib) is licensed under the MIT License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/MelonLoader/Libs/SharpZipLib/LICENSE.txt) for the full License.
- [UnityEngine.Il2CppAssetBundleManager](https://github.com/LavaGang/UnityEngine.Il2CppAssetBundleManager) is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/UnityEngine.Il2CppAssetBundleManager/blob/master/LICENSE.md) for the full License.
- [Unity Runtime Libraries](https://github.com/LavaGang/Unity-Runtime-Libraries) are part of Unity Software.  
Their usage is subject to [Unity Terms of Service](https://unity3d.com/legal/terms-of-service), including [Unity Software Additional Terms](https://unity3d.com/legal/terms-of-service/software).

External tools downloaded and used at runtime:
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) is licensed under the MIT License. See [LICENSE](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) for the full License.
- [Il2CppAssemblyUnhollower](https://github.com/knah/Il2CppAssemblyUnhollower) is licensed under the GNU Lesser General Public License v3.0. See [LICENSE](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) for the full License.

See [MelonLoader Wiki](https://melonwiki.xyz/#/credits) for the full Credits.

MelonLoader is not sponsored by, affiliated with or endorsed by Unity Technologies or its affiliates.  
"Unity" is a trademark or a registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.