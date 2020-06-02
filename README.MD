### AUTOMATED INSTALL:

1. Download MelonLoader.Installer.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2. Run MelonLoader.Installer.exe and Follow the Instructions.

---
	
### MANUAL INSTALLATION FOR MONO GAMES:

1. Download MelonLoader.Mono.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2. Extract MelonLoader.Mono.zip to the Game's Installation Folder.

---

### MANUAL INSTALLATION FOR IL2CPP GAMES:

1. Download MelonLoader.Il2Cpp.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2. Extract MelonLoader.Il2Cpp.zip to the Game's Installation Folder.

---

### MANUAL UNINSTALL:

1. Remove the version.dll file from the Game's Installation Folder.
2. Remove the MelonLoader folder from the Game's Installation Folder.
3. Remove the Mods folder from the Game's Installation Folder.
4. Remove the Logs folder from the Game's Installation Folder.

---

### GENERAL INFORMATION:

- All Mods get placed in the created Mods folder inside the Game's Install Folder.
- All Preload Mods get placed in the created Mods/PRELOAD folder inside the Game's Install Folder.
- All Logs are made in the created Logs folder inside the Game's Install Folder.

---

### LAUNCH OPTIONS:

| Argument              | Description                              |
| --------------------- | ---------------------------------------- |
| --no-mods             | Launch game without loading Mods         |
| --melonloader.debug   | Debug Console                            |
| --melonloader.quitfix   | Fixes the Hanging Process Issue with some Games |
| --melonloader.maxlogs   | Max Log Files   [ Default: 10 ] [ Disable: 0 ] |
| --melonloader.rainbow | Rainbow Console Color                |
| --melonloader.randomrainbow | Random Rainbow Console Color |

---

### CONSOLE DIFFERENCES:

- Normal Console is for the Logger class used by MelonLoader.ModHandler and Mods for debugging and developing Mods.
- Debug Console is for the debugging of MelonLoader internals.

#### Specific combinations of Compile Versions will open a specific Console by Default:

| MelonLoader | ModHandler | Result                        |
| ----------- | ---------- | ----------------------------- |
| DEBUG       | RELEASE    | Debug Console                 |
| RELEASE     | DEBUG      | Normal Console                |
| DEBUG       | DEBUG      | Debug Console                 |
| RELEASE     | RELEASE    | Left up to the Launch Options |

---

### CREDITS+LICENSING:

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/LICENSE) for the full License.  
[MelonLoader.dll](MelonLoader) was adapted from [AtiRoNya](https://github.com/AtiLion/AtiRoNya) by [AtiLion](https://github.com/AtiLion), licensed under [MIT License](https://github.com/AtiLion/AtiRoNya/blob/e20e4a8fc47b37834c8284f9e6e937f04a84c510/LICENSE)

Third-party libraries used as source code or bundled in binary form:
- [Research Detours Package](https://github.com/microsoft/Detours) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/Detours/LICENSE.md) for the full License.
- [Mono](https://github.com/Unity-Technologies/mono) is licensed under multiple licenses. See [LICENSE](https://github.com/Unity-Technologies/mono/blob/unity-master/LICENSE) for full details.
- [Harmony](https://github.com/pardeike/Harmony) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.ModHandler/Harmony/LICENSE) for the full License.
- Unity Runtime libraries (found in [Unity Dependencies](BaseLibs/Unity%20Dependencies)) are part of Unity Software. Their usage is subject to [Unity Terms of Service](https://unity3d.com/legal/terms-of-service), including [Unity Software Additional Terms](https://unity3d.com/legal/terms-of-service/software).
- [TinyJSON](https://github.com/pbhogan/TinyJSON) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.ModHandler/TinyJSON/LICENSE.md) for the full License.

External tools downloaded and used at runtime:
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) is licensed under the MIT License. See [LICENSE](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) for the full License.
- [Il2CppAssemblyUnhollower](https://github.com/knah/Il2CppAssemblyUnhollower) is licensed under the GNU Lesser General Public License v3.0. See [LICENSE](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) for the full License.

---

See [MelonLoader Wiki](https://melonwiki.xyz/#/credits) for the full Credits.

MelonLoader is not sponsored by, affiliated with or endorsed by Unity Technologies or its affiliates.
"Unity" is a trademark or a registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.
