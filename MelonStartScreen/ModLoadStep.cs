namespace MelonLoader.MelonStartScreen
{
    internal enum ModLoadStep
    {
        Generation,
        OnApplicationStart_Plugins,
        LoadMods,
        OnApplicationStart_Mods,
        OnApplicationLateStart_Plugins,
        OnApplicationLateStart_Mods
    }
}
