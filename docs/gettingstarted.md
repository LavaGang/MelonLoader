# Getting Started

MelonLoader is a mod loader working on most unity games. It has initially been designed to focus Il2Cpp games, but also supports Mono games.<br/>
To use it, you will first need to install it to the desired game.

!> Please note that MelonLoader doesn't do anything on its own. You will have to install some mods to make changes to your game.

!> Also note that there are currently two standards for mods: Unhollower 0.3.1.0, and Unhollower 0.2.0.0. Unhollower 0.2.0.0 is considered **legacy**, and should only be used with mods that haven't been updated.
# Installation on Il2Cpp games
### Automatic Installation

!> This method requires Windows 8 or newer.

 - Download the [MelonLoader installation script](https://github.com/thetrueyoshifan/MelonLoaderAutoInstaller/releases/download/1.4.3/MelonLoaderInstaller.bat) or [Melonloader Legacy installation script](https://github.com/Slaynash/MelonLoaderAutoInstaller/releases/download/v1.4.2/MelonLoaderInstaller.bat). We will call it `MelonLoaderInstaller.bat`.
 - Move MelonLoaderInstaller.bat to your game folder, next to your game executable.<br/>
 On steam, you can find the folder by right clicking your game name, clicking `Properties...`, going to the `LOCAL FILES` tab, and pressing the `BROWSE LOCAL FILES...` button.

<div align="center">
    <img src="\_media/ml_install_example.png "Example of installation on BONEWORKS">
</div>

 - Double-click MelonLoaderInstaller.bat. A command prompt should shows up, and start installing MelonLoader on your game.<br/>
 In case the installation fails, tell us in the #melonloader-support channel of the [MelonLoader Discord](https://discord.gg/2Wn3N2P).
 - MelonLoader is now installed to your game! To install a new mod, you will have to download it and put it in the newly-created `Mods` folder (located in your game directory).<br>
 You can find some mods in the Officially Supported Games category.


### Manual Installation

?> If you need any help, feel free to ask us in the #melonloader-support channel of the [MelonLoader Discord](https://discord.gg/2Wn3N2P)!

#### Video version
<div align="center">
    <iframe width="560" height="315" style="min-width: 560px" src="https://www.youtube.com/embed/0Jpi9i4HSsI" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

#### Text version
- Download <https://github.com/HerpDerpinstine/MelonLoader/releases/download/v0.1.0/MelonLoader.zip>
- Copy every files inside it to your game directory (where the game executable is)
- Download <https://github.com/Perfare/Il2CppDumper/releases/download/v6.2.1/Il2CppDumper-v6.2.1.zip>
- Extract the files to a new folder named `il2cppdumper` in your game directory
- Download <https://github.com/Slaynash/MelonLoaderAutoInstaller/raw/master/il2cppdumper_config.json> to `<Game/il2cppdumper/config.json` (replace the file)
- Download <https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v0.3.1.0/Il2CppAssemblyUnhollower.0.3.1.0.7z> (or <https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v0.2.0.0/Il2CppAssemblyUnhollower.0.2.0.0.7z> for legacy mods)
- Extract the files to a new folder named `il2cppassemblyunhollower` in your game directory.<br/>
At this point, your game hierarchy should looks be like this:
```
<Game>
|-<Game>_Data\
|-il2cppdumper\
| |-Il2CppDumper.exe
| |-config.json
|  `...
|-il2cppassemblyunhollower\
| |-AssemblyUnhollower.exe
|  `...
|-MelonLoader\
| |-Managed\
| |-Mono\
| |-MelonLoader.dll
|  `MelonLoader.ModHandler.dll
|-<Game>.exe
|-GameAssembly.dll
|-winmm.dll
 `...
```
- In your game directory, right click and select "Open PowerShell window here"
- Enter the following commands line by line (replace `BONEWORKS` by the name of the game you are installing it on):
```batch
cd il2cppdumper
.\Il2CppDumper.exe ..\GameAssembly.dll ..\BONEWORKS_Data\il2cpp_data\Metadata\global-metadata.dat
cd ..
mkdir il2cppassemblyunhollower_output
.\il2cppassemblyunhollower\AssemblyUnhollower.exe --input=il2cppdumper\DummyDll --output=il2cppassemblyunhollower_output --mscorlib=MelonLoader\Managed\mscorlib.dll
OR (for legacy mods)
.\il2cppassemblyunhollower\AssemblyUnhollower.exe il2cppdumper\DummyDll il2cppassemblyunhollower_output MelonLoader\Managed\mscorlib.dll
robocopy il2cppassemblyunhollower_output MelonLoader\Managed /XC /XN /XO /NFL /NDL /NJH
```
- Delete the folders `il2cppdumper`, `il2cppassemblyunhollower` and `il2cppassemblyunhollower_output`
- Create a folder named `Mods`
- MelonLoader is now installed to your game! To install a new mod, you will have to download it and put it in the newly-created `Mods` folder (located in your game directory).<br>
You can find some mods in the Officially Supported Games category.

# Installation on Mono games

!> Coming Soon

# Launch commands

MelonLoader have a few launch arguments, as defined here:

| Argument                    | Description                              |
| --------------------------- | ---------------------------------------- |
| --no-mods                   | Launch game without loading Mods         |
| --melonloader.console       | Normal Console                           |
| --melonloader.debug         | Debug Console                            |
| --melonloader.mupot         | Experimental MUPOT Mode for IL2CPP Games |
| --melonloader.rainbow       | Rainbow Console Color                    |
| --melonloader.randomrainbow | Random Rainbow Console Color             |
