## What's Changed:
* Removed Unneeded AsmResolverFix
* Fixed an issue with Platform Specification being incorrect on Platforms other than Windows
* Fixed Incorrect BuildInfo Values
* Implemented Backwards Compatibility for MelonLoader.BuildInfo to use MelonLoader.Properties.BuildInfo
* Fixed ColorARGB File Name to match the Class
* Fixed an issue with capturing stdout / stderr when capture player logs is disabled   [[#962](<https://github.com/LavaGang/MelonLoader/pull/962>)]
* Added "." as a starting exclusion for Melon Folders
* Implemented Exception Logging for Mono Invokes   [[#939](<https://github.com/LavaGang/MelonLoader/pull/939>)]
* Reimplemented Il2CppInteropFixes
* Removed broken ClassInjector.SystemTypeFromIl2CppType Il2CppInterop fix
* Fixed an issue with Melon Attribute checks not gracefully failing when an Exception is Thrown
* Added AsmResolver.DotNet for Mono and MonoBleedingEdge Games
* Fixed an issue with MelonUtils.IsGameIl2Cpp sometimes returning incorrect values
* Reimplemented "manifest.json" Requirement for Recursive Melon Subfolder scanning
* Implemented Config Options for Subfolder Loading Customization
* Exposed "MelonFolderHandler" methods for Custom Melon Folder Exclusion
* Fixed an issue with empty strings in "PATH" Environment Variable causing crashes   [[#970](<https://github.com/LavaGang/MelonLoader/pull/970>)]
* Fixed an issue with exposed Melon Folder Exclusion API not working correctly for Plugins
* Fixed an issue with Melon Subfolder Parent Context checks using an incorrect ScanType
* Implemented MelonInfoAttribute.SystemType Validation to prevent infinite loop during Melon Instantiation
* Fixed an issue with Infinite Loops from RottenMelon Instantiation calling MelonAssembly.LoadMelonAssembly
* Updated Cpp2IL to 2022.1.0-pre-release.21   [[#1104](<https://github.com/LavaGang/MelonLoader/pull/1104)]
* Implemented Il2CppInterop GetFieldDefaultValue Fix to allow custom signatures to be added to Il2CppInterop's Class::GetFieldDefaultValue Hook
* Allowed exclamation marks in namespaces/types   [[#983](<https://github.com/LavaGang/MelonLoader/pull/983>)]
* Updated Tomlet to 6.2.0
* Updated Pastel to 7.0.1
* Updated AssetRipper.Primitives to 3.2.0
* Updated Il2CppInterop to 1.5.1
* Fixed an issue with Melon Subfolder toggle being inverted
* Fixed an issue with Il2CppAssemblyGenerator and File Detection Failures
* Fixed an issue with Assembly Verifier throwing an error when loading SharpDX
* Fixed an issue with MelonCoroutines attempting to use the Support Module before it is loaded
* Fixed an issue with MelonCoroutines started from OnApplicationStart running before other MelonCoroutines in the queue
* Fixed an issue with Il2CppInterop's GenericMethod::GetMethod Hook causing crashes on some Unity Versions
* Fixed an issue with AsmResolver's Utf8String.Concat using the wrong Length variable for byte array allocation
* Fixed an issue with Il2CppInterop's MethodRewriteContext.UnmangleMethodNameWithSignature not fully validating strings before passing them to Utf8String.Concat
* Fixed an issue with Mono Initialization not rethrowing exceptions to logging in some rare cases
* Implemented experimental MonoBleedingEdge Environment Patches behind Loader config option (Default is OFF)
* Fixed an issue with Harmony Auto-Patching for Plugins and Mods not gracefully failing
* Implemented Harmony TryPatchAll Extension to MelonUtils
* Rewrote Il2Cpp Scene Handling to Patch Internal_SceneLoaded and Internal_SceneUnloaded
* Fixed an issue with Windows Bootstrap not being compilable on Linux   [[#1032](<https://github.com/LavaGang/MelonLoader/pull/1032>)]
* Fixed an issue with Compilation Runtime Identifier being overridden by Compiling Operation System
* Fixed an issue with MelonLogger.MsgPastel not working as intended   [[#1041](<https://github.com/LavaGang/MelonLoader/pull/1041>)]
* Fixed an issue with Bootstrap's Runtime Symbol Redirect causing weird Span Marshal crashing
* Rewrote Bootstrap PLTHooks to utilize NativeHook overrides
* Reworked Bootstrap NativeFunc to avoid infinite looping resolves
* Improved Handling of Compatibility Layers and Unity Version Parsing
* Fixed an issue with Command-Line Parsing sometimes failing   [[#1044](<https://github.com/LavaGang/MelonLoader/pull/1044>)]
* Adjusted DotNet handling to initialize more reliably
* Implemented DotNet HostFXR Path Override config and launch options --melonloader.hostfxr
* Added missing Microsoft.Extensions.Logging packages
* Fixed an issue with Logging not validating Section   [[#1070](<https://github.com/LavaGang/MelonLoader/pull/1070>)]
* Updated AssetTools.Net to 3.0.4   [[#1071](<https://github.com/LavaGang/MelonLoader/pull/1071>)]
* Fixed Il2CppInterop RunFinalizer sometimes failing on certain Unity versions   [[#1075](<https://github.com/LavaGang/MelonLoader/pull/1075>)]
* Fixed compilation issues with MelonStartScreen   [[#1084](<https://github.com/LavaGang/MelonLoader/pull/1084>)]
* Fixed an issue with Wine console not resetting the colors after message output   [[#1085](<https://github.com/LavaGang/MelonLoader/pull/1085>)]
* Removed broken InstancePatchFix patches   [[#1106](<https://github.com/LavaGang/MelonLoader/pull/1106>)]

## Contributors:
* [slxdy](<https://github.com/slxdy>) made a contribution in [#939](<https://github.com/LavaGang/MelonLoader/pull/939>)
* [aldelaro5](<https://github.com/aldelaro5>) made a contribution in [#962](<https://github.com/LavaGang/MelonLoader/pull/962>)
* [Windows10CE](<https://github.com/Windows10CE>) made a contribution in [#1032](<https://github.com/LavaGang/MelonLoader/pull/1032>)
* [HAHOOS](<https://github.com/HAHOOS>) made a contribution in [#1041](<https://github.com/LavaGang/MelonLoader/pull/1041>)
* [Atmudia](<https://github.com/Atmudia>) made a contribution in [#1075](<https://github.com/LavaGang/MelonLoader/pull/1075>)
* [Squaduck](<https://github.com/Squaduck>) made their first contribution in [#970](<https://github.com/LavaGang/MelonLoader/pull/970>)
* [Emik03](<https://github.com/Emik03>) made their first contribution in [#983](<https://github.com/LavaGang/MelonLoader/pull/983>)
* [official-notfishvr](<https://github.com/official-notfishvr>) made their first contribution in [#1044](<https://github.com/LavaGang/MelonLoader/pull/1044>) & [#1084](<https://github.com/LavaGang/MelonLoader/pull/1084>)
* [MrModification](<https://github.com/MrModification>) made their first contribution in [#1070](<https://github.com/LavaGang/MelonLoader/pull/1070>)
* [dommrogers](<https://github.com/dommrogers>) made their first contribution in [#1071](<https://github.com/LavaGang/MelonLoader/pull/1071>)
* [bnfour](<https://github.com/bnfour>) made their first contribution in [#1085](<https://github.com/LavaGang/MelonLoader/pull/1085>)
* [IkariDevGIT](<https://github.com/IkariDevGIT>) made their first contribution in [#1104](<https://github.com/LavaGang/MelonLoader/pull/1104>)
* [RaptorRush135](<https://github.com/RaptorRush135>) made their first contribution in [#1106](<https://github.com/LavaGang/MelonLoader/pull/1106>)

**Full Changelog**: [CHANGELOG.md](<https://github.com/LavaGang/MelonLoader/blob/master/CHANGELOG.md>) | [v0.7.1...v0.7.2](<https://github.com/LavaGang/MelonLoader/compare/v0.7.1...v0.7.2>)