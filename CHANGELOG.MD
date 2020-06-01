### Known Issues:

1. StackTraces refuse to show line numbers.
2. Console Logs randomly do not Color properly.
3. Mono Module currently has an Issue with https/ssl.

---

### Work-in-Progress / To-Do List:

1. Fix the above issues.
2. Add Debugger Support to Mono Domain.
3. Create Method for ModPrefs to Register Categories and Preferences in a single call.
4. OnLevelIsLoading needs fixing for other games than just BONEWORKS.
5. Add Android Support.
6. Add Linux Support.
7. Add Mac OS Support.

---

### Version List:

[v0.2.1 Open-Beta](#v021-open-beta)  
[v0.2.0 Open-Beta](#v020-open-beta)  
[v0.1.0 Open-Beta](#v010-open-beta)  
[v0.0.3 Closed-Beta](#v003-closed-beta)  
[v0.0.2 Closed-Beta](#v002-closed-beta)  
[v0.0.1 Closed-Beta](#v001-closed-beta)  

---

### v0.2.1 Open-Beta:

1. Overwrite files when copying freshly generated assemblies.  (Credits to knah :D)
2. Implemented auto-download of required tools for IL2CPP games.  (Credits to knah :D)
3. Fixed Config Issue causing Assembly to always need to Regenerate.
4. Added an Installer.

### v0.2.0 Open-Beta:

1. Fixed issue with the Console still showing as Closed-Beta.
2. Fixed issue with not having MelonLoader.ModHandler.3.5.dll included in the Release zip.
3. Fixed support for 2018.4.11f1 and Ori and the Will of the Wisps.  (Credits to Hector ^.^)
4. Added 2018.4.11f1 Unity Dependencies.  (Credits to Hector ^.^)
5. Removed Unity Dependencies from Release Build.  (No Longer Needed)
6. Fixed issue with MelonCoroutines not allowing a yield of Coroutine within a Coroutine.  (Credits to Slaynash :3)
7. Revised README.MD.
8. Changed the Hook and Unhook Internal Calls in MelonLoader.Imports from internal to public to expose them and allow them to be used by Mods.
9. Revised CHANGELOG.MD.
10. Added Built-In QuitFix under the Launch Option --melonloader.quitfix  (For Games like VRChat where the Process Hangs after telling it to Exit)
11. Added Stop Methods to MelonCoroutines.  (Credits to Slaynash :3)
12. Fixed Error Handling in MelonCoroutines.  (Credits to Slaynash :3)
13. Fixed Issue with MelonCoroutines when using Non-Yielding Coroutines.  (Credits to Slaynash :3)
14. Added Support for Method Unstripping through Il2CppAssemblyUnhollower to MelonLoader.AssemblyGenerator.
15. Replaced the Doorstop winmm.dll with version.dll.
16. Fixed Compatibility with IL2CPP Games for 0Harmony in MelonLoader.ModHandler.  (Credits to zeobviouslyfakeacc :D)
17. Fixed Issue with 0Harmony not Unpatching Methods when Unloading.
18. Fixed Incompatibility Issue between 0Harmony in MelonLoader.ModHandler and Mono Game Support.
19. Added Event Fix for knah's Unhollower Logging.
20. Fixed Null Reference Exception Error when using MethodInfo of Generated Assembly Methods for 0Harmony.
21. Removed NET_SDK.
22. Added Custom Component System for the ModHandler.
23. Added Preload Support for Mods to Load before the Game Initializes.
24. Added OnPreInitialization for Mods that Preload.
25. Split support for Mono and Il2Cpp into their own Support Modules.  (Credits to knah :D)
26. Fixed Issue with OnSceneLoad in Il2Cpp Support Module.  (Credits to knah :D)
27. Split MelonCoroutines to integrate Support Module support.  (Credits to knah :D)
28. Fixed issues with needing the ModHandler compiled under a certain .NET Framework.  (Credits to knah :D)
29. Fixed issue with OnGUI.
30. Fixed Compatibility Issues with SCP Unity.
31. Fixed issue with version.dll causing UnityCrashHandler to crash.
32. Fixed Compatibility Issues with Hot Dogs, Horseshoes & Hand Grenades.
33. Added Pre 2017 Mono Support Module.
34. Made Normal Console Open by Default.
35. Removed Launch Option "--melonloader.console".
36. Added Launch Option "--melonloader.hideconsole" to Hide the Normal Console.
37. Cleaned up Il2Cpp Imports.
38. Cleaned up Mono Imports.
39. Revised Command Line Parsing.
40. Fixed issue with the Log Cleaner deleting Logs that were not made by MelonLoader.
41. Added Launch Option --melonloader.maxlogs. Sets the Max Logs for the Log Cleaner. Set as 0 to Disable the Log Cleaner.
42. Added Global Exception Handler to the Mono Domain.
43. Added Assembly Generation Integration.
44. Fixed Illegal Characters in Path Error.
45. Temporarily Disabled Zipped Mod Support.
46. Added Log Error Limiter.
47. Re-Enabled Download Links in Mod Logs.
48. Fixed issue with TestMod not loading on Mono Games.

---

### v0.1.0 Open-Beta:

1. Moved Exports in MelonLoader.ModHandler to Mono Internal Calls.
2. Cleaned up Hooking for MelonLoader Internals.
3. Added a new cleaner and faster Internal Call Resolver for an Assembly Generator Test.
4. Added 2018.4.19f1 Unity Dependencies.
5. Added additional check for MelonLoader.ModHandler needing to be built with .NET 3.5.
6. Added 2018.4.20f1 Unity Dependencies.
7. Changed lists to arrays for caching classes, fields, methods, events, and etc.  (Credits to Kronik ^.^)
8. Changed foreach to statements to for statements for faster iteration.  (Credits to Kronik ^.^)
9. Made certain fields "readonly" (Name, Flags, etc) as it won't be reassigned.  (Credits to Kronik ^.^)
10. Made SDK and Imports class static.  (Credits to Kronik ^.^)
11. Cleaned up MelonLoader.ModHandler to use less Linq when possible for improved performance.  (Credits to Kronik ^.^)
12. Added CreateInstance method to IL2CPP_Class in NET_SDK.Reflection.  (Credits to Kronik ^.^)
13. Fixed SystemTypeToIL2CPPClass in NET_SDK.IL2CPP.
14. Cleaned up TestMod.
15. Added VRChat_OnUiManagerInit for VRChat Mods.
16. Made MelonModGameAttribute work as intended and MelonLoader.ModHandler will now only load Mods marked as Compatible or Universal.
17. Temporarily removed the Guid attribute from the AssemblyInfo of both TestMod and MelonLoader.ModHandler.
18. MelonMod now properly caches their respective MelonModInfoAttribute and MelonModGameAttribute.
19. Fixed PointerUtils::FindAllPattern.
20. OnLevelWasLoaded, OnLevelWasInitialized, OnUpdate, OnFixedUpdate, OnLateUpdate, and OnGUI have been fixed and now function properly.
21. Cleaned up LoadLibraryW Hook.
22. Cleaned up and Renamed Exports in MelonLoader.ModHandler.
23. Added a HookManager system.
24. Cleaned up and Combined all Internal Hooking into HookManager.cpp and HookManager.h improving compile time.
25. Cleaned up and removed unused or otherwise unneeded includes improving compile time.
26. Used regions to further clean up and organize HookManager.cpp.
27. Added Status Log for when detecting if a Mod is Universal, Compatible, or Incompatible.
28. Cleaned up the Mod Loading functionality of MelonLoader.ModHandler.
29. Fixed issue with the Compatibility Check when using multiple MelonModGameAttribute.
30. Fixed issue with OnLevelWasLoaded and OnLevelWasInitialized getting the wrong Scene Index and only running once then never again.
31. Upgraded MelonLoader.ModHandler and TestMod to .NET v4.7.2.  (As requested by Emilia :D)
32. Made MelonModController use directly invokable delegates for faster invoking.  (Credits to Kronik ^.^)
33. Removed Unneeded Caching of MethodInfo in MelonModController.
34. Added mono-2.0-boehm.dll to Mono Check.
35. Added --melonloader.rainbow and --melonloader.randomrainbow as Launch Options.  (As requested by Hordini :3)
36. Redesigned the Logging system to be unified between MelonLoader and MelonLoader.ModHandler.
37. Colorized Errors in both Consoles.
38. Added the ability for Mods to set a Color when Logging.
39. Fixed issue with --melonloader.rainbow and --melonloader.randomrainbow not working when using --melonloader.console.
40. Added Unload Functionality. Entirely unloads MelonLoader.  (MelonLoader::UNLOAD() or MelonLoader.Imports.UNLOAD_MELONLOADER())
41. Fixed issue with using 1 or 0 for a boolean in ModPrefs.
42. Fixed issue with OnUpdate, OnFixedUpdate, and OnLateUpdate invoking too early.
43. Fixed issue with Unload Functionality not closing either Consoles.
44. Added MelonCoroutines to handle Coroutine functionality for Mods on both IL2CPP and Mono.  (Credits to knah :D)
45. Fixed issue with Logger removing Old Logs when it shouldn't.
46. Fixed issue with OnLevelWasLoaded, OnLevelWasInitialized, and VRChat_OnUiManagerInit when using knah's UnHollower.
47. Fixed Execution Order of OnApplicationStart to run before any of the other Mod Methods.
48. Added Assertion Manager to handle MelonLoader Internals.
49. Fixed issue with MelonCoroutines throwing a Cast Exception for knah's UnHollower.
50. Added Support for WaitForFixedUpdate and WaitForEndOfFrame to MelonCoroutines.
51. Fixed issue with MelonCoroutines throwing a Missing Method Exception for knah's UnHollower.
52. Added 2019.3.6f1 Unity Dependencies.
53. Fixed Internal Failure issues with RotMG Exalt and Unity 2019.3.6f1.
54. Fixed Internal Failure issues with Pistol Whip and Unity 2019.2.0f1.
55. Fixed issue with MelonLoader.ModHandler complaining about Mod Libraries.
56. Fixed issue with MelonCoroutines processing a yield of null as the same as WaitForFixedUpdate.
57. Made MelonMod use directly invokable virtuals and overrides for faster invoking.  (As requested by knah :3)
58. Removed MelonModController from MelonLoader.ModHandler.
59. Made MelonLoader.ModHandler look for the MelonMod subclass in the Mod's MelonModInfoAttribute instead of searching the entire Assembly.  (As requested by knah :3)
60. Fixed issue with Logger Imports in MelonLoader.ModHandler being passed garbage strings.

---

### v0.0.3 Closed-Beta:

1. General Stability and Optimization Improvements.
2. Added Support for Mono based Games.
3. Added MUPOT Mode which forces IL2CPP games to run under Mono. This Mode is still extremely Experimental.
4. Logger has been renamed to MelonModLogger to prevent conflict with UnityEngine.Logger.
5. Fixed Crash issue when using System.Type in MUPOT Mode.
6. MelonModInfo Attribute has been changed to an Assembly Attribute.
7. Fixed issue with MelonModLogger not getting the Mod Name when used outside of the main class.
8. FileInfo in the TestMod has been renamed to BuildInfo so it won't conflict with the System.IO.FileInfo class.
9. Added 0Harmony Integration and Support.
10. Modified MelonLoader.ModHandler to use a Component based system in MUPOT Mode and Mono based games.
11. Fixed issue preventing OnUpdate from being called in MUPOT Mode and Mono based games.
12. Fixed issue preventing OnApplicationQuit from being called in MUPOT Mode and Mono based games.
13. Fixed Conflict issue triggering Unity's Single Instance function in MUPOT Mode.
14. Added OnLevelWasLoaded, OnLevelWasInitialized, OnUpdate, OnFixedUpdate, OnLateUpdate, and OnGUI for Mods in MUPOT Mode and Mono based games.
15. Bug that prevented Debug Compilation is now fixed.
16. Fixed issue that caused Unity Crash Handler to initialize MelonLoader twice.
17. Fixed Runtime Initialization issue with Audica that caused crashes.
18. Fixed Execution Order of MelonLoader.ModHandler.
19. Fixed issue with Mono Game Support due to Mono DLL naming.
20. Fixed issue with Internal Calls through MUPOT Mode.
21. Added better Mod Exception Handling and Logging.
22. Added Dependencies for Unity 2019.1.0b1.
23. Added Dependencies for Unity 2017.4.28f1.
24. Added Dependencies for Unity 2018.4.13f1.
25. Added Dependencies for Unity 2018.1.6f1.
26. Added Dependencies for Unity 2019.2.0f1.
27. Fixed Incompatibility issue with Unity Dependencies between x86 and x64.
28. Added Log in MelonLoader.ModHandler for Unity Version.
29. General Code Cleanup.
30. Fixed OnApplicationQuit not working without MUPOT Mode.
31. Fixed Mono Game Support for games that don't use MonoBleedingEdge.  (MelonLoader.ModHandler and Mods must be compiled with .NET 3.5 Framework)
32. Fixed Execution Order when using MUPOT Mode.

---

### v0.0.2 Closed-Beta:

1. Added NET_SDK to be built into MelonLoader.ModHandler as the designated IL2CPP Wrapper.
2. Fixed issue with Mods and Logs folders not being in the correct place next to the game's exe.
3. Fixed issue with the ModPrefs not calling the OnModSettingsApplied method of Mods.
4. Replaced old ModPrefs debug log message left in from my VRChat mod.
5. Fixed issue with Audica crashing when trying to use MelonLoader.
6. Added Unity 2018.4.6f1 Dependencies.
7. Fixed bug with Launch Options.
8. Fixed Pathing issue that caused games like Boneworks to crash.
9. Fixed issue where Environment.CurrentDirectory wasn't being set properly.
10. Clean-up and Additions made to NET_SDK.  (Thanks Sc4ad :D)
11. Added SteamVR and OculusVR Dependency DLLs to MelonLoader/Managed.
12. Added TestMod to Project Files.
13. Added Attribute StackTrace System to Logger.  (As requested by Camobiwon :3)
14. Replaced Spaces in Names for the Attribute StackTrace System with Underscores.
15. Fixed the bug that caused the Logger from MelonLoader.ModHandler to not properly output when using the Debug Console.
16. Added SystemTypeToIL2CPPClass method to NET_SDK.IL2CPP.
17. Crash Issue with Game.DontDestroyOnLoad has been fixed and it now works.
18. Fixed IL2CPP_Field where SetValue does not function on a instance object.  (Thanks DubyaDude ^.^)
19. Fixed Compile Version conflict with the 2 Consoles.  
MelonLoader [DEBUG] & MelonLoader.ModHandler [RELEASE]  =  Debug Console  
MelonLoader [RELEASE] & MelonLoader.ModHandler [DEBUG]  =  Normal Console  
MelonLoader [DEBUG] & MelonLoader.ModHandler [DEBUG]  =  Debug Console  
MelonLoader [RELEASE] & MelonLoader.ModHandler [RELEASE]  =  Left up to the Launch Options  

---

### v0.0.1 Closed-Beta:

1) Initial Commit and Rewrite.