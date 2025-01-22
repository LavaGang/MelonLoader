using MelonLoader.Modules;
using MelonLoader.Resolver;
using ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[assembly: MelonLoader.PatchShield]

namespace MelonLoader.CompatibilityLayers;

internal class Muse_Dash_Mono_Module : MelonModule
{
    public override void OnInitialize()
    {
        // To-Do:
        // Detect if MuseDashModLoader is already Installed
        // Point AssemblyResolveInfo to already installed MuseDashModLoader Assembly
        // Inject Custom Resolver

        string[] assembly_list =
        [
            "ModHelper",
            "ModLoader",
        ];
        var base_assembly = typeof(Muse_Dash_Mono_Module).Assembly;
        foreach (var assemblyName in assembly_list)
            MelonAssemblyResolver.GetAssemblyResolveInfo(assemblyName).Override = base_assembly;

        MelonAssembly.CustomMelonResolvers += Resolve;
    }

    private ResolvedMelons Resolve(Assembly asm)
    {
        var modTypes = asm.GetValidTypes(x =>
        {
            var interfaces = x.GetInterfaces();
            return (interfaces != null) && interfaces.Any() && interfaces.Contains(typeof(IMod));  // To-Do: Change to Type Reflection based on Setup
        });
        if ((modTypes == null) || !modTypes.Any())
            return new ResolvedMelons(null, null);

        var melons = new List<MelonBase>();
        var rotten = new List<RottenMelon>();
        foreach (var t in modTypes)
        {
            var mel = LoadMod(asm, t, out var rm);
            if (mel != null)
                melons.Add(mel);
            else
                rotten.Add(rm);
        }

        return new ResolvedMelons([.. melons], [.. rotten]);
    }

    private MelonBase LoadMod(Assembly asm, Type modType, out RottenMelon rottenMelon)
    {
        rottenMelon = null;

        IMod modInstance;
        try
        {
            modInstance = Activator.CreateInstance(modType) as IMod;
        }
        catch (Exception ex)
        {
            rottenMelon = new RottenMelon(modType, "Failed to create an instance of the MMDL Mod.", ex);
            return null;
        }

        var modName = modInstance.Name;

        if (string.IsNullOrEmpty(modName))
            modName = modType.FullName;

        var modVersion = asm.GetName().Version.ToString();
        if (string.IsNullOrEmpty(modVersion) || modVersion.Equals("0.0.0.0"))
            modVersion = "1.0.0.0";

        var melon = MelonBase.CreateWrapper<MuseDashModWrapper>(modName, null, modVersion);
        melon.modInstance = modInstance;
        ModLoader.ModLoader.mods.Add(modInstance);
        ModLoader.ModLoader.LoadDependency(asm);
        return melon;
    }
}