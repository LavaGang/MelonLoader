using Semver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
    internal class Melon_Resolver : MelonCompatibilityLayer.Resolver
    {
        private bool is_plugin = false;

        private Melon_Resolver(Assembly assembly, string filepath) : base(assembly, filepath) { }

        internal static void Setup()
        {
            MelonCompatibilityLayer.AddAssemblyToResolverEvent(GetResolverFromAssembly);
            MelonCompatibilityLayer.AddRefreshPluginsEvent(RefreshPlugins);
            MelonCompatibilityLayer.AddRefreshModsEvent(RefreshMods);
        }

        private static MelonCompatibilityLayer.Resolver GetResolverFromAssembly(Assembly assembly, string filepath)
        {
            IEnumerable<Type> melon_types = assembly.GetValidTypes(x => x.IsSubclassOf(typeof(MelonBase)));
            if ((melon_types == null) || !melon_types.Any())
                return null;

            if (string.IsNullOrEmpty(filepath))
                filepath = assembly.GetName().Name;
            return new Melon_Resolver(assembly, filepath);
        }

        private static void RefreshPlugins() => Main.Plugins = MelonHandler.Plugins;
        private static void RefreshMods() => Main.Mods = MelonHandler.Mods;

        public override void CheckAndCreate(ref List<MelonBase> melonTbl)
        {
            MelonInfoAttribute infoAttribute = null;
            if (!CheckInfoAttribute(ref infoAttribute))
                return;

            List<MelonGameAttribute> gameAttributes = null;
            if (!CheckGameAttributes(ref gameAttributes))
                return;

            if (!CheckProcessAttributes())
                return;

            if (!CheckGameVersionAttribute())
                return;

            if (!CheckPlatformAttribute())
                return;

            if (!CheckPlatformDomainAttribute())
                return;

            if (!CheckVerifyLoaderVersionAttribute())
                return;

            if (!CheckVerifyLoaderBuildAttribute())
                return;

            MelonColorAttribute coloratt = MelonUtils.PullAttributeFromAssembly<MelonColorAttribute>(Assembly);
            MelonAuthorColorAttribute authorcoloratt = MelonUtils.PullAttributeFromAssembly<MelonAuthorColorAttribute>(Assembly);
            MelonPriorityAttribute priorityatt = MelonUtils.PullAttributeFromAssembly<MelonPriorityAttribute>(Assembly, true);
            MelonIDAttribute idatt = MelonUtils.PullAttributeFromAssembly<MelonIDAttribute>(Assembly, true);

            MelonBase instance = new MelonCompatibilityLayer.WrapperData()
            {
                Assembly = Assembly,
                Info = infoAttribute,
                Games = gameAttributes.ToArray(),
                OptionalDependencies = MelonUtils.PullAttributeFromAssembly<MelonOptionalDependenciesAttribute>(Assembly),
                ConsoleColor = (coloratt == null) ? MelonLogger.DefaultMelonColor : coloratt.Color,
                AuthorConsoleColor = (authorcoloratt == null) ? MelonLogger.DefaultTextColor : authorcoloratt.Color,
                Priority = (priorityatt == null) ? 0 : priorityatt.Priority,
                Location = FilePath,
                ID = (idatt == null) ? null : idatt.ID
            }.CreateMelon();
            if (instance == null)
                return;

            melonTbl.Add(instance);
        }

        private bool CheckInfoAttribute(ref MelonInfoAttribute infoAttribute)
        {
            infoAttribute = MelonUtils.PullAttributeFromAssembly<MelonInfoAttribute>(Assembly);

            // Legacy Support
            if (infoAttribute == null)
                infoAttribute = MelonUtils.PullAttributeFromAssembly<MelonModInfoAttribute>(Assembly)?.Convert();
            if (infoAttribute == null)
                infoAttribute = MelonUtils.PullAttributeFromAssembly<MelonPluginInfoAttribute>(Assembly)?.Convert();

            if ((infoAttribute == null) || (infoAttribute.SystemType == null))
            {
                MelonLogger.Error($"No {((infoAttribute == null) ? "MelonInfoAttribute Found" : "Type given to MelonInfoAttribute")} in {FilePath}");
                return false;
            }

            is_plugin = infoAttribute.SystemType.IsSubclassOf(typeof(MelonPlugin));
            bool is_mod_subclass = infoAttribute.SystemType.IsSubclassOf(typeof(MelonMod));
            if (!is_plugin && !is_mod_subclass)
            {
                MelonLogger.Error($"Type Specified {infoAttribute.SystemType.AssemblyQualifiedName} is not a Subclass of MelonPlugin or MelonMod in {FilePath}");
                return false;
            }

            bool nullcheck_name = string.IsNullOrEmpty(infoAttribute.Name);
            bool nullcheck_version = string.IsNullOrEmpty(infoAttribute.Version);
            if (nullcheck_name || nullcheck_version)
            {
                MelonLogger.Error($"No {(nullcheck_name ? "Name" : (nullcheck_version ? "Version" : ""))} given to MelonInfoAttribute in {FilePath}");
                return false;
            }

            if (is_plugin 
                ? MelonHandler.IsPluginAlreadyLoaded(infoAttribute.Name)
                : MelonHandler.IsModAlreadyLoaded(infoAttribute.Name))
            {
                MelonLogger.Error($"Duplicate {(is_plugin ? "Plugin" : "Mod")} {infoAttribute.Name}: {FilePath}");
                return false;
            }

            return true;
        }

        private bool CheckGameAttributes(ref List<MelonGameAttribute> gameAttributes)
        {
            gameAttributes = new List<MelonGameAttribute>();
            MelonGameAttribute[] gameatt = MelonUtils.PullAttributesFromAssembly<MelonGameAttribute>(Assembly);
            if ((gameatt != null) && (gameatt.Length > 0))
                gameAttributes.AddRange(gameatt);

            // Legacy Support
            MelonModGameAttribute[] legacymodgameAttributes = MelonUtils.PullAttributesFromAssembly<MelonModGameAttribute>(Assembly);
            if ((legacymodgameAttributes != null) && (legacymodgameAttributes.Length > 0))
                foreach (MelonModGameAttribute legacyatt in legacymodgameAttributes)
                    gameAttributes.Add(legacyatt.Convert());
            MelonPluginGameAttribute[] legacyplugingameAttributes = MelonUtils.PullAttributesFromAssembly<MelonPluginGameAttribute>(Assembly);
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
                    MelonLogger.Error($"Incompatible Game for {(is_plugin ? "Plugin" : "Mod")}: {FilePath}");
                    return false;
                }
            }

            return true;
        }

        private bool CheckProcessAttributes()
        {
            List<MelonProcessAttribute> processAttributes = MelonUtils.PullAttributesFromAssembly<MelonProcessAttribute>(Assembly).ToList();
            if (processAttributes.Count <= 0)
                return true;

            string current_exe_path = Process.GetCurrentProcess().MainModule.FileName;
            string current_exe_name = Path.GetFileName(current_exe_path);
            string current_exe_name_no_ext = Path.GetFileNameWithoutExtension(current_exe_path);

            bool is_compatible = false;
            for (int i = 0; i < processAttributes.Count; i++)
            {
                MelonProcessAttribute melonProcessAttribute = processAttributes[i];
                if (melonProcessAttribute == null)
                    continue;

                if (melonProcessAttribute.Universal
                    || current_exe_name.Equals(melonProcessAttribute.EXE_Name)
                    || current_exe_name_no_ext.Equals(melonProcessAttribute.EXE_Name))
                {
                    is_compatible = true;
                    break;
                }
            }
            if (!is_compatible)
            {
                MelonLogger.Error($"Incompatible Process Executable for {(is_plugin ? "Plugin" : "Mod")}: {FilePath}");
                return false;
            }

            return true;
        }

        private bool CheckGameVersionAttribute()
        {
            if (!is_plugin) // Temporarily Skip this Check for Plugins
                return true;

            List<MelonGameVersionAttribute> gameVersionAttributes = MelonUtils.PullAttributesFromAssembly<MelonGameVersionAttribute>(Assembly).ToList();
            if (gameVersionAttributes.Count <= 0)
                return true;

            bool is_compatible = false;
            for (int i = 0; i < gameVersionAttributes.Count; i++)
            {
                MelonGameVersionAttribute melonGameVersionAttribute = gameVersionAttributes[i];
                if (melonGameVersionAttribute == null)
                    continue;

                if (melonGameVersionAttribute.Universal
                    || (InternalUtils.UnityInformationHandler.GameVersion == melonGameVersionAttribute.Version))
                {
                    is_compatible = true;
                    break;
                }
            }
            if (!is_compatible)
            {
                //MelonLogger.Error($"Incompatible Game Version for {(is_plugin ? "Plugin" : "Mod")}: {FilePath}");
                MelonLogger.Error($"Incompatible Game Version for Mod: {FilePath}");
                return false;
            }

            return true;
        }

        private bool CheckPlatformAttribute()
        {
            MelonPlatformAttribute platformAttribute = MelonUtils.PullAttributeFromAssembly<MelonPlatformAttribute>(Assembly);
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
                MelonLogger.Error($"Incompatible Platform for {(is_plugin ? "Plugin" : "Mod")}: {FilePath}");
                return false;
            }
            return true;
        }

        private bool CheckPlatformDomainAttribute()
        {
            MelonPlatformDomainAttribute platformDomainAttribute = MelonUtils.PullAttributeFromAssembly<MelonPlatformDomainAttribute>(Assembly);
            if ((platformDomainAttribute == null)
                || (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.UNIVERSAL))
                return true;
            
            bool is_acceptable = MelonUtils.IsGameIl2Cpp()
                ? (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)
                : (platformDomainAttribute.Domain == MelonPlatformDomainAttribute.CompatibleDomains.MONO);

            if (!is_acceptable)
            {
                MelonLogger.Error($"Incompatible Platform Domain for {(is_plugin ? "Plugin" : "Mod")}: {FilePath}");
                return false;
            }

            return true;
        }

        private bool CheckVerifyLoaderVersionAttribute()
        {
            VerifyLoaderVersionAttribute verifyLoaderVersionAttribute = MelonUtils.PullAttributeFromAssembly<VerifyLoaderVersionAttribute>(Assembly);
            if (verifyLoaderVersionAttribute == null)
                return true;

            bool is_acceptable = verifyLoaderVersionAttribute.IsMinimum
                ? (verifyLoaderVersionAttribute.SemVer <= BuildInfo.Version)
                : (verifyLoaderVersionAttribute.SemVer == BuildInfo.Version);

            if (!is_acceptable)
            {
                MelonLogger.Error($"Incompatible MelonLoader Version for {(is_plugin ? "Plugin" : "Mod")}: {FilePath}");
                return false;
            }

            return true;
        }

        private bool CheckVerifyLoaderBuildAttribute()
        {
            VerifyLoaderBuildAttribute verifyLoaderBuildAttribute = MelonUtils.PullAttributeFromAssembly<VerifyLoaderBuildAttribute>(Assembly);
            if ((verifyLoaderBuildAttribute == null)
                || string.IsNullOrEmpty(verifyLoaderBuildAttribute.HashCode))
                return true;
            string currentHashCode = MelonUtils.HashCode;
            if (string.IsNullOrEmpty(currentHashCode))
                return true;
            if (!currentHashCode.Equals(verifyLoaderBuildAttribute.HashCode))
            {
                MelonLogger.Error($"Incompatible MelonLoader Build for {(is_plugin ? "Plugin" : "Mod")}: {FilePath}");
                return false;
            }
            return true;
        }
    }
}