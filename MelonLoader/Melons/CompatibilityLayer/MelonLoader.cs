using System;
using System.Collections.Generic;
using System.Reflection;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayer
{
	internal class MelonLoader : MelonCompatibilityLayerResolver
	{
		internal static void TryResolve(object sender, MelonCompatibilityLayerResolverEventArgs args)
		{
			if (args.inter != null)
				return;
			bool isMelonLoader = false;
			foreach (Type type in args.assembly.GetTypes())
				if (type.IsSubclassOf(typeof(MelonMod))
					|| type.IsSubclassOf(typeof(MelonPlugin))
					|| type.IsSubclassOf(typeof(MelonBase)))
                {
					isMelonLoader = true;
					break;
                }
			if (!isMelonLoader)
				return;
			args.inter = new MelonLoader();
		}

		internal override bool CheckAndCreate(Assembly asm, string filelocation, bool is_plugin, ref MelonBase baseInstance)
		{
			MelonInfoAttribute infoAttribute = null;
			if (!CheckInfoAttribute(asm, filelocation, is_plugin, ref infoAttribute))
				return false;

			List<MelonGameAttribute> gameAttributes = null;
			if (!CheckGameAttributes(asm, filelocation, is_plugin, ref gameAttributes))
				return false;

			if (!CheckPlatformAttribute(asm, filelocation, is_plugin))
				return false;

			if (!CheckPlatformDomainAttribute(asm, filelocation, is_plugin))
				return false;

			if (!CheckVerifyLoaderVersionAttribute(asm, filelocation, is_plugin))
				return false;

			if (!CheckVerifyLoaderBuildAttribute(asm, filelocation, is_plugin))
				return false;

			baseInstance = Activator.CreateInstance(infoAttribute.SystemType) as MelonBase;
			if (baseInstance == null)
			{
				MelonLogger.Error($"Failed to Create Instance for {filelocation}");
				return false;
			}

			baseInstance.Info = infoAttribute;
			baseInstance.Games = gameAttributes.ToArray();

			MelonColorAttribute coloratt = MelonHandler.PullCustomAttributeFromAssembly<MelonColorAttribute>(asm);
			baseInstance.ConsoleColor = ((coloratt == null) ? MelonLogger.DefaultMelonColor : coloratt.Color);

			MelonPriorityAttribute priorityatt = MelonHandler.PullCustomAttributeFromAssembly<MelonPriorityAttribute>(asm);
			baseInstance.Priority = ((priorityatt == null) ? 0 : priorityatt.Priority);

			baseInstance.OptionalDependencies = MelonHandler.PullCustomAttributeFromAssembly<MelonOptionalDependenciesAttribute>(asm);
			baseInstance.Location = filelocation;
			baseInstance.Assembly = asm;
			baseInstance.Harmony = Harmony.HarmonyInstance.Create(asm.FullName);

			return true;
		}

		private static bool CheckInfoAttribute(Assembly asm, string filelocation, bool is_plugin, ref MelonInfoAttribute infoAttribute)
		{
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
			bool nullcheck_author = string.IsNullOrEmpty(infoAttribute.Author);
			if (nullcheck_name || nullcheck_version || nullcheck_author)
			{
				MelonLogger.Error($"No {(nullcheck_name ? "Name" : (nullcheck_version ? "Version" : (nullcheck_author ? "Author" : "")))} given to MelonInfoAttribute in {filelocation}");
				return false;
			}

			return true;
		}

		private static bool CheckGameAttributes(Assembly asm, string filelocation, bool is_plugin, ref List<MelonGameAttribute> gameAttributes)
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
					MelonLogger.Error($"Incompatible Game for {(is_plugin ? "Plugin" : "Mod")} {filelocation}");
					return false;
				}
			}

			return true;
		}

		private static bool CheckPlatformAttribute(Assembly asm, string filelocation, bool is_plugin)
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
				MelonLogger.Error($"Incompatible Platform for {(is_plugin ? "Plugin" : "Mod")} {filelocation}");
				return false;
			}
			return true;
		}

		private static bool CheckPlatformDomainAttribute(Assembly asm, string filelocation, bool is_plugin)
		{
			MelonPlatformDomainAttribute platformDomainAttribute = MelonHandler.PullCustomAttributeFromAssembly<MelonPlatformDomainAttribute>(asm);
			if ((platformDomainAttribute == null)
				|| (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.UNIVERSAL))
				return true;
			bool is_il2cpp_expected_mono = (MelonUtils.IsGameIl2Cpp() && (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.MONO));
			bool is_mono_expected_il2cpp = (!MelonUtils.IsGameIl2Cpp() && (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP));
			if (is_il2cpp_expected_mono || is_mono_expected_il2cpp)
			{
				MelonLogger.Error($"Incompatible Platform Domain for {(is_plugin ? "Plugin" : "Mod")} {filelocation}");
				return false;
			}
			return true;
		}

		private static int[] CurrentMLVersionIntArr = null;
		private static bool CheckVerifyLoaderVersionAttribute(Assembly asm, string filelocation, bool is_plugin)
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
				MelonLogger.Error($"Incompatible MelonLoader Version for {(is_plugin ? "Plugin" : "Mod")} {filelocation}");
				return false;
			}

			return true;
		}

		private static bool CheckVerifyLoaderBuildAttribute(Assembly asm, string filelocation, bool is_plugin)
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
				MelonLogger.Error($"Incompatible MelonLoader Build for {(is_plugin ? "Plugin" : "Mod")} {filelocation}");
				return false;
			}
			return true;
		}
	}
}