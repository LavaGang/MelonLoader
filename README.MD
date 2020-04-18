Compile winmm64, MelonLoader, and MelonLoader.ModHandler then follow the installation instructions below.

---

## IL2CPP GAME INSTALLATION:

1.  Place `winmm.dll` next to the game's EXE
2.  Copy the `MelonLoader\` folder from `<REPO>\BaseLibs\` to the game's install folder next to the exe and `winmm.dll`
3.  Place `MelonLoader.dll` and `MelonLoader.ModHandler.dll` in the `MelonLoader\` folder in the game's install folder
4.  [Find the game's Unity version](#get-unity-version) in Unity Dependencies in BaseLibs
    - If it does not exist you need to [generate it yourself](#unity-dependency-generation)
5.  Extract the zip to the `MelonLoader\` folder in the game's install folder

## MONO GAME INSTALLATION:

1.  Place `winmm.dll` next to the game's exe.
2.  Create a new folder named `MelonLoader\` in the game's install folder.
3.  Place `MelonLoader.dll` and `MelonLoader.ModHandler.dll` in the `MelonLoader\` folder in the game's install folder.

---

### LAUNCH OPTIONS:

| Argument              | Description                              |
| --------------------- | ---------------------------------------- |
| --no-mods             | Launch game without loading Mods         |
| --melonloader.console | Normal Console                           |
| --melonloader.debug   | Debug Console                            |
| --melonloader.mupot   | Experimental MUPOT Mode for IL2CPP Games |

---

### GET UNITY VERSION

1. Open the file `\<GAME_PATH>\<GAME_NAME>_Data\Resources\unity default resources` with a text editor like [Notepad++](https://notepad-plus-plus.org/)
   - If you don't see it you can try guessing the Unity version by checking the file version of the main game EXE
2. In the first line you should see something like `­ 9ëd   2019.1.0b1` where `2019.1.0b1` would be the unity version

---

### UNITY DEPENDENCY GENERATION:

1. Download the correct unity version found [here](#get-unity-version)
   - Release: `https://unity3d.com/unity/whats-new/<VERSION>`
   - Beta: `https://unity3d.com/unity/beta/<VERSION>`
2. Install it (you don't need anything other than the core files [no need to select il2cpp or mono])
3. Start it and create a new project
4. Make sure that you scene is completely empty
5. Make sure your "Scripting Backend" is set to Mono
   - "Edit" -> "Project Settings..." -> "Player" -> "Configuration"
6. Compile your project ("File" -> "Build and run...")
7. Create a new folder that looks like [this](https://gist.github.com/Bluscream/120d634988a4b37cf1f97b01ecf47214#file-struct-txt) somewhere
8. Open `<UNITY_PROJECT_PATH>\<UNIT_PROJECT_NAME>_Data\Managed\`
9. Move all files that do **not** already exist in [`<REPO>\BaseLibs\MelonLoader\Managed\`](https://gitlab.com/HerpDerpinstine/MelonLoader/-/tree/master/BaseLibs/MelonLoader/Managed) into `<FOLDER_YOU_CREATED_IN_STEP_7\Managed\`
   - For comparison you can use a tool like [WinMerge](https://winmerge.org/)
   - Example Diff: https://gist.github.com/Bluscream/120d634988a4b37cf1f97b01ecf47214#file-melonloader_unity_diff-csv
   - Files not starting with Unity... and XML files should not end up in `<FOLDER_YOU_CREATED_IN_STEP_7\Managed\`!
10. Move the `UnityPlayer.dll` from `<UNITY_PROJECT_PATH>\` to `<FOLDER_YOU_CREATED_IN_STEP_7\Mono\` and rename it to `MonoUnityPlayer.dll`
11. ZIP up the `Managed\` and `Mono\` folder in a `<UNITY_VERSION>.zip` and either [pull request it into the repo directly](https://gitlab.com/HerpDerpinstine/MelonLoader/-/tree/master/BaseLibs/Unity%20Dependencies#modal-upload-blob) or drop it on the discord and mention one of the repo maintainers.

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

### LICENSING

MelonLoader is licensed under the Apache License, Version 2.0. See [LICENSE](https://gitlab.com/HerpDerpinstine/MelonLoader/-/blob/master/LICENSE) for the full license text.