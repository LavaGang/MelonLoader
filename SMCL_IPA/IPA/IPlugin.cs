using System;

namespace IllusionPlugin
{
	[Obsolete("IllusionPlugin.IPlugin is Only Here for Compatibility Reasons. Please use MelonPlugin or MelonMod instead.")]
	public interface IPlugin
	{
		[Obsolete("IllusionPlugin.IPlugin.Name is Only Here for Compatibility Reasons. Please use MelonPlugin.Info.Name or MelonMod.Info.Name instead.")]
		string Name { get; }
		[Obsolete("IllusionPlugin.IPlugin.Version is Only Here for Compatibility Reasons. Please use MelonPlugin.Info.Version or MelonMod.Info.Version instead.")]
		string Version { get; }
		[Obsolete("IllusionPlugin.IPlugin.OnApplicationStart() is Only Here for Compatibility Reasons. Please use MelonBase.OnApplicationStart() instead.")]
		void OnApplicationStart();
		[Obsolete("IllusionPlugin.IPlugin.OnApplicationQuit() is Only Here for Compatibility Reasons. Please use MelonBase.OnApplicationQuit() or MelonMod.OnApplicationQuit() instead.")]
		void OnApplicationQuit();
		[Obsolete("IllusionPlugin.IPlugin.OnLevelWasLoaded(int) is Only Here for Compatibility Reasons. Please use MelonMod.OnSceneWasLoaded(int, string) instead.")]
		void OnLevelWasLoaded(int level);
		[Obsolete("IllusionPlugin.IPlugin.OnLevelWasInitialized(int) is Only Here for Compatibility Reasons. Please use MelonMod.OnSceneWasInitialized(int, string) instead.")]
		void OnLevelWasInitialized(int level);
		[Obsolete("IllusionPlugin.IPlugin.OnUpdate() is Only Here for Compatibility Reasons. Please use MelonBase.OnUpdate() instead.")]
		void OnUpdate();
		[Obsolete("IllusionPlugin.IPlugin.OnFixedUpdate() is Only Here for Compatibility Reasons. Please use MelonMod.OnFixedUpdate() instead.")]
		void OnFixedUpdate();
	}
}
