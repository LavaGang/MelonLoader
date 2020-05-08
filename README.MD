### AUTOMATED INSTALLATION:
MelonManager Coming Soon...

---
	
### MANUAL INSTALLATION FOR MONO GAMES:

1.  Download both MelonLoader.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2.  Extract MelonLoader.zip to the Game's Installation Folder.

---

### MANUAL INSTALLATION FOR IL2CPP GAMES:

1.  Download MelonLoader.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
2.  Extract MelonLoader.zip to the Game's Installation Folder.
3.  Download MonoDependencies.zip from the latest version in [Releases](https://github.com/HerpDerpinstine/MelonLoader/releases).
4.  Extract MonoDependencies.zip to the Game's Installation Folder.

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

---

- All Mods get placed in the created Mods folder in the game's install folder.
- All Logs are made in the created Logs folder in the game's install folder.

#### Specific combinations of Compile Versions will open a specific Console by Default:

| MelonLoader | ModHandler | Result                        |
| ----------- | ---------- | ----------------------------- |
| DEBUG       | RELEASE    | Debug Console                 |
| RELEASE     | DEBUG      | Normal Console                |
| DEBUG       | DEBUG      | Debug Console                 |
| RELEASE     | RELEASE    | Left up to the Launch Options |

---

### CREDITS:

| User | Source | License |
| ----------- | ---------- | ---------- |
| [Microsoft](https://github.com/microsoft) | Research Detours Package | [MIT](https://github.com/microsoft/Detours/blob/master/LICENSE.md) |
| [pardeike](https://github.com/pardeike) | Harmony | [MIT](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.ModHandler/Harmony/LICENSE) |
| [Perfare](https://github.com/Perfare) | Il2CppDumper | [MIT](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) |
| [knah](https://github.com/knah) | Il2CppAssemblyUnhollower | [GNUv3](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) |

---

### LICENSING:

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/LICENSE) for the full license text.

Research Detours Package is licensed under the MIT License. See [LICENSE](https://github.com/microsoft/Detours/blob/master/LICENSE.md) for the full license text.

Harmony is licensed under the MIT License. See [LICENSE](https://github.com/HerpDerpinstine/MelonLoader/blob/master/MelonLoader.ModHandler/Harmony/LICENSE) for the full license text.

Il2CppDumper is licensed under the MIT License. See [LICENSE](https://github.com/Perfare/Il2CppDumper/blob/master/LICENSE) for the full license text.

Il2CppAssemblyUnhollower is licensed under the GNU Lesser General Public License v3.0. See [LICENSE](https://github.com/knah/Il2CppAssemblyUnhollower/blob/master/LICENSE) for the full license text.