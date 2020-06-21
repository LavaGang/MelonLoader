### REQUIREMENTS:

- In order to run the Automated Installer and MelonLoader you must have [.NET Framework 4.7.2 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net472) installed.
- In order to run MelonLoader you must have [Microsoft Visual C++ 2015-2019 Redistributable](https://aka.ms/vs/16/release/vc_redist.x64.exe) installed.

---

### AUTOMATED INSTALLATION:

1. Download MelonLoader.Installer.exe from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2. Run MelonLoader.Installer.exe.
3. Click the SELECT button.
4. Select and Open the Game's EXE in your Game's Installation Folder.
5. Select which Version of MelonLoader to install using the Drop-Down List.  (Or leave it as-is for the Latest Version.)
6. Click the INSTALL or RE-INSTALL button.

---
	
### MANUAL INSTALLATION:

1. Download MelonLoader.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2. Extract the MelonLoader folder from MelonLoader.zip to the Game's Installation Folder.
3. Extract version.dll from MelonLoader.zip to the Game's Installation Folder.

---

### MANUAL UNINSTALL:

1. Remove the version.dll file from the Game's Installation Folder.
2. Remove the MelonLoader folder from the Game's Installation Folder.
3. Remove the Mods folder from the Game's Installation Folder.
4. Remove the Plugins folder from the Game's Installation Folder.
5. Remove the Logs folder from the Game's Installation Folder.

---

### GENERAL INFORMATION:

- All Mods get placed in the created Mods folder inside the Game's Install Folder.
- All Plugins get placed in the created Plugins folder inside the Game's Install Folder.
- All Logs are made in the created Logs folder inside the Game's Install Folder.

---

### CONSOLE DIFFERENCES:

- Normal Console is for Debugging and Displaying the Output of Plugins and Mods.
- Debug Console is for Debugging MelonLoader Internals.

---

### LAUNCH OPTIONS:

| Argument              | Description                              |
| --------------------- | ---------------------------------------- |
| --no-mods | Launch game without loading Mods |
| --quitfix | Fixes the Hanging Process Issue with some Games |
| --melonloader.hideconsole | Hides the Normal Console |
| --melonloader.debug | Debug Console |
| --melonloader.rainbow | Rainbow Console Color |
| --melonloader.randomrainbow | Random Rainbow Console Color |
| --melonloader.maxlogs | Max Log Files   [ Default: 10 ] [ Disable: 0 ] |
| --melonloader.devmodsonly | Loads only Mods with the "-dev.dll" extension |
| --melonloader.devpluginsonly | Loads only Plugins with the "-dev.dll" extension |
| --melonloader.agregenerate | Forces Assembly to be Regenerated on Il2Cpp Games |
| --melonloader.agfvunhollower | Forces the Assembly Generator to use a Specified Version of Il2CppAssemblyUnhollower |

---

### CREDITS+LICENSING:

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/LICENSE) for the full License.  
[MelonLoader.dll](MelonLoader) was adapted from [AtiRoNya](https://github.com/AtiLion/AtiRoNya) by [AtiLion](https://github.com/AtiLion), licensed under [MIT License](https://github.com/AtiLion/AtiRoNya/blob/e20e4a8fc47b37834c8284f9e6e937f04a84c510/LICENSE)

Third-party libraries used as source code or bundled in binary form:
- [Research Detours Package](https://github.com/microsoft/Detours) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/Detours/LICENSE.md) for the full License.
- [Mono](https://github.com/Unity-Technologies/mono) is licensed under multiple licenses. See [LICENSE](https://github.com/Unity-Technologies/mono/blob/unity-master/LICENSE) for full details.
- [Harmony](https://github.com/pardeike/Harmony) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.ModHandler/Harmony/LICENSE) for the full License.
- [TinyJSON](https://github.com/pbhogan/TinyJSON) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.AssemblyGenerator/TinyJSON/LICENSE.md) for the full License.
- [LightJson](https://github.com/MarcosLopezC/LightJson) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.Installer/LightJson/LICENSE.txt) for the full License.
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.ModHandler/SharpZipLib/LICENSE.txt) for the full License.
- Unity Runtime libraries (found in [Unity Dependencies](BaseLibs/Unity%20Dependencies)) are part of Unity Software.  
Their usage is subject to [Unity Terms of Service](https://unity3d.com/legal/terms-of-service), including [Unity Software Additional Terms](https://unity3d.com/legal/terms-of-service/software).

External tools downloaded and used at runtime:
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) is licensed under the MIT License. See [LICENSE](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) for the full License.
- [Il2CppAssemblyUnhollower](https://github.com/knah/Il2CppAssemblyUnhollower) is licensed under the GNU Lesser General Public License v3.0. See [LICENSE](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) for the full License.

---

See [MelonLoader Wiki](https://melonwiki.xyz/#/credits) for the full Credits.

MelonLoader is not sponsored by, affiliated with or endorsed by Unity Technologies or its affiliates.  
"Unity" is a trademark or a registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.