## What's Changed:
* Bumped Bootstrap .NET version to 10
* Improved RemoteAPI outage handling to report one warning instead of an error per host and retain cached Il2Cpp generation settings
* Fixed an issue with stale Cpp2IL output being reused for assembly generation
* Fixed an issue with Logging sometimes deleting the newest log instead of the oldest in rotation
* Changed Portable Dotnet Handling to download from official Microsoft Sources instead
* Fixed an issue with Melon Callback Registry not failing gracefully
* Implemented --cpp2il.keepoutput launch option for keeping Cpp2IL output after generation is attempted

## Contributors:
* [ds5678](<https://github.com/ds5678>) made a contribution in [#1167](<https://github.com/LavaGang/MelonLoader/pull/1167>)
* [batmanwarrior](<https://github.com/batmanwarrior>) made a contribution in [#1186](<https://github.com/LavaGang/MelonLoader/pull/1186>)
* [ifBars](https://github.com/ifBars) made a contribution in [#1193](<https://github.com/LavaGang/MelonLoader/pull/1193>)
* [JesseWV](https://github.com/JesseWV) made a contribution in [#1194](<https://github.com/LavaGang/MelonLoader/pull/1194>)
* [ObjectInSpace](https://github.com/ObjectInSpace) made a contribution in [#1195](<https://github.com/LavaGang/MelonLoader/pull/1195>)

**Full Changelog**: [CHANGELOG.md](<https://github.com/LavaGang/MelonLoader/blob/master/CHANGELOG.md>) | [v0.7.3...v0.7.4](<https://github.com/LavaGang/MelonLoader/compare/v0.7.3...v0.7.4>)
