using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MelonLoader;

public abstract class MelonMod : MelonTypeBase<MelonMod>
{
    static MelonMod()
    {
        TypeName = "Mod";
    }

    private protected override bool RegisterInternal()
    {
        try
        {
            OnPreSupportModule();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Failed to register {MelonTypeName} '{MelonAssembly.Location}': Melon failed to initialize in the deprecated OnPreSupportModule callback!");
            MelonLogger.Error(ex.ToString());
            return false;
        }

        if (!base.RegisterInternal())
            return false;

        if (MelonEvents.MelonHarmonyInit.Disposed)
            HarmonyInit();
        else
            MelonEvents.MelonHarmonyInit.Subscribe(HarmonyInit, Priority, true);

        return true;
    }
    private void HarmonyInit()
    {
        if (!MelonAssembly.HarmonyDontPatchAll)
            HarmonyInstance.PatchAll(MelonAssembly.Assembly);
    }

    private protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        MelonEvents.OnSceneWasLoaded.Subscribe(OnSceneWasLoaded, Priority);
        MelonEvents.OnSceneWasInitialized.Subscribe(OnSceneWasInitialized, Priority);
        MelonEvents.OnSceneWasUnloaded.Subscribe(OnSceneWasUnloaded, Priority);

#pragma warning disable CS0618 // Type or member is obsolete
        RegisterObsoleteCallbacks();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Obsolete("Used to make obsolete callbacks still function.")]
    private void RegisterObsoleteCallbacks()
    {
        MelonEvents.OnSceneWasLoaded.Subscribe((idx, name) => OnLevelWasLoaded(idx), Priority);
        MelonEvents.OnSceneWasInitialized.Subscribe((idx, name) => OnLevelWasInitialized(idx), Priority);
        MelonEvents.OnApplicationStart.Subscribe(OnApplicationStart, Priority);
    }

    #region Callbacks

    /// <summary>
    /// Runs when a new Scene is loaded.
    /// </summary>
    public virtual void OnSceneWasLoaded(int buildIndex, string sceneName) { }

    /// <summary>
    /// Runs once a Scene is initialized.
    /// </summary>
    public virtual void OnSceneWasInitialized(int buildIndex, string sceneName) { }

    /// <summary>
    /// Runs once a Scene unloads.
    /// </summary>
    public virtual void OnSceneWasUnloaded(int buildIndex, string sceneName) { }

    #endregion

    #region Obsolete Members
    [Obsolete("Override OnSceneWasLoaded instead. This will be removed in a future version.", true)]
    public virtual void OnLevelWasLoaded(int level) { }
    [Obsolete("Override OnSceneWasInitialized instead. This will be removed in a future version.", true)]
    public virtual void OnLevelWasInitialized(int level) { }

    [Obsolete]
    private MelonModInfoAttribute _LegacyInfoAttribute = null;
    [Obsolete("Use MelonBase.Info instead. This will be removed in a future version.", true)]
    public MelonModInfoAttribute InfoAttribute
    {
        get
        {
            _LegacyInfoAttribute ??= new MelonModInfoAttribute(Info.SystemType, Info.Name, Info.Version, Info.Author, Info.DownloadLink);
            return _LegacyInfoAttribute;
        }
    }
    [Obsolete()]
    private MelonModGameAttribute[] _LegacyGameAttributes = null;
    [Obsolete("Use MelonBase.Games instead. This will be removed in a future version.", true)]
    public MelonModGameAttribute[] GameAttributes
    {
        get
        {
            if (_LegacyGameAttributes != null)
                return _LegacyGameAttributes;
            List<MelonModGameAttribute> newatts = [];
            foreach (var att in Games)
                newatts.Add(new MelonModGameAttribute(att.Developer, att.Name));
            _LegacyGameAttributes = [.. newatts];
            return _LegacyGameAttributes;
        }
    }

    #endregion
}