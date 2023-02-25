using MelonLoader.InternalUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
#pragma warning disable 0618

namespace MelonLoader
{
    public abstract class MelonBase
    {
        #region Static

        /// <summary>
        /// Called once a Melon is fully registered.
        /// </summary>
        public static readonly MelonEvent<MelonBase> OnMelonRegistered = new();

        /// <summary>
        /// Called when a Melon unregisters.
        /// </summary>
        public static readonly MelonEvent<MelonBase> OnMelonUnregistered = new();

        /// <summary>
        /// Called before a Melon starts initializing.
        /// </summary>
        public static readonly MelonEvent<MelonBase> OnMelonInitializing = new();

        public static ReadOnlyCollection<MelonBase> RegisteredMelons => _registeredMelons.AsReadOnly();
        internal static List<MelonBase> _registeredMelons = new();

        /// <summary>
        /// Creates a new Melon instance for a Wrapper.
        /// </summary>
        public static T CreateWrapper<T>(string name, string author, string version, MelonGameAttribute[] games = null, MelonProcessAttribute[] processes = null, int priority = 0, Color? color = null, Color? authorColor = null, string id = null) where T : MelonBase, new()
        {
            var melon = new T
            {
                Info = new MelonInfoAttribute(typeof(T), name, version, author),
                MelonAssembly = MelonAssembly.LoadMelonAssembly(null, typeof(T).Assembly),
                Priority = priority,
                ConsoleColor = color ?? MelonLogger.DefaultMelonColor,
                AuthorConsoleColor = authorColor ?? MelonLogger.DefaultTextColor,
                SupportedProcesses = processes,
                Games = games,
                OptionalDependencies = null,
                ID = id
            };

            return melon;
        }

        /// <summary>
        /// Registers a List of Melons in the right order.
        /// </summary>
        public static void RegisterSorted<T>(IEnumerable<T> melons) where T : MelonBase
        {
            if (melons == null)
                return;

            var collection = melons.ToList();
            SortMelons(ref collection);

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

        private MelonGameAttribute[] _games = new MelonGameAttribute[0];
        private MelonProcessAttribute[] _processes = new MelonProcessAttribute[0];
        private MelonGameVersionAttribute[] _gameVersions = new MelonGameVersionAttribute[0];

        public readonly MelonEvent OnRegister = new();
        public readonly MelonEvent OnUnregister = new();

        /// <summary>
        /// MelonAssembly of the Melon.
        /// </summary>
        public MelonAssembly MelonAssembly { get; internal set; }

        /// <summary>
        /// Priority of the Melon.
        /// </summary>
        public int Priority { get; internal set; }

        /// <summary>
        /// Console Color of the Melon.
        /// </summary>
        public Color ConsoleColor { get; internal set; }

        /// <summary>
        /// Console Color of the Author that made this melon.
        /// </summary>
        public Color AuthorConsoleColor { get; internal set; }

        /// <summary>
        /// Info Attribute of the Melon.
        /// </summary>
        public MelonInfoAttribute Info { get; internal set; }

        /// <summary>
        /// AdditionalCredits Attribute of the Melon
        /// </summary>
        public MelonAdditionalCreditsAttribute AdditionalCredits { get; internal set; }

        /// <summary>
        /// Process Attributes of the Melon.
        /// </summary>
        public MelonProcessAttribute[] SupportedProcesses
        {
            get => _processes;
            internal set => _processes = (value == null || value.Any(x => x.Universal)) ? new MelonProcessAttribute[0] : value;
        }

        /// <summary>
        /// Game Attributes of the Melon.
        /// </summary>
        public MelonGameAttribute[] Games
        {
            get => _games;
            internal set => _games = (value == null || value.Any(x => x.Universal)) ? new MelonGameAttribute[0] : value;
        }

        /// <summary>
        /// Game Version Attributes of the Melon.
        /// </summary>
        public MelonGameVersionAttribute[] SupportedGameVersions
        {
            get => _gameVersions;
            internal set => _gameVersions = (value == null || value.Any(x => x.Universal)) ? new MelonGameVersionAttribute[0] : value;
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

        #region Callbacks

        /// <summary>
        /// Runs before Support Module Initialization and after Assembly Generation for Il2Cpp Games.
        /// </summary>
        public virtual void OnPreSupportModule() { }

        /// <summary>
        /// Runs once per frame.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Physics.
        /// </summary>
        public virtual void OnFixedUpdate() { }

        /// <summary>
        /// Runs once per frame, after <see cref="OnUpdate"/>.
        /// </summary>
        public virtual void OnLateUpdate() { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Unity's IMGUI.
        /// </summary>
        public virtual void OnGUI() { }

        /// <summary>
        /// Runs on a quit request. It is possible to abort the request in this callback.
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
        /// Runs when the Melon is registered. Executed before the Melon's info is printed to the console. This callback should only be used a constructor for the Melon.
        /// </summary>
        /// <remarks>
        /// Please note that this callback may run before the Support Module is loaded.
        /// <br>As a result, using unhollowed assemblies may not be possible yet and you would have to override <see cref="OnInitializeMelon"/> instead.</br>
        /// </remarks>
        public virtual void OnEarlyInitializeMelon() { }

        /// <summary>
        /// Runs after the Melon has registered. This callback waits until MelonLoader has fully initialized (<see cref="MelonEvents.OnApplicationStart"/>).
        /// </summary>
        public virtual void OnInitializeMelon() { }

        /// <summary>
        /// Runs after <see cref="OnInitializeMelon"/>. This callback waits until Unity has invoked the first 'Start' messages (<see cref="MelonEvents.OnApplicationLateStart"/>).
        /// </summary>
        public virtual void OnLateInitializeMelon() { }

        /// <summary>
        /// Runs when the Melon is unregistered. Also runs before the Application is closed (<see cref="MelonEvents.OnApplicationDefiniteQuit"/>).
        /// </summary>
        public virtual void OnDeinitializeMelon() { }

        #endregion

        public Incompatibility[] FindIncompatiblities(MelonGameAttribute game, string processName, string gameVersion,
            string mlVersion, string mlBuildHashCode, MelonPlatformAttribute.CompatiblePlatforms platform,
            MelonPlatformDomainAttribute.CompatibleDomains domain)
        {
            var result = new List<Incompatibility>();
            if (!(Games.Length == 0 || Games.Any(x => x.IsCompatible(game))))
                result.Add(Incompatibility.Game);
            else
            {
                if (!(SupportedGameVersions.Length == 0 || SupportedGameVersions.Any(x => x.Version == gameVersion)))
                    result.Add(Incompatibility.GameVersion);

                if (!(SupportedProcesses.Length == 0 || SupportedProcesses.Any(x => x.IsCompatible(processName))))
                    result.Add(Incompatibility.ProcessName);

                if (!(SupportedPlatforms == null || SupportedPlatforms.IsCompatible(platform)))
                    result.Add(Incompatibility.Platform);

                if (!(SupportedDomain == null || SupportedDomain.IsCompatible(domain)))
                    result.Add(Incompatibility.Domain);
            }
            if (!(SupportedMLVersion == null || SupportedMLVersion.IsCompatible(mlVersion)))
                result.Add(Incompatibility.MLVersion);
            else
            {
                if (!(SupportedMLBuild == null || SupportedMLBuild.IsCompatible(mlBuildHashCode)))
                    result.Add(Incompatibility.MLBuild);
            }

            return result.ToArray();
        }

        public Incompatibility[] FindIncompatiblitiesFromContext()
        {
            return FindIncompatiblities(MelonUtils.CurrentGameAttribute, Process.GetCurrentProcess().ProcessName, MelonUtils.GameVersion, BuildInfo.Version, MelonUtils.HashCode, MelonUtils.CurrentPlatform, MelonUtils.CurrentDomain);
        }

        public static void PrintIncompatibilities(Incompatibility[] incompatibilities, MelonBase melon)
        {
            if (incompatibilities == null || incompatibilities.Length == 0)
                return;

            MelonLogger.WriteLine(Color.Red);
            MelonLogger.MsgDirect(Color.DarkRed, $"'{melon.Info.Name} v{melon.Info.Version}' is incompatible:");
            if (incompatibilities.Contains(Incompatibility.Game))
            {
                MelonLogger.MsgDirect($"- {melon.Info.Name} is only compatible with the following Games:");

                foreach (var g in melon.Games)
                    MelonLogger.MsgDirect($"    - '{g.Name}' by {g.Developer}");
            }
            if (incompatibilities.Contains(Incompatibility.GameVersion))
            {
                MelonLogger.MsgDirect($"- {melon.Info.Name} is only compatible with the following Game Versions:");

                foreach (var g in melon.SupportedGameVersions)
                    MelonLogger.MsgDirect($"    - {g.Version}");
            }
            if (incompatibilities.Contains(Incompatibility.ProcessName))
            {
                MelonLogger.MsgDirect($"- {melon.Info.Name} is only compatible with the following Process Names:");

                foreach (var p in melon.SupportedProcesses)
                    MelonLogger.MsgDirect($"    - '{p.EXE_Name}'");
            }
            if (incompatibilities.Contains(Incompatibility.Platform))
            {
                MelonLogger.MsgDirect($"- {melon.Info.Name} is only compatible with the following Platforms:");

                foreach (var p in melon.SupportedPlatforms.Platforms)
                    MelonLogger.MsgDirect($"    - {p}");
            }
            if (incompatibilities.Contains(Incompatibility.Domain))
            {
                MelonLogger.MsgDirect($"- {melon.Info.Name} is only compatible with the following Domain:");
                MelonLogger.MsgDirect($"    - {melon.SupportedDomain.Domain}");
            }
            if (incompatibilities.Contains(Incompatibility.MLVersion))
            {
                MelonLogger.MsgDirect($"- {melon.Info.Name}  is only compatible with the following MelonLoader Versions:");
                MelonLogger.MsgDirect($"    - {melon.SupportedMLVersion.SemVer}{(melon.SupportedMLVersion.IsMinimum ? " or higher" : "")}");
            }
            if (incompatibilities.Contains(Incompatibility.MLBuild))
            {
                MelonLogger.MsgDirect($"- {melon.Info.Name} is only compatible with the following MelonLoader Build Hash Codes:");
                MelonLogger.MsgDirect($"    - {melon.SupportedMLBuild.HashCode}");
            }

            MelonLogger.WriteLine(Color.Red);
            MelonLogger.WriteSpacer();
        }

        /// <summary>
        /// Registers the Melon.
        /// </summary>
        public bool Register()
        {
            if (Registered)
                return false;

            if (FindMelon(Info.Name, Info.Author) != null)
            {
                MelonLogger.Warning($"Failed to register {MelonTypeName} '{Location}': A Melon with the same Name and Author is already registered!");
                return false;
            }

            var comp = FindIncompatiblitiesFromContext();
            if (comp.Length != 0)
            {
                PrintIncompatibilities(comp, this);
                return false;
            }

            OnMelonInitializing.Invoke(this);

            LoggerInstance ??= new MelonLogger.Instance(string.IsNullOrEmpty(ID) ? Info.Name : $"{ID}:{Info.Name}", ConsoleColor);
            HarmonyInstance ??= new HarmonyLib.Harmony($"{Assembly.FullName}:{Info.Name}");

            Registered = true; // this has to be true before the melon can subscribe to any events
            RegisterCallbacks();

            try
            {
                OnEarlyInitializeMelon();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to register {MelonTypeName} '{Location}': Melon failed to initialize!");
                MelonLogger.Error(ex.ToString());
                Registered = false;
                return false;
            }

            if (!RegisterInternal())
                return false;

            _registeredMelons.Add(this);

            PrintLoadInfo();

            OnRegister.Invoke();
            OnMelonRegistered.Invoke(this);

            if (MelonEvents.OnApplicationStart.Disposed)
                LoaderInitialized();
            else
                MelonEvents.OnApplicationStart.Subscribe(LoaderInitialized, Priority, true);

            if (MelonEvents.OnApplicationLateStart.Disposed)
                OnLateInitializeMelon();
            else
                MelonEvents.OnApplicationLateStart.Subscribe(OnLateInitializeMelon, Priority, true);

            return true;
        }

        private void HarmonyInit()
        {
            if (!MelonAssembly.HarmonyDontPatchAll)
                HarmonyInstance.PatchAll(MelonAssembly.Assembly);
        }

        private void LoaderInitialized()
        {
            try
            {
                OnInitializeMelon();
            }
            catch (Exception ex)
            {
                LoggerInstance.Error(ex);
            }
        }

        protected private virtual bool RegisterInternal()
        {
            return true;
        }

        protected private virtual void UnregisterInternal() { }

        protected private virtual void RegisterCallbacks()
        {
            MelonEvents.OnApplicationQuit.Subscribe(OnApplicationQuit, Priority);
            MelonEvents.OnUpdate.Subscribe(OnUpdate, Priority);
            MelonEvents.OnLateUpdate.Subscribe(OnLateUpdate, Priority);
            MelonEvents.OnGUI.Subscribe(OnGUI, Priority);
            MelonEvents.OnFixedUpdate.Subscribe(OnFixedUpdate, Priority);
            MelonEvents.OnApplicationLateStart.Subscribe(OnApplicationLateStart, Priority);

            MelonPreferences.OnPreferencesLoaded.Subscribe(PrefsLoaded, Priority);
            MelonPreferences.OnPreferencesSaved.Subscribe(PrefsSaved, Priority);
        }

        private void PrefsSaved(string path)
        {
            OnPreferencesSaved(path);
            OnPreferencesSaved();
            OnModSettingsApplied();
        }

        private void PrefsLoaded(string path)
        {
            OnPreferencesLoaded(path);
            OnPreferencesLoaded();
        }

        /// <summary>
        /// Tries to find a registered Melon that matches the given Info.
        /// </summary>
        public static MelonBase FindMelon(string melonName, string melonAuthor)
        {
            return _registeredMelons.Find(x => x.Info.Name == melonName && x.Info.Author == melonAuthor);
        }

        /// <summary>
        /// Unregisters the Melon and all other Melons located in the same Assembly.
        /// <para>This only unsubscribes the Melons from all Callbacks/<see cref="MelonEvent"/>s and unpatches all Methods that were patched by Harmony, but doesn't actually unload the whole Assembly.</para>
        /// </summary>
        public void Unregister(string reason = null, bool silent = false)
        {
            if (!Registered)
                return;

            MelonAssembly.UnregisterMelons(reason, silent);
        }

        internal void UnregisterInstance(string reason, bool silent)
        {
            if (!Registered)
                return;

            try
            {
                OnDeinitializeMelon();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to properly unregister {MelonTypeName} '{Location}': Melon failed to deinitialize!");
                MelonLogger.Error(ex.ToString());
            }

            UnregisterInternal();

            _registeredMelons.Remove(this);
            HarmonyInstance.UnpatchSelf();
            Registered = false;

            if (!silent)
                PrintUnloadInfo(reason);

            OnUnregister.Invoke();
            OnMelonUnregistered.Invoke(this);
        }

        private void PrintLoadInfo()
        {
            MelonLogger.WriteLine(Color.DarkGreen);
            
            MelonLogger.Internal_PrintModName(ConsoleColor, AuthorConsoleColor, Info.Name, Info.Author, AdditionalCredits?.Credits, Info.Version, ID);
            MelonLogger.MsgDirect(Color.DarkGray, $"Assembly: {Path.GetFileName(MelonAssembly.Location)}");

            MelonLogger.WriteLine(Color.DarkGreen);
        }

        private void PrintUnloadInfo(string reason)
        {
            MelonLogger.WriteLine(Color.DarkRed);

            MelonLogger.MsgDirect(Color.DarkGray, MelonTypeName + " deinitialized:");
            MelonLogger.Internal_PrintModName(ConsoleColor, AuthorConsoleColor, Info.Name, Info.Author, AdditionalCredits?.Credits, Info.Version, ID);

            if (!string.IsNullOrEmpty(reason))
            {
                MelonLogger.MsgDirect(string.Empty);
                MelonLogger.MsgDirect($"Reason: '{reason}'");
            }

            MelonLogger.WriteLine(Color.DarkRed);
        }

        public static void ExecuteAll(LemonAction<MelonBase> func, bool unregisterOnFail = false, string unregistrationReason = null)
        {
            ExecuteList(func, _registeredMelons, unregisterOnFail, unregistrationReason);
        }

        public static void ExecuteList<T>(LemonAction<T> func, List<T> melons, bool unregisterOnFail = false, string unregistrationReason = null) where T : MelonBase
        {
            var failedMelons = (unregisterOnFail ? new List<T>() : null);

            LemonEnumerator<T> enumerator = new(melons.ToArray());
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

        public static void SendMessageAll(string name, params object[] arguments)
        {
            LemonEnumerator<MelonBase> enumerator = new(_registeredMelons.ToArray());
            while (enumerator.MoveNext())
            {
                var melon = enumerator.Current;
                if (!melon.Registered)
                    continue;

                try { melon.SendMessage(name, arguments); }
                catch (Exception ex) { melon.LoggerInstance.Error(ex.ToString()); }
            }
        }

        public object SendMessage(string name, params object[] arguments)
        {
            var msg = Info.SystemType.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (msg == null)
                return null;

            return msg.Invoke(msg.IsStatic ? null : this, arguments);
        }
        #endregion

        #region Obsolete Members

        private Harmony.HarmonyInstance _OldHarmonyInstance;

        [Obsolete("Please use either the OnLateInitializeMelon callback, or the 'MelonEvents::OnApplicationLateStart' event instead.")]
        public virtual void OnApplicationLateStart() { }

        [Obsolete("For mods, use OnInitializeMelon instead. For plugins, use OnPreModsLoaded instead.")]
        public virtual void OnApplicationStart() { }

        [Obsolete("Please use OnPreferencesSaved instead.")]
        public virtual void OnModSettingsApplied() { }

        [Obsolete("Please use HarmonyInstance instead.")]
#pragma warning disable IDE1006 // Naming Styles
        public Harmony.HarmonyInstance harmonyInstance { get { _OldHarmonyInstance ??= new Harmony.HarmonyInstance(HarmonyInstance.Id); return _OldHarmonyInstance; } }
#pragma warning restore IDE1006 // Naming Styles

        [Obsolete("Please use HarmonyInstance instead.")]
        public Harmony.HarmonyInstance Harmony { get { _OldHarmonyInstance ??= new Harmony.HarmonyInstance(HarmonyInstance.Id); return _OldHarmonyInstance; } }

        [Obsolete("Please use MelonAssembly.Assembly instead.")]
        public Assembly Assembly => MelonAssembly.Assembly;

        [Obsolete("Please use MelonAssembly.HarmonyDontPatchAll instead.")]
        public bool HarmonyDontPatchAll => MelonAssembly.HarmonyDontPatchAll;

        [Obsolete("Please use MelonAssembly.Hash instead.")]
        public string Hash => MelonAssembly.Hash;

        [Obsolete("Please use MelonAssembly.Location instead.")]
        public string Location => MelonAssembly.Location;

        #endregion


        public enum Incompatibility
        {
            MLVersion,
            MLBuild,
            Game,
            GameVersion,
            ProcessName,
            Domain,
            Platform
        }
    }
}