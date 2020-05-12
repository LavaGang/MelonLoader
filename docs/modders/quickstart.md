# Quick Start

!> This tutorial assumes that you know how to write mods using the C# programming language, and that you have some basic Visual Studio knowledges.

### Basic mod setup

!> MelonLoader currently only supports C# mods, made using .NET Framework up to version 4.7.2.

At first, you will need to create a new project of type `Class Library (.NET Framework)`, and select a version up to 4.7.2.<br>
Doing so create a new empty cs file, called `Class1`. This will be our mod main class.<br>
Let's rename it `MyMod` for now. We will come back to it later.

You will now need to reference the main MelonLoader assembly. Right click the `Reference` directory, `Add a reference...`, and click `Browse`.<br/>
Browse up to the folder of the game you installer MelonLoader on. The file you need to reference from here is `MelonLoader/MelonLoader.ModHandler.dll`.

MelonLoader rely on the assembly infos to get your mod description. We will have to setup them to make it works.<br>
To do that, go to the `Properties` directory, and add these two lines to the `AssemblyInfo.cs` file:
```cs
using MelonLoader;
// ...
[assembly: MelonModInfo(typeof(MyMod), "My Mod Name", "version", "Author Name")]
[assembly: MelonModGame("GameName", "GameStudio")]
```
MelonModInfo contains 4 parameters:
- `MyMod`: The main class of your mod. We will talk about it later
- `My Mod Name`: The name of your mod
- `version`: The version of the mod. It should respect the [semver format](https://semver.org/) (example: `1.0.0`)
- `Author Name`: The name of author of the mod

MelonModGame contains 2 parameters:
- `GameName`: The name of the game, as defined in the Unity settings.
- `GameStudio`: The name of the developer(s) of the game, as defined in the Unity settings.

?> You can get the value of `GameName` and `GameStudio` on the top of the Logs file.<br/>You can also set these two parameters to `null` if you want you mod to be Universal.

We are almost ready. Let's go back to our `MyMod` class, add a `using MelonLoader;` to the import list, and make our `MyMod` class inherit from `MelonMod`.

### The MelonMod class

At this point, your `MyMod` class should looks like this:
```cs
using MelonLoader;

namespace MyProject
{
    public class MyMod : MelonMod
    {

    }
}
```

MelonMod have a few virtual method that can be overridden:
 - `OnApplicationStart()`: Called after every mod is loaded into the current Mono Domain
 - `OnApplicationQuit()`: Called when the application is closing
 - `OnLevelWasInitialized(int level)`: Called when a scene is initialized
 - `OnLevelWasLoaded(int level)`: Called when a scene is loaded
 - `OnUpdate()`: Called at the end of each `Update` call
 - `OnLateUpdate()`: Called at the end of each `Update` call
 - `OnFixedUpdate()`: Called at the end of each `Update` call
 - `OnGUI()`: Called during the GUI update
 - `OnModSettingsApplied()`: Called when a mod calls `MelonLoader.ModPrefs.SaveConfig()`, or when the application quits.
 - `VRChat_OnUiManagerInit()`: (VRChat only) Called if the `VRCUiManager` component has been initialized this frame. This method is called at the end of the frame, before OnUpdate.

!> - `OnGUI()` is currently broken.<br/> - `VRChat_OnUiManagerInit()` may be removed in the future.


### Basic method calling

Thanks to Il2CppAssemblyUnhollower, we have a fair pack of generated proxy assemblies. These can be used as reference to call, get and set the methods, properties and fields.

Let's print something to the console.<br>
First, you will need to add a reference to `UnityEngine.CoreModule.dll` and `Il2Cppmscorlib.dll`. Both of them are in `MelonLoader/Managed/`.

```cs
public override void OnUpdate()
{
    if(Input.GetKeyDown(KeyCode.T))
    {
        MelonModLogger.Log("You just pressed T")
    }
}
```

You now have a mod that prints "You just pressed T" when you, well press the T key!
