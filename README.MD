### AUTOMATED INSTALL / UNINSTALL:

- MelonManager Coming Soon...

---
	
### MANUAL INSTALLATION FOR MONO GAMES:

1.  Download MelonLoader.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2.  Extract MelonLoader.zip to the Game's Installation Folder.

---

### MANUAL INSTALLATION FOR IL2CPP GAMES:

1.  Download MelonLoader.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2.  Extract MelonLoader.zip to the Game's Installation Folder.
3.  Download MonoDependencies.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
4.  Extract MonoDependencies.zip to the Game's Installation Folder.

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
| --melonloader.console | Normal Console                           |
| --melonloader.debug   | Debug Console                            |
| --melonloader.quitfix   | Fixes the Hanging Process Issue with some Games |
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

### CREDITS:

| Source | Author | License |
| ----------- | ---------- | ---------- |
| [Research Detours Package](https://github.com/microsoft/Detours) | [Microsoft](https://github.com/microsoft) | [MIT](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader/detours/LICENSE.md) |
| [UnityCsReference](https://github.com/Unity-Technologies/UnityCsReference) | [Unity Technologies](https://github.com/Unity-Technologies) | [Unity Reference-Only](https://unity3d.com/legal/licenses/Unity_Reference_Only_License) |
| UnityPlayer | [Unity Technologies](https://github.com/Unity-Technologies) | [Unity Reference-Only](https://unity3d.com/legal/licenses/Unity_Reference_Only_License) |
| Il2Cpp | [Unity Technologies](https://github.com/Unity-Technologies) | [Unity Reference-Only](https://unity3d.com/legal/licenses/Unity_Reference_Only_License) |
| [Mono](https://github.com/Unity-Technologies/mono) | [Unity Technologies](https://github.com/Unity-Technologies) | [MIT](https://github.com/Unity-Technologies/mono/blob/unity-master/LICENSE) |
| [Harmony](https://github.com/pardeike/Harmony) | [pardeike](https://github.com/pardeike) | [MIT](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.ModHandler/Harmony/LICENSE) |
| [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) | [Perfare](https://github.com/Perfare) | [MIT](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) |
| [Il2CppAssemblyUnhollower](https://github.com/knah/Il2CppAssemblyUnhollower) | [knah](https://github.com/knah) | [GNUv3](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) |
- See [WIKI](https://melonwiki.xyz/#/credits) for the full Credits.

---

### LICENSING:

- MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/LICENSE) for the full License.
- [Research Detours Package](https://github.com/microsoft/Detours) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader/detours/LICENSE.md) for the full License.
- [UnityCsReference](https://github.com/Unity-Technologies/UnityCsReference) is licensed under the Unity Reference-Only License. See [LICENSE](https://unity3d.com/legal/licenses/Unity_Reference_Only_License) for the full License.
- UnityPlayer is licensed under the Unity Reference-Only License. See [LICENSE](https://unity3d.com/legal/licenses/Unity_Reference_Only_License) for the full License.
- Il2Cpp is licensed under the Unity Reference-Only License. See [LICENSE](https://unity3d.com/legal/licenses/Unity_Reference_Only_License) for the full License.
- [Mono](https://github.com/Unity-Technologies/mono) is licensed under the MIT License. See [LICENSE](https://github.com/Unity-Technologies/mono/blob/unity-master/LICENSE) for the full License.
- [Harmony](https://github.com/pardeike/Harmony) is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.ModHandler/Harmony/LICENSE) for the full License.
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) is licensed under the MIT License. See [LICENSE](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) for the full License.
- [Il2CppAssemblyUnhollower](https://github.com/knah/Il2CppAssemblyUnhollower) is licensed under the GNU Lesser General Public License v3.0. See [LICENSE](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) for the full License.