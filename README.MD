---

## MANUAL INSTALLATION:

1.  Download both MelonLoader.zip and MonoDependencies.zip from the latest version at https://github.com/HerpDerpinstine/MelonLoader/releases
2.  Extract MelonLoader.zip to the Game's Installation Folder.
3.  Extract MonoDependencies.zip to the Game's Installation Folder.

---

### LAUNCH OPTIONS:

| Argument              | Description                              |
| --------------------- | ---------------------------------------- |
| --no-mods             | Launch game without loading Mods         |
| --melonloader.console | Normal Console                           |
| --melonloader.debug   | Debug Console                            |
| --melonloader.mupot   | Experimental MUPOT Mode for IL2CPP Games |
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

### LICENSING

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://gitlab.com/HerpDerpinstine/MelonLoader/-/blob/master/LICENSE) for the full license text.