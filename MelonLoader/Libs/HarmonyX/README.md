<p align="center">
	<img src="./logo.png" alt="HarmonyX Logo" height="128" />
</p>

<p align="center">
	<a href="https://www.nuget.org/packages/HarmonyX/">
		<img src="https://img.shields.io/nuget/dt/HarmonyX?label=NuGet&style=for-the-badge" alt="NuGet" />
	</a>
</p>

***

<p align="center">
	A library for patching, replacing and decorating .NET and Mono methods during runtime. Now powered by MonoMod.RuntimeDetour!
</p>

***

## About

**HarmonyX** is a fork of [Harmony 2](https://github.com/pardeike/Harmony) that specializes on support for games and game modding frameworks.

HarmonyX is being developed primarily for use in game frameworks alongside MonoMod. The main target usage of HarmonyX is [BepInEx](https://github.com/BepInEx/BepInEx) and Unity.

Important aspects of HarmonyX include:

* Unity support first: builds for .NET Framework 3.5 and .NET Standard 2.0
* Patching feature parity with Harmony while reducing code duplication with MonoMod
* Full interop with [MonoMod.RuntimeDetour](https://github.com/MonoMod/MonoMod/blob/master/README-RuntimeDetour.md): patches made with either can coexist
* Easily extendable patching: [built-in support for native method patching](https://github.com/BepInEx/HarmonyX/wiki/Valid-patch-targets#native-methods-marked-extern) and possibility to extend to other patch targets (e.g. IL2CPP)
* Fixes, changes and optimizations aimed at Unity modding

HarmonyX is powered by [MonoMod](https://github.com/MonoMod) and its runtime patching tools.

## Documentation

Check the documentation out at [HarmonyX wiki](https://github.com/BepInEx/HarmonyX/wiki).
