using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
	internal class Melon_CL : MelonCompatibilityLayer.Resolver
	{
		private readonly Type[] melon_types = null;
		private readonly Assembly asm = null;
		private Melon_CL(Assembly assembly, IEnumerable<Type> types) { asm = assembly; melon_types = Enumerable.ToArray(types); }

		internal static void Setup(AppDomain domain)
		{
			domain.AssemblyResolve += (sender, args) =>
				(args.Name.StartsWith("MelonLoader.ModHandler, Version=")
				|| args.Name.StartsWith("MelonLoader, Version="))
				? typeof(Melon_CL).Assembly
				: null;
			MelonCompatibilityLayer.AddResolveAssemblyToLayerResolverEvent(ResolveAssemblyToLayerResolver);
			MelonCompatibilityLayer.AddRefreshPluginsTableEvent(RefreshPluginsTable);
			MelonCompatibilityLayer.AddRefreshModsTableEvent(RefreshModsTable);
		}

		private static void ResolveAssemblyToLayerResolver(MelonCompatibilityLayer.LayerResolveEventArgs args)
		{
			if (args.inter != null)
				return;
			IEnumerable<Type> melon_types = args.assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(MelonBase)));
			if ((melon_types == null)
				|| (melon_types.Count() <= 0))
				return;
			args.inter = new Melon_CL(args.assembly, melon_types);
		}

		private static void RefreshPluginsTable() => Main.Plugins = MelonHandler._Plugins;
		private static void RefreshModsTable() => Main.Mods = MelonHandler._Mods;

		public override void CheckAndCreate(string filelocation, bool is_plugin, ref List<MelonBase> melonTbl)
		{
			MelonInfoAttribute infoAttribute = null;
			if (!CheckInfoAttribute(filelocation, is_plugin, ref infoAttribute))
				return;

			List<MelonGameAttribute> gameAttributes = null;
			if (!CheckGameAttributes(filelocation, is_plugin, ref gameAttributes))
				return;

			if (!CheckPlatformAttribute(filelocation, is_plugin))
				return;

			if (!CheckPlatformDomainAttribute(filelocation, is_plugin))
				return;

			if (!CheckVerifyLoaderVersionAttribute(filelocation, is_plugin))
				return;

			if (!CheckVerifyLoaderBuildAttribute(filelocation, is_plugin))
				return;

			MelonColorAttribute coloratt = MelonHandler.PullCustomAttributeFromAssembly<MelonColorAttribute>(asm);
			MelonPriorityAttribute priorityatt = MelonHandler.PullCustomAttributeFromAssembly<MelonPriorityAttribute>(asm);

			MelonBase instance = MelonCompatibilityLayer.CreateMelonFromWrapperData(new MelonCompatibilityLayer.WrapperData()
			{
				Assembly = asm,
				Info = infoAttribute,
				Games = gameAttributes.ToArray(),
				OptionalDependencies = MelonHandler.PullCustomAttributeFromAssembly<MelonOptionalDependenciesAttribute>(asm),
				ConsoleColor = (coloratt == null) ? MelonLogger.DefaultMelonColor : coloratt.Color,
				Priority = (priorityatt == null) ? 0 : priorityatt.Priority,
				Location = filelocation
			});
			if (instance == null)
				return;

			melonTbl.Add(instance);
		}

		private bool CheckInfoAttribute(string filelocation, bool is_plugin, ref MelonInfoAttribute infoAttribute)
		{
			if (string.IsNullOrEmpty(filelocation))
				filelocation = asm.GetName().Name;

			infoAttribute = MelonHandler.PullCustomAttributeFromAssembly<MelonInfoAttribute>(asm);

			// Legacy Support
			if (infoAttribute == null)
				infoAttribute = MelonHandler.PullCustomAttributeFromAssembly<MelonModInfoAttribute>(asm)?.Convert();
			if (infoAttribute == null)
				infoAttribute = MelonHandler.PullCustomAttributeFromAssembly<MelonPluginInfoAttribute>(asm)?.Convert();

			if ((infoAttribute == null) || (infoAttribute.SystemType == null))
			{
				MelonLogger.Error($"No {((infoAttribute == null) ? "MelonInfoAttribute Found" : "Type given to MelonInfoAttribute")} in {filelocation}");
				return false;
			}

			bool is_plugin_subclass = infoAttribute.SystemType.IsSubclassOf(typeof(MelonPlugin));
			bool is_mod_subclass = infoAttribute.SystemType.IsSubclassOf(typeof(MelonMod));
			if (!is_plugin_subclass && !is_mod_subclass)
			{
				MelonLogger.Error($"Type Specified {infoAttribute.SystemType.AssemblyQualifiedName} is not a Subclass of MelonPlugin or MelonMod in {filelocation}");
				return false;
			}

			bool plugin_expected_got_mod = (is_plugin && is_mod_subclass);
			bool mod_expected_got_plugin = (!is_plugin && is_plugin_subclass);
			if (plugin_expected_got_mod || mod_expected_got_plugin)
			{
				MelonLogger.Error($"{(plugin_expected_got_mod ? "Plugin" : "Mod")} Expected, Got {(plugin_expected_got_mod ? "Mod" : "Plugin")} {infoAttribute.SystemType.AssemblyQualifiedName} in {filelocation}");
				return false;
			}

			bool nullcheck_name = string.IsNullOrEmpty(infoAttribute.Name);
			bool nullcheck_version = string.IsNullOrEmpty(infoAttribute.Version);
			if (nullcheck_name || nullcheck_version)
			{
				MelonLogger.Error($"No {(nullcheck_name ? "Name" : (nullcheck_version ? "Version" : ""))} given to MelonInfoAttribute in {filelocation}");
				return false;
			}

			bool isAlreadyLoaded = (is_plugin ? MelonHandler.IsPluginAlreadyLoaded(infoAttribute.Name) : MelonHandler.IsModAlreadyLoaded(infoAttribute.Name));
			if (isAlreadyLoaded)
			{
				MelonLogger.Error($"Duplicate {(is_plugin ? "Plugin" : "Mod")} {infoAttribute.Name}: {filelocation}");
				return false;
			}

			return true;
		}

		private bool CheckGameAttributes(string filelocation, bool is_plugin, ref List<MelonGameAttribute> gameAttributes)
		{
			gameAttributes = new List<MelonGameAttribute>();
			MelonGameAttribute[] gameatt = MelonHandler.PullCustomAttributesFromAssembly<MelonGameAttribute>(asm);
			if ((gameatt != null) && (gameatt.Length > 0))
				gameAttributes.AddRange(gameatt);

			// Legacy Support
			MelonModGameAttribute[] legacymodgameAttributes = MelonHandler.PullCustomAttributesFromAssembly<MelonModGameAttribute>(asm);
			if ((legacymodgameAttributes != null) && (legacymodgameAttributes.Length > 0))
				foreach (MelonModGameAttribute legacyatt in legacymodgameAttributes)
					gameAttributes.Add(legacyatt.Convert());
			MelonPluginGameAttribute[] legacyplugingameAttributes = MelonHandler.PullCustomAttributesFromAssembly<MelonPluginGameAttribute>(asm);
			if ((legacyplugingameAttributes != null) && (legacyplugingameAttributes.Length > 0))
				foreach (MelonPluginGameAttribute legacyatt in legacyplugingameAttributes)
					gameAttributes.Add(legacyatt.Convert());

			if (!MelonUtils.CurrentGameAttribute.Universal && (gameAttributes.Count > 0))
			{
				bool is_compatible = false;
				for (int i = 0; i < gameAttributes.Count; i++)
				{
					MelonGameAttribute melonGameAttribute = gameAttributes[i];
					if (melonGameAttribute == null)
						continue;
					if (melonGameAttribute.Universal || MelonUtils.CurrentGameAttribute.IsCompatible(melonGameAttribute))
					{
						is_compatible = true;
						break;
					}
				}
				if (!is_compatible)
				{
					MelonLogger.Error($"Incompatible Game for {(is_plugin ? "Plugin" : "Mod")}: {filelocation}");
					return false;
				}
			}

			return true;
		}

		private bool CheckPlatformAttribute(string filelocation, bool is_plugin)
		{
			MelonPlatformAttribute platformAttribute = MelonHandler.PullCustomAttributeFromAssembly<MelonPlatformAttribute>(asm);
			if ((platformAttribute == null)
				|| (platformAttribute.Platforms == null)
				|| (platformAttribute.Platforms.Length <= 0))
				return true;
			bool is_compatible = false;
			for (int i = 0; i < platformAttribute.Platforms.Length; i++)
			{
				MelonPlatformAttribute.CompatiblePlatforms platform = platformAttribute.Platforms[i];
				if ((platform == MelonPlatformAttribute.CompatiblePlatforms.UNIVERSAL)
					|| (MelonUtils.IsGame32Bit() && (platform == MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X86))
					|| (!MelonUtils.IsGame32Bit() && (platform == MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)))
				{
					is_compatible = true;
					break;
				}
			}
			if (!is_compatible)
			{
				MelonLogger.Error($"Incompatible Platform for {(is_plugin ? "Plugin" : "Mod")}: {filelocation}");
				return false;
			}
			return true;
		}

		private bool CheckPlatformDomainAttribute(string filelocation, bool is_plugin)
		{
			MelonPlatformDomainAttribute platformDomainAttribute = MelonHandler.PullCustomAttributeFromAssembly<MelonPlatformDomainAttribute>(asm);
			if ((platformDomainAttribute == null)
				|| (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.UNIVERSAL))
				return true;
			bool is_il2cpp_expected_mono = (MelonUtils.IsGameIl2Cpp() && (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.MONO));
			bool is_mono_expected_il2cpp = (!MelonUtils.IsGameIl2Cpp() && (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP));
			if (is_il2cpp_expected_mono || is_mono_expected_il2cpp)
			{
				MelonLogger.Error($"Incompatible Platform Domain for {(is_plugin ? "Plugin" : "Mod")}: {filelocation}");
				return false;
			}
			return true;
		}

		private static int[] CurrentMLVersionIntArr = null;
		private bool CheckVerifyLoaderVersionAttribute(string filelocation, bool is_plugin)
		{
			VerifyLoaderVersionAttribute verifyLoaderVersionAttribute = MelonHandler.PullCustomAttributeFromAssembly<VerifyLoaderVersionAttribute>(asm);
			if (verifyLoaderVersionAttribute == null)
				return true;

			if (CurrentMLVersionIntArr == null)
			{
				string[] versionArgs = BuildInfo.Version.Split('.');
				CurrentMLVersionIntArr = new int[4];
				CurrentMLVersionIntArr[0] = int.Parse(versionArgs[0]);
				CurrentMLVersionIntArr[1] = int.Parse(versionArgs[1]);
				CurrentMLVersionIntArr[2] = int.Parse(versionArgs[2]);
				CurrentMLVersionIntArr[3] = int.Parse(versionArgs[3]);
			}

			bool major_is_acceptable = ((verifyLoaderVersionAttribute.Major == CurrentMLVersionIntArr[0])
				|| (verifyLoaderVersionAttribute.IsMinimum
					&& (CurrentMLVersionIntArr[0] > verifyLoaderVersionAttribute.Major)));

			bool minor_is_acceptable = ((verifyLoaderVersionAttribute.Minor == CurrentMLVersionIntArr[1])
				|| (verifyLoaderVersionAttribute.IsMinimum
					&& (CurrentMLVersionIntArr[1] > verifyLoaderVersionAttribute.Minor)));

			bool revision_is_acceptable = ((verifyLoaderVersionAttribute.Revision == CurrentMLVersionIntArr[2])
				|| (verifyLoaderVersionAttribute.IsMinimum
					&& (CurrentMLVersionIntArr[2] > verifyLoaderVersionAttribute.Revision)));

			bool patch_is_acceptable = ((verifyLoaderVersionAttribute.Patch == CurrentMLVersionIntArr[3])
				|| (verifyLoaderVersionAttribute.IsMinimum
					&& (CurrentMLVersionIntArr[3] > verifyLoaderVersionAttribute.Patch)));

			if (!major_is_acceptable || !minor_is_acceptable || !revision_is_acceptable || !patch_is_acceptable)
			{
				MelonLogger.Error($"Incompatible MelonLoader Version for {(is_plugin ? "Plugin" : "Mod")}: {filelocation}");
				return false;
			}

			return true;
		}

		private bool CheckVerifyLoaderBuildAttribute(string filelocation, bool is_plugin)
		{
			VerifyLoaderBuildAttribute verifyLoaderBuildAttribute = MelonHandler.PullCustomAttributeFromAssembly<VerifyLoaderBuildAttribute>(asm);
			if ((verifyLoaderBuildAttribute == null)
				|| string.IsNullOrEmpty(verifyLoaderBuildAttribute.HashCode))
				return true;
			string currentHashCode = MelonUtils.HashCode;
			if (string.IsNullOrEmpty(currentHashCode))
				return true;
			if (!currentHashCode.Equals(verifyLoaderBuildAttribute.HashCode))
			{
				MelonLogger.Error($"Incompatible MelonLoader Build for {(is_plugin ? "Plugin" : "Mod")}: {filelocation}");
				return false;
			}
			return true;
		}
	}
}