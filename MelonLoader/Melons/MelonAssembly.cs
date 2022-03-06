using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    public sealed class MelonAssembly
    {
        #region Static

        /// <summary>
        /// Called before a process of resolving Melons from a MelonAssembly has started.
        /// </summary>
        public static readonly MelonEvent<Assembly> OnAssemblyResolving = new MelonEvent<Assembly>();
        public static event LemonFunc<Assembly, ResolvedMelons> CustomMelonResolvers;

        internal static List<MelonAssembly> loadedAssemblies = new List<MelonAssembly>();

        /// <summary>
        /// List of all loaded MelonAssemblies.
        /// </summary>
        public static ReadOnlyCollection<MelonAssembly> LoadedAssemblies => loadedAssemblies.AsReadOnly();

        /// <summary>
        /// Gets the MelonAssembly of the given member. If the given member is not in any MelonAssembly, returns null.
        /// </summary>
        public static MelonAssembly GetMelonAssemblyOfMember(MemberInfo member, object obj = null)
        {
            if (member == null)
                return null;

            if (obj != null && obj is MelonBase melon)
                return melon.MelonAssembly;

            var name = member.DeclaringType.Assembly.FullName;
            var ma = loadedAssemblies.Find(x => x.Assembly.FullName == name);
            return ma;
        }

        /// <summary>
        /// Loads or finds a MelonAssembly from path.
        /// </summary>
        /// <param name="path">Path of the MelonAssembly</param>
        public static MelonAssembly LoadMelonAssembly(string path)
        {
            if (path == null)
            {
                MelonLogger.Error("Failed to load a Melon Assembly: Path cannot be null.");
                return null;
            }

            path = Path.GetFullPath(path);

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(path);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to load Melon Assembly from '{path}':\n{ex}");
                return null;
            }

            return LoadMelonAssembly(assembly);
        }

        /// <summary>
        /// Loads or finds a MelonAssembly from raw Assembly Data.
        /// </summary>
        public static MelonAssembly LoadMelonAssembly(byte[] assemblyData, byte[] symbolsData = null)
        {
            if (assemblyData == null)
                MelonLogger.Error("Failed to load a Melon Assembly: assemblyData cannot be null.");

            Assembly assembly;
            try
            {
                assembly = symbolsData != null ? Assembly.Load(assemblyData, symbolsData) : Assembly.Load(assemblyData);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to load Melon Assembly from raw Assembly Data (length {assemblyData.Length}):\n{ex}");
                return null;
            }

            return LoadMelonAssembly(assembly);
        }

        /// <summary>
        /// Loads or finds a MelonAssembly.
        /// </summary>
        public static MelonAssembly LoadMelonAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                MelonLogger.Error("Failed to load a Melon Assembly: Assembly cannot be null.");
                return null;
            }

            var ma = loadedAssemblies.Find(x => x.Assembly.FullName == assembly.FullName);
            if (ma != null)
                return ma;

            MelonLogger.Msg($"Loading Melon Assembly: '{assembly.Location}'...");
            OnAssemblyResolving.Invoke(assembly);
            ma = new MelonAssembly(assembly);
            loadedAssemblies.Add(ma);
            ma.LoadMelons();
            MelonLogger.Msg($"{ma.loadedMelons.Count} {"Melon".MakePlural(ma.loadedMelons.Count)} loaded.");
            if (ma.rottenMelons.Count != 0)
            {
                MelonLogger.Error($"Failed to load {ma.rottenMelons.Count} {"Melon".MakePlural(ma.rottenMelons.Count)}:");
                foreach (var r in ma.rottenMelons)
                {
                    MelonLogger.Error($"Failed to load Melon '{r.type.FullName}': {r.errorMessage}");
                    if (r.exception != null)
                        MelonLogger.Error(r.exception);
                }
            }
            MelonLogger.WriteSpacer();
            return ma;
        }

        #endregion

        #region Instance

        private string _hash;

        private List<MelonBase> loadedMelons = new List<MelonBase>();
        private List<RottenMelon> rottenMelons = new List<RottenMelon>();

        public readonly MelonEvent OnUnregister = new MelonEvent();

        public bool HarmonyDontPatchAll { get; private set; } = true;

        /// <summary>
        /// A SHA256 Hash of the Assembly.
        /// </summary>
        public string Hash
        {
            get
            {
                if (_hash == null)
                    _hash = MelonUtils.ComputeSimpleSHA256Hash(Assembly.Location);
                return _hash;
            }
        }
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// A list of all loaded Melons in the Assembly.
        /// </summary>
        public ReadOnlyCollection<MelonBase> LoadedMelons => loadedMelons.AsReadOnly();

        /// <summary>
        /// A list of all broken Melons in the Assembly.
        /// </summary>
        public ReadOnlyCollection<RottenMelon> RottenMelons => rottenMelons.AsReadOnly();

        private MelonAssembly(Assembly assembly)
        {
            this.Assembly = assembly;
        }

        /// <summary>
        /// Unregisters all Melons in this Assembly.
        /// </summary>
        public void UnregisterMelons(string reason = null)
        {
            foreach (var m in loadedMelons)
                m.UnregisterInstance(reason);

            OnUnregister.Invoke();
        }

        private void LoadMelons()
        {
            // \/ Custom Resolver \/
            var resolvers = CustomMelonResolvers?.GetInvocationList();
            if (resolvers != null)
                foreach (var r in resolvers)
                {
                    var customMelon = (ResolvedMelons)r.DynamicInvoke(Assembly);

                    loadedMelons.AddRange(customMelon.loadedMelons);
                    rottenMelons.AddRange(customMelon.rottenMelons);
                }

            // \/ Default resolver \/
            var info = MelonUtils.PullAttributeFromAssembly<MelonInfoAttribute>(Assembly);
            if (info == null)
                return;

            if (info.SystemType == null || !info.SystemType.IsSubclassOf(typeof(MelonBase)))
                return;

            MelonBase melon;
            try
            {
                melon = (MelonBase)Activator.CreateInstance(info.SystemType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
            }
            catch (Exception ex)
            {
                rottenMelons.Add(new RottenMelon(info.SystemType, "Failed to create an instance of the Melon.", ex));
                return;
            }

            var priorityAttr = MelonUtils.PullAttributeFromAssembly<MelonPriorityAttribute>(Assembly);
            var colorAttr = MelonUtils.PullAttributeFromAssembly<MelonColorAttribute>(Assembly);
            var authorColorAttr = MelonUtils.PullAttributeFromAssembly<MelonAuthorColorAttribute>(Assembly);
            var procAttrs = MelonUtils.PullAttributesFromAssembly<MelonProcessAttribute>(Assembly);
            var gameAttrs = MelonUtils.PullAttributesFromAssembly<MelonGameAttribute>(Assembly);
            var optionalDependenciesAttr = MelonUtils.PullAttributeFromAssembly<MelonOptionalDependenciesAttribute>(Assembly);
            var idAttr = MelonUtils.PullAttributeFromAssembly<MelonIDAttribute>(Assembly);
            var gameVersionAttrs = MelonUtils.PullAttributesFromAssembly<MelonGameVersionAttribute>(Assembly);
            var platformAttr = MelonUtils.PullAttributeFromAssembly<MelonPlatformAttribute>(Assembly);
            var domainAttr = MelonUtils.PullAttributeFromAssembly<MelonPlatformDomainAttribute>(Assembly);
            var mlVersionAttr = MelonUtils.PullAttributeFromAssembly<VerifyLoaderVersionAttribute>(Assembly);
            var mlBuildAttr = MelonUtils.PullAttributeFromAssembly<VerifyLoaderBuildAttribute>(Assembly);
            var harmonyDPAAttr = MelonUtils.PullAttributeFromAssembly<HarmonyDontPatchAllAttribute>(Assembly);

            melon.Info = info;
            melon.MelonAssembly = this;
            melon.Priority = priorityAttr == null ? 0 : priorityAttr.Priority;
            melon.ConsoleColor = colorAttr == null ? MelonLogger.DefaultMelonColor : colorAttr.Color;
            melon.AuthorConsoleColor = authorColorAttr == null ? MelonLogger.DefaultTextColor : authorColorAttr.Color;
            melon.SupportedProcesses = procAttrs;
            melon.Games = gameAttrs;
            melon.SupportedGameVersions = gameVersionAttrs;
            melon.SupportedPlatforms = platformAttr;
            melon.SupportedDomain = domainAttr;
            melon.SupportedMLVersion = mlVersionAttr;
            melon.SupportedMLBuild = mlBuildAttr;
            melon.OptionalDependencies = optionalDependenciesAttr;
            melon.ID = idAttr?.ID;
            HarmonyDontPatchAll = harmonyDPAAttr != null;

            loadedMelons.Add(melon);

            RegisterTypeInIl2Cpp.RegisterAssembly(Assembly);
        }

        #endregion
    }
}
