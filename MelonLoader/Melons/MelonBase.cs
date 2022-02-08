using MelonLoader.InternalUtils;
using MelonLoader.Lemons.Cryptography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
#pragma warning disable 0618

namespace MelonLoader
{
    public abstract class MelonBase
    {
        #region Static
        public static readonly MelonEvent<MelonBase> OnMelonRegistered = new MelonEvent<MelonBase>();
        public static readonly MelonEvent<MelonBase> OnMelonUnregistered = new MelonEvent<MelonBase>();

        public static event LemonFunc<Assembly, MelonBase[]> CustomMelonResolvers;

        public static List<MelonBase> RegisteredMelons => _registeredMelons.AsReadOnly().ToList();
        internal static List<MelonBase> _registeredMelons = new List<MelonBase>();

        /// <summary>
        /// Loads an Assembly from 'filePath' and returns all the Melons within the Assembly.
        /// </summary>
        /// <param name="filePath">Path of the Assembly containing the Melons</param>
        /// <param name="errorCode">Returned Error Code</param>S
        public static MelonBase[] Load(string filePath, out MelonLoadErrorCodes errorCode)
        {
            filePath = Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                errorCode = MelonLoadErrorCodes.InvalidPath;
                return null;
            }
            if (!filePath.EndsWith(".dll"))
            {
                errorCode = MelonLoadErrorCodes.WrongFileExtension;
                return null;
            }

            var mdbPath = filePath.Remove(filePath.Length - 3) + "mdb";
            if (File.Exists(mdbPath))
            {
                byte[] mdbBytes = null;
                try
                {
                    mdbBytes = File.ReadAllBytes(mdbPath);
                }
                catch { }

                if (mdbBytes != null)
                {
                    byte[] asmBytes;
                    try
                    {
                        asmBytes = File.ReadAllBytes(filePath);
                    }
                    catch
                    {
                        errorCode = MelonLoadErrorCodes.FailedToReadFile;
                        return null;
                    }

                    return Load(asmBytes, out errorCode, mdbBytes);
                }
            }

            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(filePath);
            }
            catch
            {
                errorCode = MelonLoadErrorCodes.FailedToLoadAssembly;
                return null;
            }

            return Load(asm, out errorCode);
        }

        /// <summary>
        /// Loads all Melons from raw Assembly Data.
        /// </summary>
        /// <param name="rawAssembly">Assembly Data containing the Melons</param>
        /// <param name="errorCode">Returned Error Code</param>
        /// <param name="mdbData">Assembly's MDB Data (Optional)</param>
        public static MelonBase[] Load(byte[] rawAssembly, out MelonLoadErrorCodes errorCode, byte[] mdbData = null)
        {
            Assembly asm;
            try
            {
                asm = mdbData == null ? Assembly.Load(rawAssembly) : Assembly.Load(rawAssembly, mdbData);
            }
            catch
            {
                errorCode = MelonLoadErrorCodes.FailedToLoadAssembly;
                return null;
            }

            return Load(asm, out errorCode);
        }

        /// <summary>
        /// Loads all Melons from an Assembly.
        /// </summary>
        /// <param name="asm">Assembly containing the Melons</param>
        /// <param name="errorCode">Returned Error Code</param>
        public static MelonBase[] Load(Assembly asm, out MelonLoadErrorCodes errorCode)
        {
            if (asm == null)
            {
                errorCode = MelonLoadErrorCodes.AssemblyIsNull;
                return null;
            }

            // \/ Custom Resolver \/
            var resolvers = CustomMelonResolvers?.GetInvocationList();
            if (resolvers != null)
                foreach (var r in resolvers)
                {
                    var customMelon = (MelonBase[])r.DynamicInvoke(asm);
                    if (customMelon == null || customMelon.Length == 0)
                        continue;

                    errorCode = MelonLoadErrorCodes.None;
                    return customMelon;
                }

            // \/ Default resolver \/
            var info = MelonUtils.PullAttributeFromAssembly<MelonInfoAttribute>(asm);
            if (info == null)
            {
                errorCode = MelonLoadErrorCodes.ModNotSupported;
                return null;
            }
            if (info.SystemType == null || !info.SystemType.IsSubclassOf(typeof(MelonBase)))
            {
                errorCode = MelonLoadErrorCodes.InvalidMelonType;
                return null;
            }

            MelonBase melon; 
            try
            {
                melon = (MelonBase)Activator.CreateInstance(info.SystemType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
            }
            catch
            {
                errorCode = MelonLoadErrorCodes.FailedToInitializeMelon;
                return null;
            }

            var priorityAttr = MelonUtils.PullAttributeFromAssembly<MelonPriorityAttribute>(asm);
            var colorAttr = MelonUtils.PullAttributeFromAssembly<MelonColorAttribute>(asm);
            var authorColorAttr = MelonUtils.PullAttributeFromAssembly<MelonAuthorColorAttribute>(asm);
            var procAttrs = MelonUtils.PullAttributesFromAssembly<MelonProcessAttribute>(asm);
            var gameAttrs = MelonUtils.PullAttributesFromAssembly<MelonGameAttribute>(asm);
            var optionalDependenciesAttr = MelonUtils.PullAttributeFromAssembly<MelonOptionalDependenciesAttribute>(asm);
            var idAttr = MelonUtils.PullAttributeFromAssembly<MelonIDAttribute>(asm);
            var gameVersionAttrs = MelonUtils.PullAttributesFromAssembly<MelonGameVersionAttribute>(asm);
            var platformAttr = MelonUtils.PullAttributeFromAssembly<MelonPlatformAttribute>(asm);
            var domainAttr = MelonUtils.PullAttributeFromAssembly<MelonPlatformDomainAttribute>(asm);
            var mlVersionAttr = MelonUtils.PullAttributeFromAssembly<VerifyLoaderVersionAttribute>(asm);
            var mlBuildAttr = MelonUtils.PullAttributeFromAssembly<VerifyLoaderBuildAttribute>(asm);
            var harmonyDPAAttr = MelonUtils.PullAttributeFromAssembly<HarmonyDontPatchAllAttribute>(asm);

            melon.Info = info;
            melon.Assembly = asm;
            melon.Location = asm.Location;
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
            melon.HarmonyDontPatchAll = harmonyDPAAttr != null;

            errorCode = MelonLoadErrorCodes.None;
            return new MelonBase[] { melon };
        }

        /// <summary>
        /// Creates a new Melon instance for a Wrapper.
        /// </summary>
        public static T CreateWrapper<T>(Assembly assembly, string name, string version, string author = null, MelonGameAttribute[] games = null, MelonProcessAttribute[] processes = null, int priority = 0, ConsoleColor? color = null, ConsoleColor? authorColor = null, string id = null) where T : MelonBase, new()
        {
            var melon = new T();
            melon.Info = new MelonInfoAttribute(typeof(T), name, version, author);
            melon.Assembly = assembly;
            melon.Location = assembly.Location;
            melon.Priority = priority;
            melon.ConsoleColor = color ?? MelonLogger.DefaultMelonColor;
            melon.AuthorConsoleColor = authorColor ?? MelonLogger.DefaultTextColor;
            melon.SupportedProcesses = processes;
            melon.Games = games;
            melon.OptionalDependencies = null;
            melon.ID = id;

            return melon;
        }

        /// <summary>
        /// Registers a List of Melons in Order.
        /// </summary>
        public static void RegisterInOrder<T>(List<T> melons) where T : MelonBase
        {
            if (melons == null)
                return;

            SortMelons(ref melons);

            foreach (var m in melons)
                m.Register();
        }

        private static void SortMelons<T>(ref List<T> melons) where T : MelonBase
        {
            DependencyGraph<T>.TopologicalSort(melons);
            melons = melons.OrderBy(x => x.Priority).ToList();
        }
        #endregion

        #region Instance
        private string _hash;
        private MelonGameAttribute[] _games = new MelonGameAttribute[0];
        private MelonProcessAttribute[] _processes = new MelonProcessAttribute[0];
        private MelonGameVersionAttribute[] _gameVersions = new MelonGameVersionAttribute[0];

        public readonly MelonEvent OnRegister = new MelonEvent();
        public readonly MelonEvent OnUnregister = new MelonEvent();

        /// <summary>
        /// Assembly of the Melon.
        /// </summary>
        public Assembly Assembly { get; internal set; }

        /// <summary>
        /// File Location of the Melon.
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// Priority of the Melon.
        /// </summary>
        public int Priority { get; internal set; }

        /// <summary>
        /// Console Color of the Melon.
        /// </summary>
        public ConsoleColor ConsoleColor { get; internal set; }

        /// <summary>
        /// Console Color of the Author that made this melon.
        /// </summary>
        public ConsoleColor AuthorConsoleColor { get; internal set;}

        /// <summary>
        /// Info Attribute of the Melon.
        /// </summary>
        public MelonInfoAttribute Info { get; internal set; }

        /// <summary>
        /// Process Attributes of the Melon.
        /// </summary>
        public MelonProcessAttribute[] SupportedProcesses
        {
            get => _processes;
            internal set
            {
                _processes = (value == null || value.Any(x => x.Universal)) ? new MelonProcessAttribute[0] : value;
            }
        }

        /// <summary>
        /// Game Attributes of the Melon.
        /// </summary>
        public MelonGameAttribute[] Games
        {
            get => _games;
            internal set
            {
                _games = (value == null || value.Any(x => x.Universal)) ? new MelonGameAttribute[0] : value;
            }
        }

        /// <summary>
        /// Game Version Attributes of the Melon.
        /// </summary>
        public MelonGameVersionAttribute[] SupportedGameVersions
        {
            get => _gameVersions;
            internal set
            {
                _gameVersions = (value == null || value.Any(x => x.Universal)) ? new MelonGameVersionAttribute[0] : value;
            }
        }

        /// <summary>
        /// Optional Dependencies Attribute of the Melon.
        /// </summary>
        public MelonOptionalDependenciesAttribute OptionalDependencies { get; internal set; }

        /// <summary>
        /// Platform Attribute of the Melon.
        /// </summary>
        public MelonPlatformAttribute SupportedPlatforms { get; internal set; }

        /// <summary>
        /// Platform Attribute of the Melon.
        /// </summary>
        public MelonPlatformDomainAttribute SupportedDomain { get; internal set; }

        /// <summary>
        /// Verify Loader Version Attribute of the Melon.
        /// </summary>
        public VerifyLoaderVersionAttribute SupportedMLVersion { get; internal set; }

        /// <summary>
        /// Verify Build Version Attribute of the Melon.
        /// </summary>
        public VerifyLoaderBuildAttribute SupportedMLBuild { get; internal set; }

        /// <summary>
        /// Auto-Created Harmony Instance of the Melon.
        /// </summary>
        public HarmonyLib.Harmony HarmonyInstance { get; internal set; }

        public bool HarmonyDontPatchAll { get; internal set; }

        /// <summary>
        /// Auto-Created MelonLogger Instance of the Melon.
        /// </summary>
        public MelonLogger.Instance LoggerInstance { get; internal set; }

        /// <summary>
        /// Optional ID of the Melon.
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// <see langword="true"/> if the Melon is registered.
        /// </summary>
        public bool Registered { get; private set; }

        /// <summary>
        /// Name of the current Melon Type.
        /// </summary>
        public abstract string MelonTypeName { get; }

        /// <summary>
        /// A SHA256 Hash of the Assembly.
        /// </summary>
        public string Hash
        {
            get
            {
                if (_hash == null)
                    _hash = MelonUtils.ComputeSimpleSHA256Hash(Location);
                return _hash;
            }
        }

        /// <summary>
        /// Runs before Support Module Initialization, after Assembly Generation on Il2Cpp Games
        /// </summary>
        public virtual void OnPreSupportModule() { }

        /// <summary>
        /// Runs after Game Initialization.
        /// </summary>
        public virtual void OnApplicationStart() { }

        /// <summary>
        /// Runs after OnApplicationStart.
        /// </summary>
        public virtual void OnApplicationLateStart() { }

        /// <summary>
        /// Runs once per frame.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Runs once per frame after OnUpdate and OnFixedUpdate have finished.
        /// </summary>
        public virtual void OnLateUpdate() { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Unity's IMGUI.
        /// </summary>
        public virtual void OnGUI() { }

        /// <summary>
        /// Runs when the Game is told to Close.
        /// </summary>
        public virtual void OnApplicationQuit() { }

        /// <summary>
        /// Runs when Melon Preferences get saved.
        /// </summary>
        public virtual void OnPreferencesSaved() { }
        /// <summary>
        /// Runs when Melon Preferences get saved. Gets passed the Preferences's File Path.
        /// </summary>
        public virtual void OnPreferencesSaved(string filepath) { }

        /// <summary>
        /// Runs when Melon Preferences get loaded.
        /// </summary>
        public virtual void OnPreferencesLoaded() { }
        
        /// <summary>
        /// Runs when Melon Preferences get loaded. Gets passed the Preferences's File Path.
        /// </summary>
        public virtual void OnPreferencesLoaded(string filepath) { }

        /// <summary>
        /// Runs when the Melon is registered.
        /// </summary>
        public virtual void OnInitializeMelon() { }

        /// <summary>
        /// Runs when the Melon is unregistered.
        /// </summary>
        public virtual void OnDeinitializeMelon() { }

        public Compatibility IsCompatibleWith(MelonGameAttribute game, string processName, string gameVersion, 
            string mlVersion, string mlBuildHashCode, MelonPlatformAttribute.CompatiblePlatforms platform, 
            MelonPlatformDomainAttribute.CompatibleDomains domain)
        {
            var result = Compatibility.Compatible;
            var compatible = true;
            if (!(Games.Length == 0 || Games.Any(x => x.IsCompatible(game))))
            {
                result |= Compatibility.IncompatibleGame;
                compatible = false;
            }
            else
            {
                if (!(SupportedGameVersions.Length == 0 || SupportedGameVersions.Any(x => x.Version == gameVersion)))
                {
                    result |= Compatibility.IncompatibleGameVersion;
                    compatible = false;
                }
                if (!(SupportedProcesses.Length == 0 || SupportedProcesses.Any(x => x.IsCompatible(processName))))
                {
                    result |= Compatibility.IncompatibleProcessName;
                    compatible = false;
                }
                if (!(SupportedPlatforms == null || SupportedPlatforms.IsCompatible(platform)))
                {
                    result |= Compatibility.IncompatiblePlatform;
                    compatible = false;
                }
                if (!(SupportedDomain == null || SupportedDomain.IsCompatible(domain)))
                {
                    result |= Compatibility.IncompatibleDomain;
                    compatible = false;
                }
            }
            if (!(SupportedMLVersion == null || SupportedMLVersion.IsCompatible(mlVersion)))
            {
                result |= Compatibility.IncompatibleMLVersion;
                compatible = false;
            }
            else
            {
                if (!(SupportedMLBuild == null || SupportedMLBuild.IsCompatible(mlBuildHashCode)))
                {
                    result |= Compatibility.IncompatibleMLBuild;
                    compatible = false;
                }
            }

            if (!compatible)
                result &= ~Compatibility.Compatible;
            return result;
        }

        public Compatibility IsCompatibleWithContext()
            => IsCompatibleWith(MelonUtils.CurrentGameAttribute, Process.GetCurrentProcess().ProcessName, MelonUtils.GameVersion, BuildInfo.Version, MelonUtils.HashCode, MelonUtils.CurrentPlatform, MelonUtils.CurrentDomain);

        public static void PrintIncompatibilities(Compatibility compatibility, MelonBase melon)
        {
            if (compatibility.HasFlag(Compatibility.Compatible))
                return;

            MelonLogger.Msg(ConsoleColor.DarkRed, $"{melon.Info.Name} is incompatible:");
            if (compatibility.HasFlag(Compatibility.IncompatibleGame))
            {
                MelonLogger.Msg("- This Melon is only compatible with the following Games:");

                foreach (var g in melon.Games)
                    MelonLogger.Msg($"    - '{g.Name}' by {g.Developer}");

                MelonLogger.WriteSpacer();
            }
            if (compatibility.HasFlag(Compatibility.IncompatibleGameVersion))
            {
                MelonLogger.Msg("- This Melon is only compatible with the following Game Versions:");

                foreach (var g in melon.SupportedGameVersions)
                    MelonLogger.Msg($"    - {g.Version}");

                MelonLogger.WriteSpacer();
            }
            if (compatibility.HasFlag(Compatibility.IncompatibleProcessName))
            {
                MelonLogger.Msg("- This Melon is only compatible with the following Process Names:");

                foreach (var p in melon.SupportedProcesses)
                    MelonLogger.Msg($"    - '{p.EXE_Name}'");

                MelonLogger.WriteSpacer();
            }
            if (compatibility.HasFlag(Compatibility.IncompatiblePlatform))
            {
                MelonLogger.Msg($"- This Melon is only compatible with the following Platforms:");

                foreach (var p in melon.SupportedPlatforms.Platforms)
                    MelonLogger.Msg($"    - {p}");

                MelonLogger.WriteSpacer();
            }
            if (compatibility.HasFlag(Compatibility.IncompatibleDomain))
            {
                MelonLogger.Msg($"- This Melon is only compatible with the following Domain:");
                MelonLogger.Msg($"    - {melon.SupportedDomain.Domain}");

                MelonLogger.WriteSpacer();
            }
            if (compatibility.HasFlag(Compatibility.IncompatibleMLVersion))
            {
                MelonLogger.Msg($"- This Melon is only compatible with the following MelonLoader Versions:");
                MelonLogger.Msg($"    - {melon.SupportedMLVersion.SemVer}{(melon.SupportedMLVersion.IsMinimum ? " or higher" : "")}");

                MelonLogger.WriteSpacer();
            }
            if (compatibility.HasFlag(Compatibility.IncompatibleMLBuild))
            {
                MelonLogger.Msg($"- This Melon is only compatible with the following MelonLoader Build Hash Codes:");
                MelonLogger.Msg($"    - {melon.SupportedMLBuild.HashCode}");

                MelonLogger.WriteSpacer();
            }

            MelonLogger.Msg(ConsoleColor.Red, "-----------------------------------------------------------------------");
        }

        /// <summary>
        /// Registers the Melon.
        /// </summary>
        public virtual bool Register()
        {
            if (Registered)
                return false;

            if (IsMelonRegistered(Info.Name, Info.Author) != null)
            {
                MelonLogger.Warning($"Failed to register {MelonTypeName} '{Location}': A Melon with the same Name and Author is already registered!");
                return false;
            }

            var comp = IsCompatibleWithContext();
            if (!comp.HasFlag(Compatibility.Compatible))
            {
                PrintIncompatibilities(comp, this);
                return false;
            }

            if (LoggerInstance == null)
                LoggerInstance = new MelonLogger.Instance(string.IsNullOrEmpty(ID) ? Info.Name : $"{ID}:{Info.Name}", ConsoleColor);
            HarmonyInstance = new HarmonyLib.Harmony($"{Assembly.FullName}:{Info.Name}");

            try
            {
                OnInitializeMelon();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to register {MelonTypeName} '{Location}': Melon failed to initialize!");
                MelonLogger.Error(ex.ToString());
                return false;
            }

            _registeredMelons.Add(this);
            Registered = true;

            if (!HarmonyDontPatchAll)
                HarmonyInstance.PatchAll(Assembly);
            RegisterTypeInIl2Cpp.RegisterAssembly(Assembly);

            PrintLoadInfo();

            OnRegister.Invoke();
            OnMelonRegistered.Invoke(this);
            return true;
        }

        /// <summary>
        /// If the Name is registered, returns the registered Melon. Otherwise, returns <see langword="null"/>.
        /// </summary>
        public static MelonBase IsMelonRegistered(string melonName, string melonAuthor)
            => RegisteredMelons.Find(x => x.Info.Name == melonName && x.Info.Author == melonAuthor);

        /// <summary>
        /// Unregisters the Melon.
        /// <para>This only unsubscribes the Melon from all Callbacks and unpatches all Methods that were patched by Harmony, but doesn't actually unload the Assembly.</para>
        /// <para>It is recommended to only use this Method in Cases where you 100% know the Melon will not work and/or will only cause Issues.</para>
        /// </summary>
        public virtual bool Unregister(string reason = null)
        {
            if (!Registered)
                return false;

            try
            {
                OnDeinitializeMelon();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to properly unregister {MelonTypeName} '{Location}': Melon failed to deinitialize!");
                MelonLogger.Error(ex.ToString());
            }

            _registeredMelons.Remove(this);
            HarmonyInstance.UnpatchSelf();
            HarmonyInstance = null;
            Registered = false;

            PrintUnloadInfo(reason);

            OnUnregister.Invoke();
            OnMelonUnregistered.Invoke(this);
            return true;
        }

        private void PrintLoadInfo()
        {
            MelonLogger.WriteSpacer();
            MelonLogger.Msg(ConsoleColor.DarkGreen, "------------------------------");

            MelonLogger.Msg(ConsoleColor.DarkGray, MelonTypeName + " loaded:");
            MelonLogger.Internal_PrintModName(ConsoleColor, AuthorConsoleColor, Info.Name, Info.Author, Info.Version, ID);

            if (!string.IsNullOrEmpty(Hash))
                MelonLogger.Msg($"SHA256 Hash: {Hash}");

            MelonLogger.Msg(ConsoleColor.DarkGreen, "------------------------------");
        }

        private void PrintUnloadInfo(string reason)
        {
            MelonLogger.WriteSpacer();
            MelonLogger.Msg(ConsoleColor.DarkRed, "------------------------------");

            MelonLogger.Msg(ConsoleColor.DarkGray, MelonTypeName + " unloaded:");
            MelonLogger.Internal_PrintModName(ConsoleColor, AuthorConsoleColor, Info.Name, Info.Author, Info.Version, ID);

            if (!string.IsNullOrEmpty(reason))
            {
                MelonLogger.Msg(string.Empty);
                MelonLogger.Msg($"Reason: '{reason}'");
            }

            MelonLogger.Msg(ConsoleColor.DarkRed, "------------------------------");
        }

        public static void ExecuteAll(LemonAction<MelonBase> func, bool unregisterOnFail = false, string unregistrationReason = null)
            => ExecuteList(func, RegisteredMelons, unregisterOnFail, unregistrationReason);

        public static void ExecuteList<T>(LemonAction<T> func, List<T> melons, bool unregisterOnFail = false, string unregistrationReason = null) where T : MelonBase
        {
            var failedMelons = (unregisterOnFail ? new List<T>() : null);

            LemonEnumerator<T> enumerator = new LemonEnumerator<T>(melons.ToArray());
            while (enumerator.MoveNext())
            {
                var melon = enumerator.Current;
                if (!melon.Registered)
                    continue;

                try { func(melon); }
                catch (Exception ex)
                {
                    melon.LoggerInstance.Error(ex.ToString());
                    if (unregisterOnFail)
                        failedMelons.Add(melon);
                }
            }

            if (unregisterOnFail)
            {
                foreach (var m in failedMelons)
                    m.Unregister(unregistrationReason);
            }
        }
        #endregion

        #region Obsolete Members
        [Obsolete("OnModSettingsApplied is obsolete. Please use OnPreferencesSaved instead.")]
        public virtual void OnModSettingsApplied() { }

        private Harmony.HarmonyInstance _OldHarmonyInstance;
        [Obsolete("harmonyInstance is obsolete. Please use HarmonyInstance instead.")]
        public Harmony.HarmonyInstance harmonyInstance { get { if (_OldHarmonyInstance == null) _OldHarmonyInstance = new Harmony.HarmonyInstance(HarmonyInstance.Id); return _OldHarmonyInstance; } }
        [Obsolete("Harmony is obsolete. Please use HarmonyInstance instead.")]
        public Harmony.HarmonyInstance Harmony { get { if (_OldHarmonyInstance == null) _OldHarmonyInstance = new Harmony.HarmonyInstance(HarmonyInstance.Id); return _OldHarmonyInstance; } }
        #endregion

        [Flags]
        public enum Compatibility
        {
            Compatible,
            IncompatibleMLVersion,
            IncompatibleMLBuild,
            IncompatibleGame,
            IncompatibleGameVersion,
            IncompatibleProcessName,
            IncompatibleDomain,
            IncompatiblePlatform
        }
    }
}