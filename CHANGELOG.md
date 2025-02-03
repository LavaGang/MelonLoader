| Versions: |
| - |
| [v0.7.0](#v070) |
| [v0.6.6](#v066) |
| [v0.6.5](#v065) |
| [v0.6.4](#v064) |
| [v0.6.3](#v063) |
| [v0.6.2](#v062) |
| [v0.6.1](#v061) |
| [v0.6.0](#v060) |
| [v0.5.7](#v057) |
| [v0.5.5](#v055) |
| [v0.5.4](#v054) |
| [v0.5.3](#v053) | 
| [v0.5.2](#v052) |
| [v0.5.1](#v051) |
| [v0.5.0](#v050) |
| [v0.4.3](#v043) |
| [v0.4.2](#v042) |
| [v0.4.1](#v041) |
| [v0.4.0](#v040) |
| [v0.3.0](#v030) |
| [v0.2.7.4](#v0274) |
| [v0.2.7.3](#v0273) |
| [v0.2.7.2](#v0272) |
| [v0.2.7.1](#v0271) |
| [v0.2.7](#v027) |
| [v0.2.6](#v026) |
| [v0.2.5](#v025) |
| [v0.2.4](#v024) |
| [v0.2.3](#v023) |
| [v0.2.2](#v022) |
| [v0.2.1](#v021) |
| [v0.2.0](#v020) |
| [v0.1.0](#v010) |
| [v0.0.3](#v003) |
| [v0.0.2](#v002) |
| [v0.0.1](#v001) |

---

### v0.7.0

1. Updated Unity Dependencies Source to use the [New Automated Repository](https://github.com/LavaGang/MelonLoader.UnityDependencies)   (Credits to [slxdy](https://github.com/slxdy) :D)
2. Fixed an issue with MelonProxy not using Executable Path for Base Directory
3. Reworked Bootstrap and Proxy to use NAOT Compilation   (Credits to [slxdy](https://github.com/slxdy) :D)
4. Fixed an issue with the Il2CppAssemblyGenerator ignoring the --melonloader.agfregenerate launch option
5. Fixed an issue with Loading Plugins from MelonFolders that exist in the Mods folder too Early
6. Fixed an issue with Il2CppAssemblyGenerator using Incorrect Module Pathing to Load
7. Fixed an issue with Extended Folder Scanning not running without strict definition of Folder Names
8. Updated Cpp2IL to 2022.1.0-pre-release.19
9. Reimplemented Cpp2IL StrippedCodeRegSupport Module
10. Fixed an issue with .NET Bundle Extraction attempting to extract to a folder of the same name as the executable
11. Fixed an issue with Compatibility Layers not loading from Base Directory
12. Updated Il2CppInterop to 1.4.6-ci.585
13. Updated System.Configuration.ConfigurationManager, System.Drawing.Common, and System.Security.Permissions to 9.0.0
14. Revert de-duplication to fix decoding by type hint broken in TinyJSON   (Credits to [No3371](https://github.com/No3371) :D)
15. Implemented Loader Config system   (Credits to [slxdy](https://github.com/slxdy) :D)
16. Changed Console Encoding to UTF8   (Credits to [slxdy](https://github.com/slxdy) :D)
17. Fixed an issue with AsmResolver not being able to read files correctly   (Credits to [Atmudia](https://github.com/Atmudia) :D)
18. Made all Obsolete Members into Errors   (Credits to [slxdy](https://github.com/slxdy) :D)
19. Moved SharpZipLib to BackwardsCompatibility   (Credits to [slxdy](https://github.com/slxdy) :D)
20. Moved TinyJSON to BackwardsCompatibility   (Credits to [slxdy](https://github.com/slxdy) :D)
21. Improved Mono Library Initialization
22. Reworked Bootstrap Proxy Exports to allow Loading Original from Local Copy
23. Implemented Pre-Scan of Melon Folders to fix Load Order
24. Updated missing deps for NetStandardPatches   (Credits to [slxdy](https://github.com/slxdy) :D)

---

### v0.6.6

1. Updated Il2CppInterop to 1.4.6-ci.579
2. Reverted AssetTools.NET to 3.0.0-preview3
3. Implemented a RegisterTypeInIl2CppWithInterfaces attribute
4. Implemented Recursive Melon Folders with extended UserLib Resolving
5. Implemented Melon Preprocessor to prevent Loading Duplicates
6. Reimplemented NetFramework Variant of Cpp2IL as fallback
7. Standardized Assembly Searching and Resolving to work on both Mono and Il2Cpp Games
8. Temporarily removed Start Screen for being broken in most cases
9. Modified Command-Line Argument Logging to show Internal Arguments Only
10. Reworked Il2CppICallInjector to use Il2CppInterop's Native to Manage trampoline generation
11. Reworked Launch Option Parsing to ignore First Element
12. Added UserLibs folders to Native Library Search Directories
13. Moved `dobby.dll` to `MelonLoader\Dependencies`
14. Moved Assembly Resolver Related Classes to `MelonLoader.Resolver` Namespace
15. Moved `MonoLibrary` class to `MelonLoader.Utils` Namespace
16. Moved dobby scan to check Game Directory after Base Directory
17. Removed Useless TODO Warning from Il2CppAssemblyGenerator
18. Removed EOS Compatibility Layer for being Unneeded
19. Fixed Regression with LemonMD5, LemonSHA256, and LemonSHA512
20. Fixed an issue with older Cpp2IL versions causing a download failure
21. Fixed an issue with Il2CppInterop not properly logging Trampoline Exceptions
22. Fixed an issue with Il2Cpp Class Injection Attributes causing exceptions to be thrown on Mono games
23. Fixed an issue with the Bootstrap not reading `--melonloader.basedir` correctly
24. Fixed an issue with Loading `dobby.dll` in some rare cases
25. Fixed an issue with Compatibility Layers getting Garbage Collected while still in use
26. Fixed an issue with Linux Proxy failing to find dobby
27. Fixed an issue with trying to load Managed Assemblies from Native Libraries inside UserLibs
28. Fixed an issue with Compatibility Layer loading not validating Names Checked
29. Fixed an issue with Type Load spamming errors when failing to resolve a Dependency
30. Fixed an issue with Launch Option Parsing ignoring Argument Values
31. Fixed a race condition crash with Multi-Launching a Game and the Logger trying to delete old files

---

### v0.6.5

1. Updated Il2CppInterop to 1.4.6-ci.545
2. Updated Cpp2IL to 2022.1.0-pre-release.18
3. Updated AsmResolver to 6.0.0-beta.1
4. Updated AssetRipper.VersionUtilities to 1.5.0
5. Updated AssetsTools.NET to 3.0.0
6. Updated UnityEngine.Il2CppAssetBundleManager for latest compatibility
7. Updated UnityEngine.Il2CppImageConversionManager for latest compatibility
8. Implemented `--cpp2il.callanalyzer` launch option to enable Cpp2IL's CallAnalyzer processor
9. Implemented `--cpp2il.nativemethoddetector` launch option to enable Cpp2IL's NativeMethodDetector processor
10. Implemented several fixes for Il2CppInterop related issues
11. Implemented `ExternalArguments` Dictionary for MelonLaunchOptions   (Credits to [HAHOOS](https://github.com/HAHOOS) :P)
12. Implemented `MsgPastel` Method for MelonLogger   (Credits to [HAHOOS](https://github.com/HAHOOS) :P)
13. Implemented `Peek` Method for LemonEnumerator
14. Implemented ICallInjector for handling when Unity strips or renames Internal Calls
15. Fixed an accidental regression with LemonSHA256
16. Fixed an issue with Native logs using the wrong Colors
17. Fixed an issue with Preload module not replacing Mono libraries on Older Mono Games
18. Fixed an issue with MonoMod DetourContext Disposal not working properly
19. Fixed an issue with Debugger Launch Option causing crashes
20. Fixed an issue with Console not having the Game Name and Version in the title
21. Fixed an issue with Sharing Violation during Log Initialization
22. Fixed an issue with `--melonloader.basedir` launch option always expecting an `=` sign before the path
23. Fixed an issue with Il2CppInteropFixes not being properly error handled
24. Fixed an issue with Il2CppInteropFixes using `il2cpp_type_get_class_or_element_class` instead of `il2cpp_class_from_type`
25. Fixed an issue with EOS Support Module not being properly error handled
26. Fixed an issue with NativeStackWalk not unregistering addresses
27. Fixed an issue with Errors being Spammed if `UnityEngine.Transform::SetAsLastSibling` fails to resolve
28. Fixed an issue with MelonLaunchOptions failing to parse options if a `-` prefix is used instead of `--`
29. Fixed an issue with MelonLaunchOptions failing to parse option arguments if an `=` sign is used instead of a space
30. Fixed an issue with Command Line Arguments not being logged
31. Fixed an issue with Il2CppInterop Assembly Resolving when Il2Cpp Prefixing is used
32. Fixed an issue with .NET Executables not being Resolved properly when used as Dependencies
33. Fixed an issue with Dependency Graph failing to Resolve Assemblies properly
34. Fixed an issue with Il2Cpp Support Module not attempting to use direct references

---

### v0.6.4

1. Removed Analytics Blocker for causing crashes, will be ported to its own Plugin
2. Updated Cpp2IL to 2022.1.0-pre-release.15

---

### v0.6.3

1. Updated NuGet Packages
2. Updated Cpp2IL to 2022.1.0-pre-release.14
3. Updated Il2CppInterop to 1.4.6-ci.433
4. Updated Tomlet to 5.3.1
5. Fixed Referenced DLLs not being resolved properly resulting in crashes
6. Fixed Proxy being unable to find Original DLL    (Credits to [RinLovesYou](https://github.com/RinLovesYou) :3)
7. Fixed LoadLibrary failing on in rare cases    (Credits to [RinLovesYou](https://github.com/RinLovesYou) :3)
8. Fixed Preload Module failing to initialize included resources   (Credits to [TrevTV](https://github.com/TrevTV) :P)
9. Reimplemented Log Caching and Logs folder
10. Implemented Additional Initialization Error Handling
11. Implemented Garbage Collection handling for NativeHook
12. Rewrote the Demeo Compatibility Layer to be less prone to breakage
13. Fixed DarkRed ConsoleColor   (Credits to [Scoolnik](https://github.com/Scoolnik) :D)
14. Fixed several .NET Framework 2.0 compatibility issues   (Credits to [slxdy](https://github.com/slxdy) :D)
15. Fixed an issue with Mods loading when Il2Cpp Assembly Generation fails
16. Added `--melonloader.sab` Launch Option to disable the Analytics Blocker
17. Fixed an issue with MelonCompatibilityLayer causing crashes on some games

---

### v0.6.2

1. Added a compatibility layer for EOS (Epic Online Services), preventing a crash caused by the Overlay
2. Updated Cpp2IL
3. Updated Tomlet
5. Updated `MelonLoader.NativeUtils.NativeHook` to prevent GC issues 
6. Updated Il2CppAssetBundleManager for Il2CppInterop
7. Fixed Proxy being unable to find System32 when Windows is not installed in C:
8. Fixed logger sha256 hash (Credits to [Windows10CE](https://github.com/Windows10CE))
9. Fixed DAB Thread safety & Oculus Profile Pictures (Credits to [SirCoolness](https://github.com/SirCoolness))
10. Fix Melon Load Order (Credits to [Loukylor](https://github.com/loukylor))
11. General fixes for Proxy & Bootstrap

---

### v0.6.1

1. Refactored Bootstrap, more informative errors
2. Updated classdata.tpk, fixing some issues when reading game versions
3. Deprecated NativeHooks, giving a new utility class `MelonLoader.NativeUtils.NativeHook<T> where T : Delegate`
4. Updated MonoMod, fixing some issues with old Mono Games
5. Fix some compatibility issues for certain Mono Games
6. Implemented some missing launch options

---

### v0.6.0:

1. Added Linux Support   (Credits to [RinLovesYou](https://github.com/RinLovesYou) :3)
2. Switched the runtime to .NET 6 CoreCLR for Il2Cpp Games (Credits to [SamboyCoding](https://github.com/SamboyCoding) :D)
3. Moved a lot of logic from Native to Managed   (Credits to [SamboyCoding](https://github.com/SamboyCoding) & [RinLovesYou](https://github.com/RinLovesYou) :D)
4. Replaced Unhollower with Il2CppInterop   (Credits to [SamboyCoding](https://github.com/SamboyCoding), [nitrog0d](https://github.com/nitrog0d) & [ds5678](https://github.com/ds5678) :D)
5. Rewrote Proxy & Bootstrap   (Credits to [RinLovesYou](https://github.com/RinLovesYou) :3)
6. Fixed some start screen corruption
7. Added helper methods for determining current platform
8. added `Utils.MelonConsole` for printing to console on mono games
9. Removed Il2CppDumper
10. Update HarmonyX, MonoMod, and Tomlet
11. Switched from MSDetours to Dobby
12. Added full RGB color support in console using Pastel
13. Added `MelonEnvironment` class

---

### v0.5.7:

1. Fixed the MelonEvent registration sorting.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
2. Inverted the MelonEvent priority sorting system to match the MelonPriorityAttribute.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
3. Updated Start Screen Signatures.     (Credits to [Slaynash](https://github.com/Slaynash) :3)
4. Fixed Start Screen Issues with UnityInternals.runtime_class_init.
5. Updated Class Data TPK.     (Credits to [ds5678](https://github.com/ds5678) :D)
6. Updated [Cpp2IL](https://github.com/SamboyCoding/Cpp2IL) to v2022.1.0-pre-release.8.
7. Added Launch Option Logging.
8. Added new MelonInfo constructors.
9. Setup Compatibility Layer for BONEWORKS and BONELAB.
10. Made Game Specific Compatibility Layers load agnostically.
11. Fixed race condition issue with multiple concurrent scene loads.
12. Removed BONEWORKS_OnLoadingScreen MelonMod override and MelonEvent.
13. Added Auto-Resolver for OnLoadingScreen MelonMod methods.
14. Rewrote ServerCertificateValidation Fix to use Reflection.
15. Separated Melon Harmony Initialization from OnApplicationStart events.
16. Fixed issue with MelonInfo Semver Constructors being inaccessable.
17. Added LemonTuple Constructors.

---

### v0.5.5:

1. Rewrote Proxy.   (Credits to [autumncpp](https://github.com/autumncpp) :D)
2. Updated [AssetsTools.NET](https://github.com/nesrak1/AssetsTools.NET) to v3.0.0-preview1.
3. Updated [AssetRipper.VersionUtilities](https://github.com/AssetRipper/VersionUtilities) to v1.2.1.
4. Removed Game Blacklist.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
5. Rewritten the Melons loader.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
6. Added MelonEvents.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
7. Added MelonModules.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
8. Gave all MelonModules their own logger instances.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
9. Added MelonAssemblies.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
10. Fixed MelonLoader's deinitialization time.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
11. Implemented warning for Melons which don't use a semver-style version.    (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
12. Fixed Crash Issue with Demeo Compatibility Layer.
13. Fixed Issue with VR Mode on Demeo.
14. Fixed Issues with Demeo CL not refreshing Melon Listings.
15. Fixed MME Issue with MelonBase.
16. Updated [Cpp2IL](https://github.com/SamboyCoding/Cpp2IL) to v2022.1.0-pre-release.7.
17. Backported StackFrame::GetMethod Fix.     (Credits to [Samboy](https://github.com/SamboyCoding) :3)
18. Added a singleton class for Melons (Melon<T>)     (Credits to [SlidyDev](https://github.com/SlidyDev) :])
19. Fixed Issue with Unity Game Check in Proxy.
20. Removed ``--melonloader.agfvunity`` Launch Option.
21. Added ``--melonloader.unityversion`` Launch Option.
22. Added Backup Unity Version Parsing Method.
23. Fixed Issue with Reading Launch Options.
24. Temporarily disabled mono_debug_init Call for Mono Games to fix Crash Issue.
25. Removed Useless DLL Name Parse.
26. Downgraded HarmonyX and MonoMod.RuntimeDetour to fix NRE issue with MonoMod's ReflectionHelper.
27. Added a simpler unsub method for melon events.     (Credits to [Samboy](https://github.com/SamboyCoding) :3)
28. Replaced Assembly Generator User-Agent with Build Version.
29. Cleaned the melon info log format.     (Credits to [SlidyDev](https://github.com/SlidyDev) :])
30. Implemented Fallback Unity Version Parsing.     (Credits to [Samboy](https://github.com/SamboyCoding) and [ds5678](https://github.com/ds5678) :D)
31. Swapped out the official bHaptics library with the open-source bHapticsLib.
32. Fixed MRE Issue with MelonLoader.Core referencing the Obsolete bHaptics class.
33. Fixed Regex Compatibility Issue with Older Runtimes.
34. Added additional null checking for Start Screen's GfxDevice.
35. Fixed Start Screen issue with Signature Scanning GfxDevice on Unity 2020.3.15 x86 Mono.
36. Updated [Tomlet](https://github.com/SamboyCoding/Tomlet) to v5.0.0.
37. Implemented Fallback Game Information Parsing.
38. Fixed issue with manually setting Unity version via Launch Option causing it to not attempt to read Game Name, Game Developer, or Game Version.

---

### v0.5.4:

1. Modified VerifyLoaderVersion Attribute to bring it in line with SemVer spec.
2. Removed File Watchers from Start Screen Config.
3. Removed Manual Parsing for Game Information.
4. Implemented [AssetsTools.NET](https://github.com/nesrak1/AssetsTools.NET).
5. Rewrote Unity Information Handling.
6. Fixed Issue with Exception Logging on Older Mono versions.
7. Improved UnityInformationHandler Exception Handling.
8. Fixed Issue with Il2CppAssemblyUnhollower Initialization.
9. Fixed Issue with Cpp2IL Package Cleanup.
10. Fixed Comparison Issue with Semver.
11. Fixed Issue with Start Screen Background and Logo not Animating Properly.
12. Added Start Screen Element Position Customization.
13. Fixed Issue with Start Screen Progress Bar Positioning.
14. Added Start Screen Version Text Customization.
15. Added Start Screen Version Text Anchor Customization.
16. Added Start Screen Version Text Style, RichText, FontSize, Scale, and LineSpacing Customization.
17. Demeo_LobbyRequired has been renamed to Demeo_LobbyRequirement.
18. Cleaned up Start Screen Config.
19. Rewrote Start Screen UI Element Backend.
20. Updated AssetsTools.NET to v2.0.11.
21. Fixed Issue with Melon Author Coloring.
22. Added Theme Folders and Theme Selection to Start Screen Customization.
23. Fixed Issue with Custom Start Screen Background Image rendering upside-down.
24. Fixed Issue with Custom Start Screen Image Scanning not checking the Theme's Folder.
25. Fixed Issue with TomlEnumParseException during MelonPreferences load causing Crashing.
26. Added Default Fallback to Start Screen Customization.
27. Added Old Category Purging to Start Screen Customization.
28. Fixed Issue with Custom Start Screen Image Scanning not taking into account File Extension.
29. Added Optional Random Theme Selection to Start Screen.
30. Fixed Issue with OnPreferencesSaved and OnPreferencesLoaded being called Incorrectly.
31. Changed "No Compatibility Layer Found" Message from Error to Warning.   (Credits to [ds5678](https://github.com/ds5678) :D)
32. Implemented [AssetRipper.VersionUtilities](https://github.com/AssetRipper/VersionUtilities)   (Credits to [ds5678](https://github.com/ds5678) :D)
33. Fixed Demeo Incompatibility Issues with Compatibility Layer and PC Edition.
34. Combined Demeo Compatibility Layers into 1 Singular Dll.
35. Fixed TLS Bridging Failure on Unity 2021.2.7 x64 Il2Cpp.
36. Updated to Il2CppAssemblyUnhollower v0.4.18.0.
37. Improved Signature Scanning for Il2CppUnityTls Compatibility Layer.
38. Fixed Mono TLS Provider Registration.
39. Updated AssetRipper.VersionUtilities to v1.2.0.   (Credits to [ds5678](https://github.com/ds5678) :D)
40. Implemented TLS Certificate Validation Fix.
41. Fixed Downloading Issue with Deobfuscation Maps.
42. Fixed Crash Issue with Start Screen attempting to instantiate stripped classes.     (Credits to [Samboy](https://github.com/SamboyCoding) :3)
43. Fixed Crash Issue with Il2CppUnityTls Compatibility Layer.     (Credits to [Samboy](https://github.com/SamboyCoding) :3)
44. Fixed Hashing Case Issue with Deobfuscation Maps.
45. Removed ILRepack.   (Credits to [ds5678](https://github.com/ds5678) :D)
46. Added Forwarding Attributes.   (Credits to [ds5678](https://github.com/ds5678) :D)
47. Moved Il2Cpp Harmony Patcher to Il2Cpp Support Module.
48. Fixed Issue with Solution Build Dependencies copying to Output Directory.
49. Fixed Issue with Il2Cpp Harmony Patcher breaking on Custom Il2Cpp Types.   (Credits to [ds5678](https://github.com/ds5678) :D)
50. Implemented Management and Disposal Handling for Start Screen UI Elements.
51. Used a different namespace for UnityEngine classes in Start Screen wrapper.   (Credits to [Sinai](https://github.com/sinai-dev) :D)
52. Removed runtime_class_init Call from MelonUnityEngine.UnityObject to fix Crashing Issue.
53. Updated Cpp2IL to v2022.1.0-pre-release.3.     (Credits to [Samboy](https://github.com/SamboyCoding) :3)
54. Added Version Check to Cpp2IL Package Usage.

---

### v0.5.3:

1. Updated HarmonyX to v2.8.0.
2. Updated Tomlet to v3.1.3.
3. Updated SharpZipLib to v1.3.3.
4. Updated Il2CppAssemblyUnhollower to v0.4.17.1.
5. Updated Newtonsoft.Json to v13.0.1.
6. Updated Cpp2IL to v2022.0.0.
7. Removed Analytics Blocker's modification of hostname if NULL.   (Credits to [SirCoolness](https://github.com/SirCoolness) :D)  
8. Fixed Connection Issue on Demeo.
9. Added Demeo_LobbyRequired Attribute to Compatibility Layer to enforce all lobby members on Demeo to have a specific melon and version.
10. Implemented mgGif for Start Screen Image Parsing.   (Credits to [TrevTV](https://github.com/TrevTV) :P)
11. Reimplemented Image Frame Parsing for Start Screen.   (Credits to [TrevTV](https://github.com/TrevTV) :P)
12. Moved EnumExtensions from MelonStartScreen Assembly to Main MelonLoader Assembly.
13. Removed ZIP Loading Functionality for Plugins and Mods folders.
14. Fixed Issue with FileHandler Blacklist.
15. Added Optional MelonID Attribute for Melons.
16. Updated Logger to Print MelonID when set.
17. Removed Obsolete Attributes from MelonLogger static methods.
18. Internal Failures now print their proper Failure Messages in the MessageBox even if not in Debug Mode.
19. Cleaned Up Start Screen's Animated Image Customization.
20. Added Start Screen Color Customization for Background, Progress Bar, and Progress Bar Outline.
21. Added Start Screen Text Color Customization.
22. Fixed Issue with Colors Reading and Writing to Start Screen Config Incorrectly.
23. Implemented Element Toggles for Start Screen.
24. Added LemonArraySegment because System.ArraySegment doesn't have interfaces on older .NET Framework versions.
25. Fixed Issue with SharpZipLib's InflaterHuffmanTree not properly Segmenting Array.
26. Refactored Start Screen Image Handling.
27. Added Start Screen Logo Customization.
28. Added Start Screen Background Image Customization.
29. Proxy will now terminate any Non-Unity Process that load it without warning. Example of this is UnityCrashHandler.
30. Fixed Issue with Custom Start Screen Background Image being Inverted.
31. Added JPG/JPEG Support to Start Screen Image Customization.
32. Replaced Start Screen's Default Loading GIF.   (Credits to [gompocp](https://github.com/gompocp) :D)
33. Combined Mono Support Modules into 1 Singular Support Module.
34. Fixed Start Screen Issue with Image Filtering.
35. Added Start Screen Image Filtering Customization.
36. MelonPreferences_Category DisplayName and IsHidden is now Modifiable during Runtime.
37. MelonPreferences_Entry DisplayName, Description, IsHidden, and DontSaveDefault are now Modifiable during Runtime.
38. Added MelonPreferences_Category.IsInlined for inlining the TomlTable of the Category.
39. MelonPreferences_Entry.Description now Writes to File as a Comment.
40. Changed MelonPreferences_Entry.Description to Preceding Comment.
41. Added MelonPreferences_Entry.Comment for Inline Comment.
42. Fixed Issue with Apostrophes in File Pathing.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)
43. Added Methods for deleting and renaming Entries in MelonPreferences.   (Credits to [DragonPlayerX](https://github.com/DragonPlayerX) :D)
44. Removed Newtonsoft.Json from Il2CppAssemblyUnhollower Blacklist.
45. Moved OnApplicationStart Initialization to Component Start.
46. MelonPreferences files will now Auto-Reset if TomlUnescapedUnicodeControlCharException is thrown when the file is corrupt.
47. Fixed Assembly Verification Issue with MonoMod DMD Dumping.
48. Added OnPreferencesSaved string variant that gets passed the Preferences's File Path.
49. Added OnPreferencesLoaded string variant that gets passed the Preferences's File Path.
50. Fixed MissingMethodException Issue with MelonLoader.MelonPreferences_Category.CreateEntry.
51. Added Il2CppUnityTls CL for Managed-sided Il2Cpp Unity TLS Bridging.
52. Fixed Issue with OnSceneWasLoaded not being called for First Scene Load.
53. [StartScreen] ICall <2018.1 for UE.SystemInfo::GetGraphicsDeviceType.     (Credits to [Slaynash](https://github.com/Slaynash) :3)
54. Added Additional Exception Catching to Il2CppUnityTls CL.
55. Moved InstallUnityTlsInterface Signature Scanning from Bootstrap to Il2CppUnityTls CL.
56. Fixed MissingMethodException Issue with MelonUtils.SetCurrentDomainBaseDirectory.
57. Added --melonloader.agfregex Launch Option to Force Regex used for Assembly Generation.
58. Added LemonSHA256 because System.Security.Cryptography.SHA256 can be stripped.
59. Fixed MissingMethodException Issue with MelonHandler.sha256.
60. Fixed MissingMethodException Issue with MonoResolveManager.AddSearchDirectory.
61. Added LemonSHA512 because System.Security.Cryptography.SHA512 can be stripped.
62. Added LemonMD5 because System.Security.Cryptography.MD5 can be stripped.
63. Fixed Issue with Assembly Generator where Deobfuscation Regex changes wouldn't induce Regeneration.
64. Rewrote Package System in Assembly Generator.
65. Fixed Issue with Main Window not having it's Close Button Disabled during Assembly Generation.
66. Added OnPreSupportModule override for Plugins and Mods.
67. Fixed Issue with Initialization of Il2Cpp Harmony Patcher.
68. Temporarily Disabled Start Screen on Unity Versions lower than 2018.
69. Improved Exception Catching on Start Screen.
70. Added '{' and '}' to Assembly Verifier whitelist.
71. Added Melon Author Coloring and MelonAuthorColor Attribute.   (Credits to [adamdev](https://github.com/adamd3v) :D)
72. Fixed Issue with Melon Author Coloring.
73. Fixed Regeneration Issue with Assembly Generator.
74. Temporarily Disabled Start Screen on Unity Versions higher than or equal to 2020.3.22.
75. Fixed Issue with Assembly Generator not Re-Enabling the Close Button on the Game Window.

---

### v0.5.2:

1. Fixed Issue with --melonloader.basedir Launch Option not finding Bootstrap.dll.
2. Fixed Issue with Console not properly Coloring Melon Names.   (Credits to [benaclejames](https://github.com/benaclejames) :D)  

---

### v0.5.1:

1. Updated Il2CppAssemblyUnhollower to v0.4.16.2.
2. Added '(' and ')' Character to Assembly Verification.
3. Fixed Issue with Force Launch Options not checking Argument Count.
4. Fixed Issue with Proxy not properly detecting UnityCrashHandler.
5. Fixed Issue with Proxy not properly locating Bootstrap.
6. Fixed Issue with Offline Mode throwing an NRE during Assembly Generation.
7. Fixed Issue with Launch Options causing Crash.
8. [StartScreen] Fix resize crash on DX11/DX12.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
9. Fixed Issue with Older OS Check running under Wine/Proton.
10. [StartScreen] Resize fix x86.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
11. [StartScreen] Fixed crash on mono.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
12. Rewrote MelonLaunchOptions Loading.
13. Fixed Issue with Console not displaying Properly under Wine/Proton.
14. Fixed Issue with VerifyLoaderVersion Attribute missing methods.

---

### v0.5.0:

1. Updated HarmonyX to v2.6.1.
2. Updated Tomlet to v2.1.0.	
3. Updated Il2CppAssemblyUnhollower to v0.4.16.1.
4. Updated Cpp2IL to v2021.5.3.
5. Added User Libs Directory and Assembly Resolver.
6. Replaced StackTrace System in MelonLogger with Instance Based System.
7 Fixed Issue with Il2Cpp Harmony Method Patcher throwing Debug Warnings multiple times for the same Method.
8. Implemented Temporary Workaround for Cpp2IL Failing under Wine or Steam Proton.
9. Fixed Missing Method Issue caused by LemonAction.
10. Implemented MonoResolveManager and Assembly Resolving Management System.
11. Removed symbolsdata null check.   (Credits to [AuM0b](https://github.com/AuM0b) :D)  
12. Implemented Directory Priority Scanning for MonoResolveManager.
13. Implemented SteamManifestReader.
14. Fixed Issue with Bhaptics API and Steam Version of Bhaptics Player.
15. Added an error message for paths with non-ASCII characters.   (Credits to [SlidyDev](https://github.com/SlidyDev) :D)  
16. File Pathing is now printed to Log by Default to help aid in Debugging.
17. Implemented Non-ASCII Character Check in Proxy.
18. Fixed Issue with ASCII Character Check preventing Bootstrap from Loading.
19. Implemented Automated Cleanup System for Support Modules.
20. Added MelonLogger Error Overloads for Exceptions.   (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
21. Implemented Start Screen.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
22. Added "--melonloader.disablestartscreen" Launch Option.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
23. Added Unity Game Check to Proxy.
24. Fixed Issue with MonoResolveManager on x86 Platform.
25. Fixed Issue with Proxy's Unity Game Check on Older Unity Versions.
26. Made Failure to Properly Load MelonStartScreen less severe of an Error.
27. Fixed Type Locking Issue with MelonPreferences_ReflectiveCategory.
28. Fixed Issue with MelonLoader reading the wrong Unity Version from EXEs with Custom Info.
29. Removed base_path DirectoryExists Check in Mono::CheckLibName.     (Credits to [Samboy](https://github.com/SamboyCoding) :3)
30. Fixed Issue with new Cpp2IL Download.
31. Moved Obfuscation Mapping Download to Il2CppAssemblyGenerator folder.
32. Added '@' Character to Assembly Verification.
33. Fixed Issue with the Start Screen using stripped Func instead of LemonFunc.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
34. Fixed Issue with the Start Screen not functioning under Mono Games.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
35. Improved Start Screen Compatibility.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
36. Changed UnityEngine.Mesh Implementation in Start Screen to Internal Calls.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
37. Fixed Issue with the Start Screen's UIVertex Implementation.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
40. Fixed Crash Issue with Mesh::SetArrayForChannelImpl.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
41. Reimplemented Image Frame Parsing for Start Screen.
42. Custom Start Screen Image File Name is now Case Insensitive.
43. Improved Frame Size Handling when Parsing Custom Start Screen Image.
44. Improved Manual FrameBuffer Input for Image Frame Parsing.
45. Changed UnityEngine.Material Implementation in Start Screen to Internal Calls.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
46. Fixed Issue with Mono TLS Bridge running on Mono Games.
47. Fixed Issue with Proxy not properly detecting Unity Games.
48. Fixed Issue with Mono TLS Bridge not failing Gracefully.
49. Fixed Issue with Start Screen Support of Unity 2020.2.0+.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
50. Implemented Automatic Aspect Ratio Sizing for Custom Start Screen Image.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
51. Il2Cpp Assembly Generation issues now elevated to Internal Failure.
52. [StartScreen] Removed UE.GL class initializer.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
53. [StartScreen] Put the NSR in a try/catch.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
54. [StartScreen] Mono + Mono x64 fixes.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
55. [StartScreen] Fix UE.Graphics icall fallback.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
56. [StartScreen] Fix text rendering on Mono.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
57. Fixed Resolving Issue with MonoResolveManager.
58. Switched Coroutine System for Il2Cpp to a Wrapper based Interface.   (Credits to [HookedBehemoth](https://github.com/HookedBehemoth) :D)  
59. Fixed occasional double-logging issue with MelonLogger.   (Credits to [benaclejames](https://github.com/benaclejames) :D)  
60. Removed Useless Logger Instance Constructor.
61. Added more Comprehensive Exception Message when attempting to use an Instance Patch Method.
62. Fixed Conflict Issue with Debug Mode.
63. Added "--melonloader.disableunityclc" Launch Option to toggle Unity Console Log Cleaner.
64. Fixed Issue with Process.MainWindowHandle and Process.MainWindowTitle returning null.
65. Improved Assembly Verifier to make it less angry about small assemblies.   (Credits to [knah](https://github.com/knah) :D)
66. Use ReferenceEquals to compare il2cpp delegates as op_equality is not always present.   (Credits to [knah](https://github.com/knah) :D)
67. Fixed Issue with Animated Image on Start Screen using Incorrect Sizing.
68. [StartScreen] <U2018.3 and Mono x64 fixes.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
69. Added MelonLoader.Assertions.LemonAssert Utility for Universal Assertions Functionality.
70. Implemented Map Based Backend for Assertions under MelonLoader.Assertions.LemonAssertMapping.
71. Fixed Issue with LemonAssert.IsNull Mapping.
72. Implemented SemVer Library.
73. Fixed Comparison Issue with VerifyLoaderVersion Attribute.

---

### v0.4.3:

1. Fixed Issue with Mono Library Scanning.
2. Updated HarmonyX to v2.5.1.
3. Updated Cpp2IL to v2021.2.4.
4. Fixed Issue with Assembly Generator not responding to RemoteAPI when Forcing Dumper Version.

---

### v0.4.2:

1. Fixed Issue with Internal Calls being added twice.
2. Fixed Issue with Compatibility Layer System not running Constructors.
3. Fixed Issue with Compatibility Layer System having the RefreshMods event ran for Plugins.
4. Fixed Issue with Compatibility Layer System's Assembly to Resolver conversion check failing.
5. Updated Cpp2IL to v2021.1.2.
6. Updated Il2CppAssemblyUnhollower to v0.4.15.4.
7. Fixed Dependency Issue with [MuseDashModLoader](https://github.com/mo10/MuseDashModLoader) Compatibility Layer.   (Credits to [gompocp](https://github.com/gompocp) :D)
8. Fixed Issue with --melonloader.consolemode Launch Option not being properly capped.
9. Added Melon Name Sorting for Printout Log.   (Credits to [ds5678](https://github.com/ds5678) :D)  
10. Updated Il2CppDumper to v6.6.5.
11. Fixed Issue with Demeo Integration not showing Plugins.
12. Added MelonProcess Attribute.
13. Added MelonGameVersion Attribute for Mods.
14. Fixed Issue with Core PreStart and OnApplicationEarlyStart not getting called on Mono Games.
15. Added --melonloader.basedir Launch Option.

---

### v0.4.1:

1. Fixed Issue with Compatibility Layer system throwing NRE about Default Constructors.
2. Added MelonCompatibilityLayer.WrapperData Extensions.
3. Fixed Case Issue with MelonHandler not handling File Extensions properly.
4. Fixed Case Issue with MelonHandler not handling File Extensions inside ZIPs properly.
5. Fixed Issue with the Close Button of the Console still being Active during Assembly Generation.
6. Fixed Issue with MelonHandler not calling OnModSettingsApplied on Legacy Melons.
7. Fixed Issue with DAB not properly caching already found Hosts.
8. Fixed NRE Issue in Il2CppAssemblyGenerator.RemoteAPI.ContactHosts.   (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
9. Updated Tomlet to v1.3.5.   (Credits to [loukylor](https://github.com/loukylor) and [Samboy](https://github.com/SamboyCoding) :3)
10. Added [MuseDashModLoader](https://github.com/mo10/MuseDashModLoader) Compatibility Layer.   (Credits to [gompocp](https://github.com/gompocp) :D)
11. Fixed Issue with MelonLogger causing Crash on null Input Text.   (Credits to [loukylor](https://github.com/loukylor) :3)
12. Fixed Issue with MelonLogger not allowing Empty Input Text.   (Credits to [ds5678](https://github.com/ds5678) :D)
13. Updated Il2CppDumper to v6.6.3.
15. Swapped out Il2CppDumper for Cpp2IL.
16. Added "--melonloader.agfoffline" Launch Option to Force the Assembly Generator to Run without Contacting the Remote API.
17. Fixed Issue with MelonLogger not abiding by Warning and Error Caps.
18. Fixed Issue with Assembly Generator using the wrong Download URL for Cpp2IL.
19. Updated Cpp2IL to v2021.1.   (Credits to [ardittristan](https://github.com/ardittristan) and [Samboy](https://github.com/SamboyCoding) :3)
20. Fixed Issue with MelonGameAttribute not setting Name after Creation.
21. Converted Compatibility Layer System to be Module Based.
22. Fixed Issue with Compatibility Layer System running Module Setup twice.
23. Implemented Demeo Integration.
24. Fixed String Marshalling Issue with bHaptics Native Library.
25. Update Il2CppAssemblyUnhollower to v0.4.15.3.
26. Fixed Issue with Compatibility Layer System incorrectly casting Delegates.   (Credits to [ds5678](https://github.com/ds5678) :D)
27. Fixed Issue with Log Coloration from Cpp2IL.   (Credits to [Samboy](https://github.com/SamboyCoding) :3)
28. Improved Analytics Blocker.
29. Added "psapi.dll" Support to Proxy.
30. Fixed Encoding Issue with MelonLogger.   (Credits to [lolligun](https://github.com/lolligun) :D)
31. Cleaned Up Logger Implementation.   (Credits to [benaclejames](https://github.com/benaclejames) :D)
32. Fixed Issue with Console not properly using Non-Ansi Manual Coloring.   (Credits to [benaclejames](https://github.com/benaclejames) :D)
33. Fixed Issue with Default Melon Constructors not being run.

---

### v0.4.0:

1. Tweaked Assembly Load exceptions logging.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
2. Fixed Assembly Load logging typo.   (Credits to [Slaynash](https://github.com/Slaynash) :3)
3. Added MelonUtils.HashCode.
4. Fixed Issue with UnityCrashHandler.
5. Cleaned up Console Title Renaming.
6. Added Game Name to Console Title.
7. Added Game Version to Console Title.
8. Changed Debug Mode Indicator in Console Title to "[D]".
9. Updated bHaptics Native Library to v1.5.3.
10. Fixed Issue with Console Always On Top Launch Options causing Warnings to be Hidden.
11 Fixed Issue with Force Version Launch Options being overwritten.
12. Fixed Issue with Force Version Launch Options counting "0.0.0.0" as a valid version.
13. Added "--melonloader.consoledst" Launch Option. When Used it will set the Console Title as the Game Name.
14. Added Secret Lemon.
15. Added Game Version to Logs.
16. Fixed Collision Issue with Hash Code Generation.
17. Improved Hash Code Generation.
18. Improved SamboyAPI Implementation in Assembly Generator.
19. Added Wine Detection to Operating System Check.
20. Converted Assembly Generator to using Internal Calls instead of Native Exports.
21. Removed Native Exports from Bootstrap.
22. Fixed Issue where Debug Mode's "[D]" would print to log.
23. Fixed Issue with SamboyAPI not parsing correctly.
24. Added Among Us Deobfuscation Mapping.
25. Fixed Issue with Game Version returning 0.
26. Fixed Issue with Deobfuscation not abiding by Config's Obfuscation Regex as a fallback.
27. Fixed Issue with bHaptics Library causing "Unable to Connect" to be spammed in Console.
28. Added Color32 Support to MelonPreferences.
29. Internal Failures now Kill the Process when thrown.
30. Fixed Issue with Analytics Blocker incorrectly blocking certain hosts.
31. Added OnApplicationLateStart Melon Override.
32. Added Missing Attribute Cleanup Fix from Installer.
33. Improved Proxy.
34. Added dont_save_default Option to MelonPreferences.   (Credits to [YOWChap](https://github.com/yowchap) :D)
35. Added Backwards Compatibility for MelonPreferences.   (Credits to [knah](https://github.com/knah) :D)
36. Fixed the return type of the MelonPreferences API.   (Credits to [knah](https://github.com/knah) :D)
37. Fixed Issue with HostName Check in Analytics Blocker.
38. Fixed Issue with HarmonyInstance.Unpatch not doing a proper null check.
39. Proxy will now have an Error Popup when it has an Invalid File Name.
40. Updated Il2CppDumper to v6.5.0.
41. Added Object Oriented File System for MelonPreferences.
42. Added MelonPreferences_Category.SetFilePath.
43. Added MelonPreferences_Category.ResetFilePath.
44. Fixed Issue with Core Initialization.
45. Fixed Conflict Issue with Multiple MelonPreferences Categories using the same file.
46. Fixed Issue with File Watcher System and Separated MelonPreferences Files.
47. Fixed Issue with OnPreferencesLoaded not being called on Creation of a Separated MelonPreferences File.
48. Moved GetMelonFromStackTrace to MelonUtils.
49. Made GetMelonFromStackTrace Accessible.
50. Fixed Encoding Issue with Mono API.   (Credits to [constfold](https://github.com/constfold) :3)
51. Fixed Issue with LoadLibrary failing when a DLL file path contains Non-ASCII Characters.   (Credits to [constfold](https://github.com/constfold) :3)
52. Fixed Issue with Unicode Command Line.   (Credits to [constfold](https://github.com/constfold) :3)
53. Removed MelonBase.Compatibility.
54. Added MelonPlatform Attribute.
55. Added MelonPlatformDomain Attribute.
56. Added Proper Check for if MelonPlugin or MelonMod is Incorrectly Loaded.
57. Implemented MelonPlatform Attribute Check.
58. Implemented MelonPlatformDomain Attribute Check.
59. Added MelonAdditionalDependencies Attribute.
60. Implemented MelonAdditionalDependencies Attribute Check.
61. Implemented File Path Display for Latest.log when Internal Failures occur.
62. Fixed Issue with Missing Plugin Check.
63. Fixed Issue with TomlArray trying to act as a List.
64. Made Initialization Exception Logging more Verbose.   (Credits to [constfold](https://github.com/constfold) :3)
65. Fixed Issue with TomlArray Transform causing Out of Range Exception.
66. Fixed Issue with FileSystemWatcher throwing an NIE on some MonoBleedingEdge Games.
67. Changed TomlArray.Insert to TomlArray.Replace.
68. Upgraded Harmony Library to HarmonyX.
69. Ported Il2Cpp Patch Fix to HarmonyX Patcher.
70. Added Compatibility Layer for Harmony.
71. Added and Implemented RegisterTypeInIl2Cpp Attribute to further streamline Registering Custom Types in Il2Cpp.
72. Fixed Internal Failure Issue from Missing Exports with Older Mono.
73. Added MelonPreferences_Category.SaveToFile Method.
74. Added MelonPreferences_Category.LoadFromFile Method.
75. Added VerifyLoaderVersion Attribute for Melons to Specify MelonLoader Version Dependency.
76. Added VerifyLoaderBuild Attribute for Melons to Specify MelonLoader Build Dependency.
77. Implemented Fallback System to Assembly Generator.
78. Implemented Fallback Obfuscation Regex for Among Us.
79. Added Enum support in MelonPreferences.   (Credits to [loukylor](https://github.com/loukylor) :3)
80. Fixed Issue with Enum.TryParse in TomlMapper.
81. Fixed Issue with Console Title being improperly set on older Unity Versions.
82. Fixed Issue with Missing System.dll on older Unity Versions.
83. Fixed MME Issue on older Unity Versions.
84. Fixed Issue with Mods not running on older Unity Versions.
85. Added Load Balancing Hosting for Assembly Generator RemoteAPI.   (Credits to [DubyaDude](https://github.com/DubyaDude) and [Samboy](https://github.com/SamboyCoding) :D)
86. Fixed Small Bug in the RemoteAPI Request System for the Assembly Generator.
87. Added Compatibility Layer Resolver to MelonHandler for Future Changes.
88. Cleaned Up Inlined Harmony Patch Warning Implementation for Il2Cpp Games.
89. Isolated All Harmony Modifications to their own Files.
90. Updated HarmonyX to v2.4.0.
91. Fixed Issue with Assembly Generator causing Regeneration every Launch.
92. Cleaned Up Melon Method Call Implementation.
93. Added Compatibility Layer Resolver for IPA Plugins to be Loaded as Mods.
94. MelonInfo Attribute No Longer Requires that an Author be Set.
95. Fixed Issue with IPA Compatibility Layer not Loading Multi-Plugin Assemblies.
96. Fixed Issue with Harmony Patch Attributes not abiding by Melon Priority or Dependency Graph.
97. Fixed Issue with RegisterTypeInIl2Cpp Attribute not abiding by Melon Priority or Dependency Graph.
98. Fixed Harmony Patch Attribute Collision Issue with IPA Compatibility Layer and Multi-Plugin Assemblies.
99. Fixed Issue with ILRepack not Running for Debug Compilation of MelonLoader.dll.
100. Fixed Issue with ILRepack not using NuGet Package Paths for Merging.
101. Fixed Issue with MelonLoader.dll Compilation would copy NuGet Package Dependencies to Output Folder.
102. Improved Compatibility Layer Resolve System.
103. Improved MelonLoader Core Initialization.
104. Temporarily Disabled the "--melonloader.agfregenerate" Launch Option.
105. Added Managed-Sided Command Line Interface.
106. Converted Assembly Generator to be ran by Base Assembly.
107. Fixed Issue with Parsing Unity Version on Weirdly Marked Versions.
108. Fixed Issue with Mono Base Directory being Wrong causing Crashes.
109. Added Warning for Transpiler Use on Il2Cpp Unhollowed Methods.
110. Added More to Harmony Compatibility Layer.
111. Fixed Issue with Missing Harmony.GeneralExtensions and Harmony.CollectionExtensions in Harmony Compatibility Layer.
112. Added Description, Validator and BoxedValue to MelonPreferences_Entry.  (Credits to [Sinai](https://github.com/sinai-dev) :D)
113. Fixed Issue with not Forcing FloatValueSyntax to serialize with decimal for MelonPreferences_Entry.  (Credits to [Sinai](https://github.com/sinai-dev) :D)
114. Fixed Order of Validation for MelonPreferences_Entry.  (Credits to [Sinai](https://github.com/sinai-dev) :D)
115. Moved ValueValidator to its own class.  (Credits to [Sinai](https://github.com/sinai-dev) :D)
116. Added BoxedEditedValue to MelonPreferences_Entry.  (Credits to [Sinai](https://github.com/sinai-dev) :D)
117. Moved Plugin Loading and OnPreInitialization to before Assembly Generation.
118. Added MelonUtils.ParseJSONStringtoStruct.
119. Added Custom Hosts Interface for Plugins to add new Contact Hosts for the Assembly Generator's RemoteAPI.
120. Removed VRChat_OnUiManagerInit check and override.
121. Removed MelonUtils.IsVRChat.
122. Added System.Collections.Generic.List Mapping Support for MelonPreferences.
123. Assembly Generator now uses the MelonPreferences API for it's Config.
124. Fixed NRE Issue with MelonUtils.ParseJSONStringtoStruct.
125. Fixed String Null Issue with MelonPreferences.
126. Fixed NIE Issue with FileSystemWatcher on GORN failing to be caught properly.
127. Fixed Issue with Il2CppAssemblyUnhollower Unstripping Methods and failing to Resolve Unity Dependencies.
128. Fixed Issue with Il2CppAssemblyGenerator not setting Working Directory for ProcessStartInfo.
129. Added Separate Support Module for Compatibility Layer System to keep References Clean.
130. Made Compatibility Layer System Extendible.
131. Removed Tomlyn.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
132. Added Tomlet created by Samboy.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
133. Converted MelonPreferences to use Tomlet.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
134. Updated to Tomlet 1.1.0.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
135. Added ReflectiveCategory API for Type-Checked Category Creation.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
136. Added SaveToFile to ReflectiveCategory.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
137. Added a Force-Save option to ReflectiveCategory.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
138. Fixed NRE Issue with MelonPreferences when Parsing TOML Document.
139. Fixed Issue with DAB functionality not properly iterating its list.
140. Changed Assembly Generator Config to use new Reflective Categories.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
141. Fixed NRE Issue with Reflective Categories.
142. Updated to Tomlet 1.2.0.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
143. Updated Il2CppAssemblyUnhollower to v0.4.14.0.
144. Updated Il2CppDumper to v6.6.2.
145. Improved BHaptics Support Implementation.
146. Fixed Issue with PatchShield preventing Harmony Patching on some Games.
147. Temporarily Disabled Launch Options Config File.
148. Fixed Issue with Il2Cpp Support Fixes running before Il2CppAssemblyGenerator.
149. Fixed Launch Option Conflict Issue with Debug Builds.
150. Fixed Issue with FileWatcher's NotImplementedException Detection.
151. Updated to Tomlet 1.3.0.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
152. Fixed Ambiguous Match Found Issue with UnhollowerSupport.
153. Updated to Tomlet 1.3.1.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
154. Fixed NRE Issue with Preference Entries to TOML.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)
155. Fixed NRE Issue with Assembly Generator RemoteAPI.
156. Fixed MFE Issue with MelonBase.Harmony.
157. Fixed Loading Issue with MelonPreferences_ReflectiveCategory.
158. Added OnEarlyStartOnApplicationEarlyStart for Plugins.  (Credits to [Slaynash](https://github.com/Slaynash) :3)
159. Moved Il2CppAssemblyGenerator after OnApplicationEarlyStart.  (Credits to [Slaynash](https://github.com/Slaynash) :3)
160. Fixed Issue with Loading Melons from ZIP Archives.
161. Rewrote File Handling and Assembly Loading System.
162. Fixed Issue with Extra Tomlet DLL getting copied to Support Modules.
163. Added MDB Loading Support for ZIPs.
164. Added Source Code Detection for ZIPs.
165. Fixed Bad Image Format Exception Issue with Loading DLLs from ZIPs.
166. Fixed Issue with Assembly Generator Contacting All RemoteAPI Hosts when One Responded.
167. Added OnSceneWasUnloaded MelonMod Override.
168. Fixed IOException Issue with Assembly Generator extracting ZIP Archives.  (Credits to [Extacy](https://github.com/Extacy) :D)
169. Fixed Issue with MelonHandler being unable to resolve MelonPriority type.
170. Added Better Exception Handling during Melon Resolving.
171. Fixed IOORE Issue with Melon Enumeration.
172. Fixed Issue with StackTrace not showing Line Numbers even when Debug Symbol Information was present.
173. HarmonyX is now integrated and updated through NuGet.
174. Fixed Issue with ILRepack and Tomlet Conflicting.
175. Fixed Issue with UnhollowerSupport causing BIF Exception to be thrown.
176. Fixed Issue with Melon Sorting.
177. Fixed Issue with Dependency Sorting.
178. Fixed Issue with Assembly Attribute Finding in MelonHandler.
179. Improved IL2CPP coroutine exception reporting to include faulty coroutine name, don't continue running an erroring-out coroutine.   (Credits to [knah](https://github.com/knah) :D)
180. Fixed Issue with MelonHandler checking File Extensions.
181. Fixed Issue with MelonHandler not properly abiding by Load Mode.
182. Added Message Suppression Support for the RegisterTypeInIl2Cpp Attribute.
183. Fixed Issue with VerifyLoaderVersion Attribute causing an IOORE when parsing SemVer.
184. Fixed Issue with VerifyLoaderVersion Attribute not properly abiding by IsMinimum.
185. Updated Il2CppAssemblyUnhollower to v0.4.15.0.
186. Fixed Issue with newer Il2CppAssemblyUnhollower version breaking Harmony Patching.   (Credits to [ds5678](https://github.com/ds5678) :D)
187. Fixed Issue with Displaying MelonLoader Version Information.
188. Updated Il2CppAssemblyUnhollower to v0.4.15.1.
189. Added Missing Harmony Patch Forwarders.   (Credits to [ds5678](https://github.com/ds5678) :D)
190. Added Error Message for when a Plugin or Mod attempts to Load from the Wrong Folder.
191. Updated Tomlet to v1.3.3.
192. Added MelonLoader.HarmonyDontPatchAll Assembly Attribute.
193. Prepared Changes for Il2CppAssemblyUnhollower v0.4.15.2.   (Credits to [ds5678](https://github.com/ds5678) :D)
194. Added dnSpy Debugger Environment Variable Handling.   (Credits to [TechInterMezzo](https://github.com/TechInterMezzo) :D)
195. Fixed Issue with DNSPY_UNITY_DBG Environment Variable not being Handled.
196. Fixed Crash Issue with Certain Oculus Platform Games.
197. Fix wrong type being created in IL2CPP SM_Component.   (Credits to [knah](https://github.com/knah) :D)
198. Updated Il2CppAssemblyUnhollower to v0.4.15.2.   (Credits to [knah](https://github.com/knah) :D)
199. Fixed Issue with Directory Check overextending.
200. Improved Native Library Handling.
201. Fixed NRE Issue with Support Module being unable to find Application.buildGUID on Certain Unity Engine Versions.
202. Fixed Issue with Export mono_debug_domain_create on Older Mono Versions.

---

### v0.3.0:

1. Rewrote Proxy from scratch.
2. Rewrote Bootstrap from scratch.
3. Rewrote MelonLoader from scratch.
4. Rewrote AssemblyGenerator from scratch.
5. Rewrote Support Modules from scratch.
6. Added x86 Support.
7. Proxy now has the ability to be named version.dll, winmm.dll, or winhttp.dll.
8. OnModSettingsApplied has been changed to OnPreferencesSaved.
9. Temporarily Removed OnLevelIsLoading.
10. Internal Failures are now a lot more informative.
11. Debug Mode is now a lot more informative.
12. Logs folder has been moved to MelonLoader/Logs.
13. Logger now produces a latest.log file in the MelonLoader folder.
14. Removed LightJson. Not Needed.
15. Load Mode "-dev" Name Extension has been changed to ".dev" Extension.
16. Added Launch Option "--melonloader.dab" to Debug Analytics Blocker.
17. Fixed Assembly Resolve Issue between Plugins and Mods.
18. MelonHandler now requires Name, Version, and Author to all be properly set in Melons.
19. Author is now mandatory to be set in MelonInfoAttribute for Melons. It can no longer be set to null.
20. Fixed issue with Support Module due to Time.deltaTime being stripped.
21. Fixed Issue with Mono Support Module.
22. Fixed Issue with Assembly Generator.
23. Fixed Issue with Queued Coroutines in Mono Pre-5 Games.
24. Re-added "--quitfix" Launch Option.
25. Fixed Console On Top Check.
26. Fixed Legacy Support for OnLevelWasLoaded and OnLevelWasInitialized.
27. Fixed Legacy Support for Preferences and OnModSettingsApplied.
28. Fixed Type Overwrite Issue with MelonPreferences.
29. Fixed Loading Issue when a Plugin is in the Mods folder or vice-versa.
30. Improved Loading of Plugins and Mods.
31. Re-added UnityEngine.Il2CppAssetBundleManager.
32. Fixed Issue with UnityEngine.Il2CppAssetBundleManager being outdated.
33. Changed Unity Dependencies URL to LavaGang/Unity-Runtime-Libraries.
34. Fixed Type Overwrite Issue with MelonPreferences when Loading during Runtime.
35. Fixed Legacy Support for MelonPluginInfo and MelonPluginGame.
36. Re-added Launch Option "--melonloader.agregenerate".
37. Re-added Launch Option "--melonloader.agfvunhollower".
38. Added Launch Option "--melonloader.agfvdumper" to Force the Version of Il2CppDumper to use.
39. Added Launch Option "--melonloader.agfvunity" to Force the Version of Unity Dependencies to use.
40. Fixed Assembly Resolve Issue when a Plugin references a Mod.
41. Fixed Displacement Issue when calling Save for MelonPreferences or Legacy MelonPrefs.
42. Fixed Issue with Legacy MelonPrefs logging "Legacy Config Saved!" even when nothing happened.
43. Fixed Issue with DEV Load Mode not working with ZIP Archives.
44. Added Check to MelonHandler to make sure Plugins and Mods only load once from 1 DLL copy.
45. Legacy MelonPrefs now properly redirect to MelonPreferences.
46. Added MelonUtils.IsOldMono.
47. Fixed Issue with MelonPreferences creating a new entry when one already exists.
48. Added Auto-Converter to convert MelonPrefs to MelonPreferences.
49. Fixed Issue with Auto-Converter deleting modprefs.ini but not saving MelonPreferences.cfg.
50. Fixed Issue with Console Output not Flushing.
51. Fixed Issue with Log Output not Flushing.
52. Fixed Crash Issue with Fresh Install Assembly Generation.
53. Support Module now uses MelonUtils.GetUnityVersion properly.
54. Fixed Hooking Issue with Harmony Attributes.
55. Fixed Issue with Reflection based Assembly Resolving.
56. Moved Plugins and Mods folders back to the Game Installation folder.
57. Fixed ConsoleOnTop Focusing Issues.
58. Added More HostNames to Analytics Blocker.
59. Fixed Issue with Legacy MelonPrefs Support when GetString or SetString was called.
60. Added "MonoBleedingEdge.x86" and "MonoBleedingEdge.x64" to the Mono Directory Search.
61. Fixed Issue with certain Mono Games causing the Mono Directory Search to fail.
62. Disabled Close Button on Console during Assembly Generation.
63. Console will now properly appear for Assembly Generation only on Il2Cpp Games when using Launch Option "--melonloader.hideconsole".
64. Fixed Issue with Launch Option "--melonloader.hideconsole" causing a Crash.
65. Fixed Execution Order of Console Initialization when using Launch Option "--melonloader.hideconsole".
66. Fixed Issue with Il2CppDumper and Il2CppAssemblyUnhollower hanging when Games are closed during Assembly Generation.
67. Launch Option "--melonloader.dab" no longer requires "--melonloader.debug".
68. Fixed Visual C++ Error that occurs when closing certain Games by closing the Console  (for example: Audica).
69. Fixed Issue with Launch Option "--melonloader.hideconsole" ignoring "--melonloader.debug".
70. Fixed Console Coloring Issue.
71. Added MelonDebug.Msg.
72. Fixed Issue with MelonLoader Unity Debug Log not running when Plugins are loaded.
73. Fixed Issue with VRChat_OnUiManagerInit not getting called.
74. MelonLoader Unity Debug Log is now default even when no Plugins or Mods are loaded.
75. Fixed Issue with No Attribute showing as Universal.
76. Implemented Fallback Game Compatibility for Melons when no app.info is found.
77. Added Internal Failure when failing to get Unity Version.
78. Implemented globalgamemanagers as Fallback for Unity Version.
79. Added Try Catch to Support Module Loading.
80. Fixed Issue with Unity Version Fallback not Triggering.
81. Fixed Issue with Exceptions not including StackTrace.
82. Fixed Issue with certain Mono Games not having the FileVersionInfo class.
83. Added Value Change Callbacks to MelonPreferences Entries.
84. Cleaned up MelonPreferences Entry Exception Handling.
85. Fixed Issue with Launch Option "--melonloader.dab" that caused it to only Log when "--melonloader.debug" was used.
86. Fixed Issue with Bootstrap Game Warnings not Logging.
87. Fixed Issue with Il2Cpp Support Module failing to Parse Unity Version.
88. Fixed Issue with Il2Cpp Support Module failing to load "Assembly-CSharp.dll" on VRChat.
89. Added Legacy Style Methods to MelonPreferences.
90. Fixed Issue with the GetString and SetString methods for ModPrefs and MelonPrefs Legacy Support.
91. Fixed Logger Flushing.
92. Redirected 0Harmony.dll References to MelonLoader.dll.
93. Added Current Culture Fix to ensure Culture is always Invariant.
94. Fixed Issue with Product Names and Versions not using Translation Query.
95. Fixed Issue with Debug Compilation.
96. Added SHA512 Check to Installer Updates.
97. Added SHA512 Check to Installer Downloads.
98. Fixed Issue when using the Installer to install v0.2.
99. Fixed Issue with Releases Selection in Installer not being properly Sorted.
100. Added Shortcut Support to Installer.
101. Added HarmonyShield Attribute.
102. HarmonyShield now protects Methods and Assembly from being patched.
103. Added HarmonyShield Protection to MelonLoader, SM_Il2Cpp, SM_Mono, SM_Mono.Pre-2017, and SM_Mono.Pre-5.
104. Fixed Issue with Unneeded Return in Logger breaking Logs after Plugin Loading.
105. Fixed Issue with Console Buffer.
106. Fixed Issue with Console Coloring.
107. Added MelonColor Attribute for Melons to set their Name Color.
108. Fixed Issue with OnSceneWasLoaded not being called.
109. Added string Parameter to OnSceneWasLoaded and OnSceneWasInitialized that gets passed the Scene's Name.
110. Plugins and Mods now have their Names Colorized properly when Loading.
111. Added HarmonyShield Support for Classes and Structs.
112. Fixed Internal Failure with mono_free on Non-BleedingEdge Mono Games.
113. Improved HashCode Implementation.
114. Added HashCodes for Support Modules.
115. Added File Change Detection for MelonPreferences.
116. Fixed Issue with MelonPreferences not Updating after Load.
117. Added MelonIncompatibleAssembliesAttribute to mark Plugins and Mods as Incompatible with each other.   (Credits to [YOWChap](https://github.com/yowchap) :D)
118. Fixed Issue with 127.0.0.1 showing as a Unique Host Name.
119. Fixed Issue with ValueEdited of MelonPreferences.
120. Fixed Issue with ValueEdited not correctly saving to MelonPreferences when Save is called.
121. Fixed Issue with ValueEdited not being set when Auto-Converter runs.
122. Added UnityEngine.Il2CppImageConversionManager to Managed folder to help with Image Conversion on Il2Cpp Games.
123. Added Better Exception Handling for MelonLoader Core Initialization.
124. Added Error Log File Output to Installer.
125. Added MelonPreferences Auto-Downgrade System to Installer.
126. Added MelonUtils.ColorToANSI to convert ConsoleColor to ANSI String.
127. Fixed Issue with ALPHA Setting in Installer not abiding by Selected Theme.
128. Fixed Issue with MelonPreferences Auto-Downgrade System in Installer when clean installing.
129. Fixed Issue with Assembly Generator not Logging Launch Arguments.
130. Added LaunchOptions.ini to allow for Configuration of Launch Options without using the Command Line.
131. Fixed Issue where Launch Option "--melonloader.hidewarnings" would not function correctly.
132. Added OnPreferencesLoaded Override for Plugins and Mods.
133. Fixed Issue where Plugins weren't given priority for OnApplicationStart.
134. Fixed Issue where certain Games would be unable to have their Unity Version read.
135. Temporarily removed the Force-Regenerate Launch Option.
136. Added GameAssembly Hash Check Logs.
137. Added double and long Support to MelonPreferences.
138. Improved MelonPreferences Type Conversion when Loading.
139. Improved Lambda Expression Usage in MelonPreferences.
140. Improved MelonPreferences Type Conversion when Loading string, floats, ints, longs, and doubles.
141. Improved MelonPreferences Change Callback Invoking.
142. Fixed Issue where Bootstrap and Proxy would cause Crashes when compiled under Debug Compilation.
143. Improved Core Initialization.
144. Fixed Issue with MelonPreferences Caching on Load and not setting the cache as Hidden.
145. Fixed Issue with Older Mono that doesn't need a Posix Helper Native Module.
146. Fixed Issue with Logger not properly detecting the Calling Melon.
147. Fixed Issue with Debug Logger not properly coloring Melons.
148. MelonPreferences Type Handling is now Modular for easier extension.
149. MelonPreferences now supports the Type byte.
150. Fixed Issue with MelonPreferences Loading when there is an Unhandled Type in the Config File.
151. Fixed Issue with MelonPreferences where Saving would Trigger the FileWatcher and cause a needless reload.
152. Fixed Issue with MelonPreferences where the Config File wasn't creating itself on Initialization.
153. Fixed Issue with MelonPreferences causing the Config File to ignore certain Preferences during Initialization.
154. Fixed Issue with Compatibility Layer failing to resolve type for MelonLoader.UnhollowerSupport.
155. Fixed Issue with Compatibility Layer failing to resolve MelonMod.InfoAttribute, MelonMod.GameAttributes, MelonPlugin.InfoAttribute, and MelonPlugin.GameAttributes.
156. Fixed Issue with Mods on BONEWORKS having OnSceneWasLoaded and OnSceneWasInitialized called too early.
157. Fixed Issue with Mods on BONEWORKS having OnSceneWasLoaded and OnSceneWasInitialized called for the Loading Screen.
158. Added BONEWORKS_OnLoadingScreen Override for Mods and it runs when BONEWORKS shows the Loading Screen.
159. Fixed Issue with MelonDebug.Msg causing Crash.
160. Fixed Issue with MelonDebug.Msg not checking if Debug is Enabled.
161. Added MelonUtils.GetCurrentGameAttribute that returns the Current Game's MelonGameAttribute.
162. Added MelonGameAttribute.Universal, MelonGameAttribute.IsCompatible, and MelonGameAttribute.IsCompatibleBecauseUniversal.
163. Fixed Issue with BONEWORKS_OnLoadingScreen not being called.
164. Fixed Issue with Certain Melons having the wrong Default Color.
165. Improved MelonPreferences Error Handling.
166. Improved MelonPreferences Unsupported Type Handling.
167. Added string[] and bool[] Support to MelonPreferences.
168. Added int[] and float[] Support to MelonPreferences.
169. Fixed Issue with MelonPreferences Value Type Conversion causing Conflicts in File.
170. Added long[] Support to MelonPreferences.
171. Added byte[] Support to MelonPreferences.
172. Added Warning to Debug Mode for when Attempting to Harmony Patch Methods that may possibly be Inlined.   (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
173. Added a Built-In bHaptics API.
174. Fixed Issue with MelonPreferences not setting properly while using Fallback Functionality.
175. Fixed Issue with Melon Name not Showing for Inline Harmony Patch Detection Warning.   (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
176. Fixed Issue with Null Reference Exception from MelonLogger.ManualWarning.   (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
177. Fixed Issue with Null Reference Exception from Inline Harmony Patch Detection.
178. Fixed Issue with MelonUtils.IsVRChat causing Null Reference Exception.
179. Fixed Issue with MelonUtils.IsBONEWORKS causing Null Reference Exception.
180. Fixed Issue with Native Unity Logs showing when not in Debug Mode.
181. Fixed Issue with Certain Unity Games when the EXE has no Information Attached.
182. Fixed Issue with Unity Games using a Version older than 5.0 not Loading Mods.
183. Fixed Issue with Unity Games using a Version older than 5.0 having mainData instead of globalgamemanagers.
184. Fixed Issue with Support Modules with Mono Games not having UnityEngine.Transform.SetAsLastSibling.
185. Added Preload Support Module.
186. Fixed Issue with Unity Games not having System.Core.dll.
187. Added bHaptics.DeviceType.
188. Added bHaptics.IsDeviceConnected.
189. Added bHaptics.DeviceTypeToPositionType.
190. Added bHaptics.DeviceType Methods.
191. Fixed Issue with Support Modules throwing a Target Framework Exception.
192. Fixed Internal Failure with Operating Systems older than Windows 10.
193. Fixed Issue with C# 9 Record Types making Assemblies Unloadable.   (Credits to [knah](https://github.com/knah) :D)
194. Added Melon Hashes to Logs.   (Credits to [TrevTV](https://github.com/TrevTV) :P)
195. Temporarily Disabled Analytics Blocker for x86 (32bit) to Fix Random Crash Issue.
196. Fixed Issue with Console Coloring on Operating Systems older than Windows 10.
197. Fixed Issue with app.info Does Not Exist Warning showing Twice.
198. Re-Implemented Color Variations of MelonLogger.Msg.
199. Fixed Issue with Melon Logging Traceback not finding certain Melons in the Call Stack.
200. Added MDB Symbol Support.   (Credits to [avail](https://github.com/avail) :P)
201. Added Deobfuscation Map Support.
202. Added "--melonloader.agfregenerate" Launch Option.
203. Re-factored Assembly Generator.
204. Re-factored MelonPreferences.
205. MelonPreferences Entries are now Extendable for Custom Type Support.
206. Added MelonPriority Assembly Attribute.
207. Fixed String Issue with MelonPreferences.
208. Re-factored MelonColor Support.
209. Fixed Issue with Legacy Settings failing to Load and breaking settings system completely.   (Credits to [knah](https://github.com/knah) :D)
210. Fixed Issue suppress exceptions from Settings Save/Load.   (Credits to [knah](https://github.com/knah) :D)
211. Rewrote settings classes to be much cleaner and easier to extend.   (Credits to [knah](https://github.com/knah) :D)
212. Updated Il2CppAssemblyUnhollower to v0.4.13.0.   (Credits to [knah](https://github.com/knah) :D)
213. Fixed Issue with Deobfuscation Map not Downloading if File does not Exist.   (Credits to [knah](https://github.com/knah) :D)
214. Fixed Issue String Settings not loading from TOML.   (Credits to [knah](https://github.com/knah) :D)
215. Fixed Issue Legacy Float Settings with Round Values not loading from INI.   (Credits to [knah](https://github.com/knah) :D)
216. Rewrote TOML Array Handling.   (Credits to [knah](https://github.com/knah) :D)
217. Fixed Issue with Virtual Terminal Processing on some systems.
218. Fixed Issue with MelonPreferences having Category and Entry Display Names not be set to Identifier when null.
219. Fixed Issue with MelonPreferences.CreateEntry not creating Category.
220. Fixed Issue with MelonPreferences.CreateCategory causing an Exception when it wasn't supposed to.
221. Fixed Issue with "Collection was modified" Error.
222. Fixed No Melon Attribute Found Issue.
223. Fixed Issue with Melon Override Exceptions not showing which Melon it was.
224. Fixed Issue with Protobuf Assemblies being rejected.   (Credits to [knah](https://github.com/knah) :D)
225. Fixed Issue with Assembly Generator's OverrideAppDomainBase Method under Mono.   (Credits to [MiincK](https://github.com/MiincK) :D)
226. Changed MelonBase.Color to MelonBase.ConsoleColor.
227. Fixed Enumerator Issue with Index Loop Locking.
228. Fixed Issue with Duplicate Melons causing the rest of the Melons to Not Load.
229. Added MonoBleedingEdge Metadata.
230. Re-factored Assembly Generator run with Mono instead of CLR.   (Credits to [MiincK](https://github.com/MiincK) :D)
231. Fixed NRE Issue with Audica and similar games.   (Credits to [MiincK](https://github.com/MiincK) :D)
232. Fixed log file name time format.   (Credits to [MiincK](https://github.com/MiincK) :D)
233. Fixed NRE Issue with Newer Games after Audica.   (Credits to [MiincK](https://github.com/MiincK) :D)
234. Changed Mono Configuration Directory on Il2Cpp Games to the Included Metadata.   (Credits to [MiincK](https://github.com/MiincK) :D)
235. Fixed Issue with Assembly Generator getting the Incorrect Configuration Directory.   (Credits to [MiincK](https://github.com/MiincK) :D)
236. Fixed Issue with Assembly Generator when Deobfuscation Map already exists.   (Credits to [knah](https://github.com/knah) :D)

---

### v0.2.7.4:

1. Fixed Issue with Amplitude using new Endpoints.

---

### v0.2.7.3:

1. Updated Il2CppAssemblyUnhollower to v0.4.10.0.
2. Il2CppDumper and Il2CppAssemblyUnhollower executables will now display their command lines in the log files.   (Credits to [stephenc87](https://github.com/stephenc87) :D)

---

### v0.2.7.2:

1. Fixed Small Issue with VRChat_OnUiManagerInit.

---

### v0.2.7.1:

1. Use a different approach to console cleaning for better interop with other tools.   (Credits to [knah](https://github.com/knah) :D)

---

### v0.2.7:

1. Console no longer has stdout directed to it.   (Credits to [knah](https://github.com/knah) :D)
2. Removed VRChat Auth Token Hider as it's no longer necessary.   (Credits to [knah](https://github.com/knah) :D)
3. Added Try Catch to DisableAnalytics Hooks.
4. Updated Il2CppAssemblyUnhollower and Il2CppDumper Versions.   (Credits to [knah](https://github.com/knah) :D)
5. Implemented potentially dead method detection.   (Credits to [knah](https://github.com/knah) :D)
6. Exposed new Console Output Handle and assign it to Managed Console Class Output.   (Credits to [knah](https://github.com/knah) :D)
7. Added Unity 2019.4.3 Dependencies.
8. Bumped Il2CppAssemblyUnhollower Version.

---

### v0.2.6:

1. Fixed Issue with Logger Timestamp making New Lines.
2. Added Silent Launch Option to Installer.   (Credits to [TrevTV](https://github.com/TrevTV) :P)
3. Fixed Internal Failure caused by DisableAnalytics for certain users.
4. Added Operating System log to startup info.
5. Added ValueTupleBridge to Managed Folder.
6. Added Unified Attributes for both Plugins and Mods.
7. Added Legacy Attribute Support.
8. Fixed Logger Issue with new Unified Attributes.
9. Deprecated MelonModLogger, use MelonLogger instead.
10. Deprecated ModPrefs, use MelonPrefs instead.
11. Added HarmonyInstance Method for easier Unpatching of Mods and Plugins.
12. Create MelonLoaderComponent after first scene load in Mono Games.  (Credits to [knah](https://github.com/knah) :D)
13. Removed Launch Option --melonloader.devpluginsonly.
14. Removed Launch Option --melonloader.devmodsonly.
15. Added Launch Option --melonloader.loadmodeplugins.
16. Added Launch Option --melonloader.loadmodemods.
17. Fixed Issue with Debug Mode causing Crashes for Some Games.
18. Fixed Issue with Manual Zip overriding Latest Version.
19. Fixed Issue where Manual Zip would get set as Default Selection.
20. Unity Analytics now redirected to localhost, rather than throwing null and exceptions.  (Credits to [Emilia](https://github.com/thetrueyoshifan) :3)
21. Plugins are now able to use OnUpdate.
22. Plugins are now able to use OnLateUpdate.
23. Plugins are now able to use OnGUI.
24. Added GetUnityTlsInterface to Imports.
25. Implemented Native Sided SSL/TLS/HTTPS Fix.
26. Fixed Issue with Support Modules not loading due to Missing Method errors.
27. Fixed Issue with attaching dnSpy on Il2Cpp Games.
28. Replaced mono-2.0-bdwgc.dll with dnSpy Debug dll.
29. Debug Mode will now use LoadFrom instead of Load for breakpoint compatibility.
30. Fixed Crash Issue with DisableAnalytics.
31. Revised Console Logger Callbacks.
32. Fixed Issue with LogMelonError not running Callbacks.
33. Deprecated MelonLoader.Main use MelonLoaderBase or MelonHandler instead.
34. Revised Base Melon Handling.
35. Revised IniFile.
36. Fixed Issue with Plugins not getting OnUpdate called.
37. Fixed Issue with Plugins not getting OnLateUpdate called.
38. Fixed Issue with Plugins not getting OnGUI called.
39. Plugins are now able to use VRChat_OnUiManagerInit.
40. Fixed Coroutine Queue for Mono Games.
41. Added Launch Option --melonloader.consoleontop.  (Credits to [TrevTV](https://github.com/TrevTV) :P)
42. Fixed Issue with Assembly Generator not stopping when failing to Download.
43. Escalated Assembly Generator failures to Internal Failures.
44. Fixed Issue where Assembly Generator failures would cause a Crash.
45. Fixed Crash Issue with Unity TLS Fix for some Games.
46. Fixed Issue with Assembly Generator not working for some Users.
47. Fixed Crash Issue with DisableAnalytics for some Games.
48. Fixed Missing NULL Check in DisableAnalytics.  (Credits to [Sinai](https://github.com/sinai-dev) :D)
49. Added Try Catch to Assembly Generator Execution.  (Credits to [Samboy](https://github.com/SamboyCoding) :D)

---

### v0.2.5:

1. Fixed Issue that prevented deserializing structs with TinyJSON.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
2. Simplified External Tool Versions.
3. Updated Il2CppDumper to 6.3.3.
4. Added Unity 2019.3.15 Dependencies.
5. Fixed Issue with some Games causing DisableAnalytics to have an Internal Failure.
6. Fixed File Descriptions.
7. Added Icon to AssemblyGenerator.exe.
8. Temporarily Removed the ModSettingsMenu.
9. Temporarily Removed RenderHelper from Support Modules.
10. Fixed Issue with some Games causing DisableAnalytics to have an Internal Failure.
11. Fixed Issue with using Application.unityVersion on some Unity versions.
12. Fixed Issue with using SetAsLastSibling on some Unity versions.
13. Fixed Issue with Release Compilation still saving Debug files to the Output Directory.
14. Added UnityEngine.Il2CppAssetBundleManager to Managed folder to help with Asset Bundle Manipulation on Il2Cpp Games.
15. Installer now properly displays UPDATE instead of INSTALL when updating to a newer version.
16. Added Auto-Update System to Installer.
17. Installer Version Check now Parses the Project File directly.
18. Added Installer Check for Manual Zip (MelonLoader.zip) next to the Installer exe.
19. Added UN-INSTALL button to Installer.
20. Added Auth Token Hider for VRChat.
21. Added Console Cleaner for Normal Console
22. Console Cleaner now uses a Harmony Patch instead of unsafe hooking.
23. Improved Unhollower Logging Support.
24. Added Console Log Callbacks to MelonLoader.Console.
25. Installer now runs Update Check on Main Thread.
26. Improved Logger System.
27. Fixed Issue with Installer where installing from Manual Zip would cause it to download Latest Version.
28. Temporarily Removed Console Cleaner.

---

### v0.2.4:

1. Added Error Output Log to Installer.  (Credits to [TrevTV](https://github.com/TrevTV) :P)
2. Use List.Count / Array.Length property instead of Linq's Count() method.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
3. Added mechanism to catch when game is open to display additional information.  (Credits to [DubyaDude](https://github.com/DubyaDude) ^.^)
4. Harmony: Remove prefixes after postfixes to fix unpatching patches with __state variables.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
5. Fixed Issue with certain Games not having a File Version attached to the EXE.
6. Fixed Small Issue with Thief Simulator.
7. Added MetroUI, Fody, and new revised Dark Theme to Installer.  (Credits to [DubyaDude](https://github.com/DubyaDude) ^.^)
8. UnityCrashHandler now terminates upon running with MelonLoader.
9. Added Analytics Disabler.
10. Added Oculus and Facebook Tracking URLs to Analytics Disabler.
11. Added Fallback System for downloading Unity Dependencies needed for Unstripping.
12. Moved LightJson under its own Namespace as MelonLoader.LightJson.
13. Installer now correctly displays Download Percentage.  (Credits to [charlesdeepk](https://github.com/charlesdeepk) :D)
14. Installer now correctly displays Percentage of Zip Extraction.

---

### v0.2.3:

1. Added Unity 2019.3.4 Dependencies.
2. Temporarily Fixed Issue with TLS Fix on Certain Il2Cpp Unity Games.
3. Added Launch Option "--melonloader.agfvunhollower" that Forces the Assembly Generator to use a Specified Version of Il2CppAssemblyUnhollower.
4. Added Location property to the MelonMod class.
5. Revised Plugin System.
6. Fixed Issue causing the Assembly Generator to constantly Regenerate every time a Game Launched.
7. Installer now creates the Plugins folder.
8. Revised MelonMod class to use a new MelonBase.
9. All Commonly used Methods and Properties that both MelonMod and MelonPlugin use have been moved to MelonBase.
10. Plugins now use their own Attributes.
11. Fixed Issue that let Plugins load as Mods when placed in the Mods folder and vice-versa.
12. Added better Legacy Version handling for "--melonloader.agfvunhollower".
13. Added Clearer Display Text to State that DLL Status is Game Compatibility.
14. Also call Harmony patches if IL2CPP methods get called using reflection / il2cpp_runtime_invoke.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
15. Fixed Issue with Harmony Patches on Virtual Methods.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
16. Improved the Harmony error message if the unhollowed target method is not backed by an IL2CPP method.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
17. Fixed Issue with Harmony AccessTools not being able to resolve the no-args constructor if a static constructor exists.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
18. Added Support for Mono Debuggers.
19. Mono JIT Domain on Il2Cpp Games should now parse Command Line arguments properly.
20. Forced Installer to reinstall the entire MelonLoader folder.
21. Moved all Console and Log Functionality to Native Exports.
22. Updated Il2CppDumper to v6.3.0.
23. Updated Il2CppDumper Config Fix in the Assembly Generator to support Il2CppDumper v6.3.0.
24. Added Temp File Cache to Assembly Generator.
25. Re-enabled Launch Option "--melonloader.devpluginsonly".
26. Fixed Issue with Certain Mono Games not having OnApplicationStart called.
27. Added Unity 2018.4.2 Dependencies.
28. Fixed Debug Compilation Issue with Console.
29. Added Launch Option "--melonloader.hidewarnings" that will Hide Warnings from Displaying in Normal Console.  (Debug Console ignores this Launch Option.)
30. Fixed Issue with Support Modules not Unloading their Components properly when OnApplicationQuit is called.
31. Fixed Mistake with Error Catching Console Creation.
32. Log Spam Shield will now Disable itself when MelonLoader is using Debug Mode/Console.
33. Log Spam Shield has now been moved to the Native Logger.
34. Added Launch Option "--melonloader.maxwarnings" for Setting the Max Amount of Warnings for the Log Spam Shield.  (Debug Console ignores this Launch Option, Default is 100, 0 to Disable.)
35. Added Launch Option "--melonloader.maxerrors" for Setting the Max Amount of Errors for the Log Spam Shield.  (Debug Console ignores this Launch Option, Default is 100, 0 to Disable.)
36. Fixed Memory Leak in Log Spam Shield.
37. Fixed Issue with Exports.
38. Moved DLL Status to its own Enum.
39. Added "Compatibility" property to MelonBase.
40. Removed "IsUniversal" property from MelonBase.
41. Fixed Issue with Incompatible Plugins and Mods still being used after Logging.
42. Added Unity 2018.4.23 Dependencies.
43. Fixed Issue with Console.h / Console::IsInitialized().
44. Added Tomlyn to ModHandler for TOML Parsing.
45. Moved Tomlyn to its own Namespace under MelonLoader.Tomlyn.
46. Added Unity 2018.3.14 Dependencies.
47. Added Unity 2019.2.16 Dependencies.
48. Added Tomlyn to Assembly Generator for TOML Parsing.
49. Cleaned up Config Handling in Assembly Generator.
50. Fixed Issue with Corrupt Tomlyn Lib.
51. Added Dependency Graph System for Plugins and Mods.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
52. Fixed Issue with Plugins still calling OnPreInitialization even when Incompatible.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
53. Added Better Plugin and Mod Cleanup when OnPreInitialization or OnApplicationStart fails.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
54. Switched Assembly Generator Config from JSON to TOML.
55. Assembly Generator Config will now be saved to a file named "config.cfg" instead of "config.json".
56. Fixed Issue with Tomlyn in the ModHandler.
57. Added Unity 2020.1.0 Dependencies.
58. Added Temporary Warning for Missing Dependencies.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
59. Added MelonOptionalDependencies Attribute.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
60. Fixed compilation issues on .NET Framework 3.5.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
61. Implemented 'Populate' methods to TinyJSON to de-serialize JSON into an existing object.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
62. Reimplemented TinyJSON in AssemblyGenerator to be up to date with the one in the ModHandler.
63. Static Linked Runtime Library to Reduce User Issues with Microsoft Visual C++ 2015-2019 Re-distributable.  (Credits to [knah](https://github.com/knah) :D)
64. Updated Il2CppAssemblyUnhollower Download to v0.4.8.  (Credits to [knah](https://github.com/knah) :D)
65. Fixed Issue with the RenderHelper of ModSettingsMenu causing Error when SupportModule Unloads their Component.
66. SupportModules now properly Unloads their Component if no Plugins or Mods are loaded.
67. Temporarily Disabled ModSettingsMenu.
68. Added Launch Option "--melonloader.chromium" to turn the Console Color Magenta.  (Thanks Chromium lol)
69. Fixed Issue with Logger methods with Color Parameter being Overridden by RainbowMode or ChromiumMode.
70. Added Unity 2019.3.5 Dependencies.
71. Added Unity 5.6.7 Dependencies.
72. Added Unity 5.6.3 Dependencies.
73. Changed Launch Option "--melonloader.chromium" to "--melonloader.magenta".

---

### v0.2.2:

1. Downgraded Installer to .NET Framework 4.7.2.  (Credits to [knah](https://github.com/knah) :D)
2. Added Error Messages to Installer.  (Credits to [knah](https://github.com/knah) :D)
3. Added Proper Threading to Installer.  (Credits to [knah](https://github.com/knah) :D)
4. Installer now creates a Mods folder if none exists.
5. Fixed Installer Form Size.
6. Fixed Issue with Label in Installer Form not Auto-Centering.
7. Fixed Build Information for Installer.
8. Moved Plugins to its own separate Plugins folder.
9. Installer now creates a Plugins folder if none exists.
10. Installer now properly installs the Latest Version instead of a Predetermined Version.
11. Fixed and Re-Enabled --melonloader.hideconsole.
12. Added Log to Unity Debug to Signify to Developers that a Game has been Modded.
13. Fixed SSL/TLS issue with the Assembly Generator.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
14. Fixed Crash Issue with Harmony when Patching Methods Multiple Times or when a Method happens to be null.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
15. Made the Loaded Mods List public.
16. Fix ModPrefs.SaveConfig() overriding previous SetString/Bool/etc calls.  (Credits to [knah](https://github.com/knah) :D)
17. Fixed SSL/TLS Issue with the Installer.
18. Fixed Issue with Games that don't have Assembly-CSharp.
19. Added a check and a warning for if the OS is older than Windows 7.
20. Updated Il2CppAssemblyUnhollower Download to v0.4.4.  (Credits to [knah](https://github.com/knah) :D)
21. Cleaned up version.dll.
22. version.dll Proxy will now look for version_original.dll next to the EXE before loading from System32.
23. Small change to version.dll to Prevent Crashing when MelonLoader.dll fails to load.
24. Revised Loading System to work without needing Signatures.
25. Reorganized Installation Folder Structure.
26. Fixed Issue with Signature-less System on Pre-2017 Unity Versions.
27. Fixed Issue with Support Modules not loading on Unity Versions older than 5.3.
28. Fixed Issue Loading Mono Module.
29. Fixed Issue with OnApplicationStart getting called even when the Assembly Generator fails.
30. Switched Assembly Generator to Process Based Execution.
31. Fixed Mistake with Assembly Generator not returning false when Execution fails.
32. Fixed Directory Issue with Assembly Generator.
33. Updated Il2CppAssemblyUnhollower Download to v0.4.5.
33. Added new Launch Option --melonloader.devmodsonly.
34. Added Unity 2018.4.16 Dependencies.
35. Plugins now only call OnPreInitialization, OnApplicationStart, OnApplicationQuit, and OnModSettingsApplied.
36. Revised Installer Design.
37. Fixed Issue with Plugin Loading.
38. Fixed OnLevelWasLoaded and OnLevelWasInitialized on Unity Versions lower than 5.3.
39. Fixed GetActiveSceneIndex for Pre2017.2 Support Module.
40. Re-added TinyJSON to ModHandler.
41. Moved TinyJSON to its own Namespace.
42. Fixed SupportModule Issue with Pre-2017 Unity Games.
43. Added Force-Regeneration to Assembly Generator.
44. Added Launch Option --melonloader.agregenerate.
45. Added Support for Mods to Harmony Patch using Harmony Attributes.
46. Fixed Issue with Assembly Generator losing output lines.  (Credits to [knah](https://github.com/knah) :D)
47. Fixed Issue with Assembly Generator failing on paths with spaces.  (Credits to [knah](https://github.com/knah) :D)
48. Fixed SSL/TLS issues in Mods for Il2Cpp games.  (Credits to [knah](https://github.com/knah) :D)
49. Updated Il2CppAssemblyUnhollower Download to v0.4.6.  (Credits to [knah](https://github.com/knah) :D)
50. Improved String Support for Harmony in Il2Cpp Games.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
51. Added Harmony Support for null Il2Cpp Argument Values.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
52. Added Harmony Support for ref Types in Il2Cpp Games.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
53. Fixed Crash Issue when Exception is thrown from C# Code.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
54. Updated Method Targeting in Il2Cpp Games.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :3)
55. Installer now Downloads and Installs Required VC Redist.
56. Fixed Invalid Handle Issue with Console Coloring.
57. Added Embedded SharpZipLib to ModHandler.
58. Fixed Zipped Mod Support.
59. Moved SharpZipLib in ModHandler to its own Namespace.
60. Fixed Issue with Zipped Mod Support when using --melonloader.devmodsonly.
61. Added Launch Option --melonloader.devpluginsonly.
62. Updated Il2CppAssemblyUnhollower Download to v0.4.7.  (Credits to [knah](https://github.com/knah) :D)
63. Launch Option --melonloader.quitfix has been changed to --quitfix.
64. Temporarily Disabled Plugins.
65. Temporarily Disabled Launch Option --melonloader.devpluginsonly.
66. Added Unity 2018.1.9 Dependencies.
67. Added Unity 2019.2.17 Dependencies.
68. Added Unity 2019.2.21 Dependencies.
69. Regenerated Unity Dependencies Zips.
70. Fixed Issue with --melonloader.devmodsonly.
71. Disabled VC Redist Installation in Installer.
72. Fixed Issue with Installer not running as Administrator.
73. Fixed Installer UAC Issue.

---

### v0.2.1:

1. Overwrite files when copying freshly generated assemblies.  (Credits to [knah](https://github.com/knah) :D)
2. Implemented auto-download of required tools for Il2Cpp games.  (Credits to [knah](https://github.com/knah) :D)
3. Fixed Config Issue causing Assembly to always need to Regenerate.
4. Added an Installer.
5. Fixed Console Display Issue.
6. Temporarily disabled --melonloader.hideconsole.

---

### v0.2.0:

1. Fixed Issue with the Console still showing as Closed-Beta.
2. Fixed Issue with not having MelonLoader.ModHandler.3.5.dll included in the Release zip.
3. Fixed support for 2018.4.11f1 and Ori and the Will of the Wisps.  (Credits to Hector ^.^)
4. Added 2018.4.11f1 Unity Dependencies.  (Credits to Hector ^.^)
5. Removed Unity Dependencies from Release Build.  (No Longer Needed)
6. Fixed Issue with MelonCoroutines not allowing a yield of Coroutine within a Coroutine.  (Credits to [Slaynash](https://github.com/Slaynash) :3)
7. Revised README.MD.
8. Changed the Hook and Unhook Internal Calls in MelonLoader.Imports from internal to public to expose them and allow them to be used by Mods.
9. Revised CHANGELOG.MD.
10. Added Built-In QuitFix under the Launch Option --melonloader.quitfix  (For Games like VRChat where the Process Hangs after telling it to Exit)
11. Added Stop Methods to MelonCoroutines.  (Credits to [Slaynash](https://github.com/Slaynash) :3)
12. Fixed Error Handling in MelonCoroutines.  (Credits to [Slaynash](https://github.com/Slaynash) :3)
13. Fixed Issue with MelonCoroutines when using Non-Yielding Coroutines.  (Credits to [Slaynash](https://github.com/Slaynash) :3)
14. Added Support for Method Unstripping through Il2CppAssemblyUnhollower to MelonLoader.AssemblyGenerator.
15. Replaced the Doorstop winmm.dll with version.dll.
16. Fixed Compatibility with Il2Cpp Games for 0Harmony in MelonLoader.ModHandler.  (Credits to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc) :D)
17. Fixed Issue with 0Harmony not Unpatching Methods when Unloading.
18. Fixed Incompatibility Issue between 0Harmony in MelonLoader.ModHandler and Mono Game Support.
19. Added Event Fix for knah's Unhollower Logging.
20. Fixed Null Reference Exception Error when using MethodInfo of Generated Assembly Methods for 0Harmony.
21. Removed NET_SDK.
22. Added Custom Component System for the ModHandler.
23. Added Preload Support for Mods to Load before the Game Initializes.
24. Added OnPreInitialization for Mods that Preload.
25. Split support for Mono and Il2Cpp into their own Support Modules.  (Credits to [knah](https://github.com/knah) :D)
26. Fixed Issue with OnSceneLoad in Il2Cpp Support Module.  (Credits to [knah](https://github.com/knah) :D)
27. Split MelonCoroutines to integrate Support Module support.  (Credits to [knah](https://github.com/knah) :D)
28. Fixed Issues with needing the ModHandler compiled under a certain .NET Framework.  (Credits to [knah](https://github.com/knah) :D)
29. Fixed Issue with OnGUI.
30. Fixed Compatibility Issues with SCP Unity.
31. Fixed Issue with version.dll causing UnityCrashHandler to crash.
32. Fixed Compatibility Issues with Hot Dogs, Horseshoes & Hand Grenades.
33. Added Pre 2017 Mono Support Module.
34. Made Normal Console Open by Default.
35. Removed Launch Option "--melonloader.console".
36. Added Launch Option "--melonloader.hideconsole" to Hide the Normal Console.
37. Cleaned up Il2Cpp Imports.
38. Cleaned up Mono Imports.
39. Revised Command Line Parsing.
40. Fixed Issue with the Log Cleaner deleting Logs that were not made by MelonLoader.
41. Added Launch Option --melonloader.maxlogs. Sets the Max Logs for the Log Cleaner. Set as 0 to Disable the Log Cleaner.
42. Added Global Exception Handler to the Mono Domain.
43. Added Assembly Generation Integration.
44. Fixed Illegal Characters in Path Error.
45. Temporarily Disabled Zipped Mod Support.
46. Added Log Error Limiter.
47. Re-Enabled Download Links in Mod Logs.
48. Fixed Issue with TestMod not loading on Mono Games.

---

### v0.1.0:

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
12. Added CreateInstance method to Il2Cpp_Class in NET_SDK.Reflection.  (Credits to Kronik ^.^)
13. Fixed SystemTypeToIl2CppClass in NET_SDK.Il2Cpp.
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
29. Fixed Issue with the Compatibility Check when using multiple MelonModGameAttribute.
30. Fixed Issue with OnLevelWasLoaded and OnLevelWasInitialized getting the wrong Scene Index and only running once then never again.
31. Upgraded MelonLoader.ModHandler and TestMod to .NET v4.7.2.  (As requested by Emilia :D)
32. Made MelonModController use directly invokable delegates for faster invoking.  (Credits to Kronik ^.^)
33. Removed Unneeded Caching of MethodInfo in MelonModController.
34. Added mono-2.0-boehm.dll to Mono Check.
35. Added --melonloader.rainbow and --melonloader.randomrainbow as Launch Options.  (As requested by Hordini :3)
36. Redesigned the Logging system to be unified between MelonLoader and MelonLoader.ModHandler.
37. Colorized Errors in both Consoles.
38. Added the ability for Mods to set a Color when Logging.
39. Fixed Issue with --melonloader.rainbow and --melonloader.randomrainbow not working when using --melonloader.console.
40. Added Unload Functionality. Entirely unloads MelonLoader.  (MelonLoader::UNLOAD() or MelonLoader.Imports.UNLOAD_MELONLOADER())
41. Fixed Issue with using 1 or 0 for a boolean in ModPrefs.
42. Fixed Issue with OnUpdate, OnFixedUpdate, and OnLateUpdate invoking too early.
43. Fixed Issue with Unload Functionality not closing either Consoles.
44. Added MelonCoroutines to handle Coroutine functionality for Mods on both Il2Cpp and Mono.  (Credits to [knah](https://github.com/knah) :D)
45. Fixed Issue with Logger removing Old Logs when it shouldn't.
46. Fixed Issue with OnLevelWasLoaded, OnLevelWasInitialized, and VRChat_OnUiManagerInit when using knah's UnHollower.
47. Fixed Execution Order of OnApplicationStart to run before any of the other Mod Methods.
48. Added Assertion Manager to handle MelonLoader Internals.
49. Fixed Issue with MelonCoroutines throwing a Cast Exception for knah's UnHollower.
50. Added Support for WaitForFixedUpdate and WaitForEndOfFrame to MelonCoroutines.
51. Fixed Issue with MelonCoroutines throwing a Missing Method Exception for knah's UnHollower.
52. Added 2019.3.6f1 Unity Dependencies.
53. Fixed Internal Failure issues with RotMG Exalt and Unity 2019.3.6f1.
54. Fixed Internal Failure issues with Pistol Whip and Unity 2019.2.0f1.
55. Fixed Issue with MelonLoader.ModHandler complaining about Mod Libraries.
56. Fixed Issue with MelonCoroutines processing a yield of null as the same as WaitForFixedUpdate.
57. Made MelonMod use directly invokable virtuals and overrides for faster invoking.  (As requested by knah :3)
58. Removed MelonModController from MelonLoader.ModHandler.
59. Made MelonLoader.ModHandler look for the MelonMod subclass in the Mod's MelonModInfoAttribute instead of searching the entire Assembly.  (As requested by knah :3)
60. Fixed Issue with Logger Imports in MelonLoader.ModHandler being passed garbage strings.

---

### v0.0.3:

1. General Stability and Optimization Improvements.
2. Added Support for Mono based Games.
3. Added MUPOT Mode which forces Il2Cpp games to run under Mono. This Mode is still extremely Experimental.
4. Logger has been renamed to MelonModLogger to prevent conflict with UnityEngine.Logger.
5. Fixed Crash issue when using System.Type in MUPOT Mode.
6. MelonModInfo Attribute has been changed to an Assembly Attribute.
7. Fixed Issue with MelonModLogger not getting the Mod Name when used outside of the main class.
8. FileInfo in the TestMod has been renamed to BuildInfo so it won't conflict with the System.IO.FileInfo class.
9. Added 0Harmony Integration and Support.
10. Modified MelonLoader.ModHandler to use a Component based system in MUPOT Mode and Mono based games.
11. Fixed Issue preventing OnUpdate from being called in MUPOT Mode and Mono based games.
12. Fixed Issue preventing OnApplicationQuit from being called in MUPOT Mode and Mono based games.
13. Fixed Conflict issue triggering Unity's Single Instance function in MUPOT Mode.
14. Added OnLevelWasLoaded, OnLevelWasInitialized, OnUpdate, OnFixedUpdate, OnLateUpdate, and OnGUI for Mods in MUPOT Mode and Mono based games.
15. Bug that prevented Debug Compilation is now fixed.
16. Fixed Issue that caused Unity Crash Handler to initialize MelonLoader twice.
17. Fixed Runtime Initialization issue with Audica that caused crashes.
18. Fixed Execution Order of MelonLoader.ModHandler.
19. Fixed Issue with Mono Game Support due to Mono DLL naming.
20. Fixed Issue with Internal Calls through MUPOT Mode.
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

### v0.0.2:

1. Added NET_SDK to be built into MelonLoader.ModHandler as the designated Il2Cpp Wrapper.
2. Fixed Issue with Mods and Logs folders not being in the correct place next to the game's exe.
3. Fixed Issue with the ModPrefs not calling the OnModSettingsApplied method of Mods.
4. Replaced old ModPrefs debug log message left in from my VRChat mod.
5. Fixed Issue with Audica crashing when trying to use MelonLoader.
6. Added Unity 2018.4.6f1 Dependencies.
7. Fixed bug with Launch Options.
8. Fixed Pathing issue that caused games like Boneworks to crash.
9. Fixed Issue where Environment.CurrentDirectory wasn't being set properly.
10. Clean-up and Additions made to NET_SDK.  (Thanks Sc4ad :D)
11. Added SteamVR and OculusVR Dependency DLLs to MelonLoader/Managed.
12. Added TestMod to Project Files.
13. Added Attribute StackTrace System to Logger.  (As requested by Camobiwon :3)
14. Replaced Spaces in Names for the Attribute StackTrace System with Underscores.
15. Fixed the bug that caused the Logger from MelonLoader.ModHandler to not properly output when using the Debug Console.
16. Added SystemTypeToIl2CppClass method to NET_SDK.Il2Cpp.
17. Crash Issue with Game.DontDestroyOnLoad has been fixed and it now works.
18. Fixed Il2Cpp_Field where SetValue does not function on a instance object.  (Thanks [DubyaDude](https://github.com/DubyaDude) ^.^)
19. Fixed Compile Version conflict with the 2 Consoles.  
MelonLoader [DEBUG] & MelonLoader.ModHandler [RELEASE]  =  Debug Console  
MelonLoader [RELEASE] & MelonLoader.ModHandler [DEBUG]  =  Normal Console  
MelonLoader [DEBUG] & MelonLoader.ModHandler [DEBUG]  =  Debug Console  
MelonLoader [RELEASE] & MelonLoader.ModHandler [RELEASE]  =  Left up to the Launch Options  

---

### v0.0.1:

1. Initial Commit and Rewrite.
