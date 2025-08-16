## What's Changed
* Removed Unneeded AsmResolverFix
* Fixed an issue with Platform Specification being incorrect on Platforms other than Windows
* Fixed Incorrect BuildInfo Values
* Implemented Backwards Compatibility for MelonLoader.BuildInfo to use MelonLoader.Properties.BuildInfo
* Fixed ColorARGB File Name to match the Class
* Fixed an issue with capturing stdout / stderr when capture player logs is disabled
* Added "." as a starting exclusion for Melon Folders
* Implemented Exception Logging for Mono Invokes
* Reimplemented Il2CppInteropFixes
* Removed broken ClassInjector.SystemTypeFromIl2CppType Il2CppInterop fix
* Fixed an issue with Melon Attribute checks not gracefully failing when an Exception is Thrown
* Added AsmResolver.DotNet for Mono and MonoBleedingEdge Games
* Fixed an issue with MelonUtils.IsGameIl2Cpp sometimes returning incorrect values
* Reimplemented "manifest.json" Requirement for Recursive Melon Subfolder scanning
* Implemented Config Options for Subfolder Loading Customization
* Exposed "MelonFolderHandler" methods for Custom Melon Folder Exclusion
* Fixed an issue with empty strings in "PATH" Environment Variable causing crashes
* Fixed an issue with exposed Melon Folder Exclusion API not working correctly for Plugins
* Fixed an issue with Melon Subfolder Parent Context checks using an incorrect ScanType
* Implemented MelonInfoAttribute.SystemType Validation to prevent infinite loop during Melon Instantiation
* Fixed an issue with Infinite Loops from RottenMelon Instantiation calling MelonAssembly.LoadMelonAssembly
* Updated Cpp2IL to 2022.1.0-pre-release.20
* Implemented Il2CppInterop GetFieldDefaultValue Fix
* Allowed exclamation marks in namespaces/types

## New Contributors
* [Squaduck](<https://github.com/Squaduck>) made their first contribution in [#970](<https://github.com/LavaGang/MelonLoader/pull/970>)
* [Emik03](<https://github.com/Emik03>) made their first contribution in [#983](<https://github.com/LavaGang/MelonLoader/pull/983>)

**Full Changelog**: [CHANGELOG.md](<https://github.com/LavaGang/MelonLoader/blob/master/CHANGELOG.md>) | [v0.7.1...v0.7.2](<https://github.com/LavaGang/MelonLoader/compare/v0.7.1...v0.7.2>)