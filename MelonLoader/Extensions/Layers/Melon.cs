using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
	internal class Melon_CL : MelonCompatibilityLayer.Resolver
	{
		private readonly Assembly asm = null;
		private readonly string filepath = null;
		private bool is_plugin = false;
		private Melon_CL(Assembly assembly, string filelocation) { asm = assembly; filepath = filelocation; }

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

			IEnumerable<Type> melon_types = args.assembly.GetOnlyValidTypes().Where(x => x.IsSubclassOf(typeof(MelonBase)));
			if ((melon_types == null)
				|| (melon_types.Count() <= 0))
				return;

			if (string.IsNullOrEmpty(args.filepath))
				args.filepath = args.assembly.GetName().Name;
			args.inter = new Melon_CL(args.assembly, args.filepath);
		}

		private static void RefreshPluginsTable() => Main.Plugins = MelonHandler._Plugins;
		private static void RefreshModsTable() => Main.Mods = MelonHandler._Mods;

		public override void CheckAndCreate(ref List<MelonBase> melonTbl)
		{
			MelonInfoAttribute infoAttribute = null;
			if (!CheckInfoAttribute(ref infoAttribute))
				return;

			List<MelonGameAttribute> gameAttributes = null;
			if (!CheckGameAttributes(ref gameAttributes))
				return;

			if (!CheckPlatformAttribute())
				return;

			if (!CheckPlatformDomainAttribute())
				return;

			if (!CheckVerifyLoaderVersionAttribute())
				return;

			if (!CheckVerifyLoaderBuildAttribute())
				return;

			MelonColorAttribute coloratt = MelonUtils.PullAttributeFromAssembly<MelonColorAttribute>(asm);
			MelonPriorityAttribute priorityatt = MelonUtils.PullAttributeFromAssembly<MelonPriorityAttribute>(asm, true);

			MelonBase instance = MelonCompatibilityLayer.CreateMelonFromWrapperData(new MelonCompatibilityLayer.WrapperData()
			{
				Assembly = asm,
				Info = infoAttribute,
				Games = gameAttributes.ToArray(),
				OptionalDependencies = MelonUtils.PullAttributeFromAssembly<MelonOptionalDependenciesAttribute>(asm),
				ConsoleColor = (coloratt == null) ? MelonLogger.DefaultMelonColor : coloratt.Color,
				Priority = (priorityatt == null) ? 0 : priorityatt.Priority,
				Location = filepath
			});
			if (instance == null)
				return;

			melonTbl.Add(instance);
		}

		private bool CheckInfoAttribute(ref MelonInfoAttribute infoAttribute)
		{
			infoAttribute = MelonUtils.PullAttributeFromAssembly<MelonInfoAttribute>(asm);

			// Legacy Support
			if (infoAttribute == null)
				infoAttribute = MelonUtils.PullAttributeFromAssembly<MelonModInfoAttribute>(asm)?.Convert();
			if (infoAttribute == null)
				infoAttribute = MelonUtils.PullAttributeFromAssembly<MelonPluginInfoAttribute>(asm)?.Convert();

			if ((infoAttribute == null) || (infoAttribute.SystemType == null))
			{
				MelonLogger.Error($"No {((infoAttribute == null) ? "MelonInfoAttribute Found" : "Type given to MelonInfoAttribute")} in {filepath}");
				return false;
			}

			is_plugin = infoAttribute.SystemType.IsSubclassOf(typeof(MelonPlugin));
			bool is_mod_subclass = infoAttribute.SystemType.IsSubclassOf(typeof(MelonMod));
			if (!is_plugin && !is_mod_subclass)
			{
				MelonLogger.Error($"Type Specified {infoAttribute.SystemType.AssemblyQualifiedName} is not a Subclass of MelonPlugin or MelonMod in {filepath}");
				return false;
			}

			bool nullcheck_name = string.IsNullOrEmpty(infoAttribute.Name);
			bool nullcheck_version = string.IsNullOrEmpty(infoAttribute.Version);
			if (nullcheck_name || nullcheck_version)
			{
				MelonLogger.Error($"No {(nullcheck_name ? "Name" : (nullcheck_version ? "Version" : ""))} given to MelonInfoAttribute in {filepath}");
				return false;
			}

			if (is_plugin 
				? MelonHandler.IsPluginAlreadyLoaded(infoAttribute.Name)
				: MelonHandler.IsModAlreadyLoaded(infoAttribute.Name))
			{
				MelonLogger.Error($"Duplicate {(is_plugin ? "Plugin" : "Mod")} {infoAttribute.Name}: {filepath}");
				return false;
			}

			return true;
		}

		private bool CheckGameAttributes(ref List<MelonGameAttribute> gameAttributes)
		{
			gameAttributes = new List<MelonGameAttribute>();
			MelonGameAttribute[] gameatt = MelonUtils.PullAttributesFromAssembly<MelonGameAttribute>(asm);
			if ((gameatt != null) && (gameatt.Length > 0))
				gameAttributes.AddRange(gameatt);

			// Legacy Support
			MelonModGameAttribute[] legacymodgameAttributes = MelonUtils.PullAttributesFromAssembly<MelonModGameAttribute>(asm);
			if ((legacymodgameAttributes != null) && (legacymodgameAttributes.Length > 0))
				foreach (MelonModGameAttribute legacyatt in legacymodgameAttributes)
					gameAttributes.Add(legacyatt.Convert());
			MelonPluginGameAttribute[] legacyplugingameAttributes = MelonUtils.PullAttributesFromAssembly<MelonPluginGameAttribute>(asm);
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
					MelonLogger.Error($"Incompatible Game for {(is_plugin ? "Plugin" : "Mod")}: {filepath}");
					return false;
				}
			}

			return true;
		}

		private bool CheckPlatformAttribute()
		{
			MelonPlatformAttribute platformAttribute = MelonUtils.PullAttributeFromAssembly<MelonPlatformAttribute>(asm);
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
				MelonLogger.Error($"Incompatible Platform for {(is_plugin ? "Plugin" : "Mod")}: {filepath}");
				return false;
			}
			return true;
		}

		private bool CheckPlatformDomainAttribute()
		{
			MelonPlatformDomainAttribute platformDomainAttribute = MelonUtils.PullAttributeFromAssembly<MelonPlatformDomainAttribute>(asm);
			if ((platformDomainAttribute == null)
				|| (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.UNIVERSAL))
				return true;
			bool is_il2cpp_expected_mono = (MelonUtils.IsGameIl2Cpp() && (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.MONO));
			bool is_mono_expected_il2cpp = (!MelonUtils.IsGameIl2Cpp() && (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP));
			if (is_il2cpp_expected_mono || is_mono_expected_il2cpp)
			{
				MelonLogger.Error($"Incompatible Platform Domain for {(is_plugin ? "Plugin" : "Mod")}: {filepath}");
				return false;
			}
			return true;
		}

		private static int[] CurrentMLVersionIntArr = null;
		private bool CheckVerifyLoaderVersionAttribute()
		{
			VerifyLoaderVersionAttribute verifyLoaderVersionAttribute = MelonUtils.PullAttributeFromAssembly<VerifyLoaderVersionAttribute>(asm);
			if (verifyLoaderVersionAttribute == null)
				return true;

			if (CurrentMLVersionIntArr == null)
			{
				string[] versionArgs = BuildInfo.Version.Split('.');
				CurrentMLVersionIntArr = new int[4];
				CurrentMLVersionIntArr[0] = int.Parse(versionArgs[0]);
				CurrentMLVersionIntArr[1] = int.Parse(versionArgs[1]);
				CurrentMLVersionIntArr[2] = int.Parse(versionArgs[2]);
				CurrentMLVersionIntArr[3] = ((versionArgs.Length == 4) && !string.IsNullOrEmpty(versionArgs[3])) ? int.Parse(versionArgs[3]) : 0;
			}

			bool is_minimum = verifyLoaderVersionAttribute.IsMinimum;

			bool major_is_acceptable = is_minimum
				? (verifyLoaderVersionAttribute.Major <= CurrentMLVersionIntArr[0])
				: (verifyLoaderVersionAttribute.Major == CurrentMLVersionIntArr[0]);

			bool minor_is_acceptable = is_minimum
				? (verifyLoaderVersionAttribute.Minor <= CurrentMLVersionIntArr[1])
				: (verifyLoaderVersionAttribute.Minor == CurrentMLVersionIntArr[1]);

			bool patch_is_acceptable = is_minimum
				? (verifyLoaderVersionAttribute.Patch <= CurrentMLVersionIntArr[2])
				: (verifyLoaderVersionAttribute.Patch == CurrentMLVersionIntArr[2]);

			bool revision_is_acceptable = is_minimum
				? (verifyLoaderVersionAttribute.Revision <= CurrentMLVersionIntArr[3])
				: (verifyLoaderVersionAttribute.Revision == CurrentMLVersionIntArr[3]);

			if (!major_is_acceptable
				|| !minor_is_acceptable
				|| !patch_is_acceptable
				|| !revision_is_acceptable)
			{
				MelonLogger.Error($"Incompatible MelonLoader Version for {(is_plugin ? "Plugin" : "Mod")}: {filepath}");
				return false;
			}

			return true;
		}

		private bool CheckVerifyLoaderBuildAttribute()
		{
			VerifyLoaderBuildAttribute verifyLoaderBuildAttribute = MelonUtils.PullAttributeFromAssembly<VerifyLoaderBuildAttribute>(asm);
			if ((verifyLoaderBuildAttribute == null)
				|| string.IsNullOrEmpty(verifyLoaderBuildAttribute.HashCode))
				return true;
			string currentHashCode = MelonUtils.HashCode;
			if (string.IsNullOrEmpty(currentHashCode))
				return true;
			if (!currentHashCode.Equals(verifyLoaderBuildAttribute.HashCode))
			{
				MelonLogger.Error($"Incompatible MelonLoader Build for {(is_plugin ? "Plugin" : "Mod")}: {filepath}");
				return false;
			}
			return true;
		}
	}
}