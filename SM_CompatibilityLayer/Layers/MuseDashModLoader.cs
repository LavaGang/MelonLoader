using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModHelper;

namespace MelonLoader.CompatibilityLayers
{
    internal class MuseDashModLoader : MelonCompatibilityLayer.Resolver
    {
        private readonly Assembly asm;
        private readonly string filepath;
        private readonly Type[] mod_types;

        private MuseDashModLoader(Assembly assembly, string filelocation, IEnumerable<Type> types)
        {
            asm = assembly;
            filepath = filelocation;
            mod_types = types.ToArray();
        }

        internal static void Setup(AppDomain domain)
        {
            domain.AssemblyResolve += (sender, args) =>
                args.Name.StartsWith("ModHelper, Version=") || args.Name.StartsWith("ModLoader, Version=")
                    ? typeof(MuseDashModLoader).Assembly
                    : null;
            MelonCompatibilityLayer.AddResolveAssemblyToLayerResolverEvent(ResolveAssemblyToLayerResolver);
        }

        private static void ResolveAssemblyToLayerResolver(MelonCompatibilityLayer.LayerResolveEventArgs args)
        {
            if (args.inter != null)
                return;

            var mod_types = args.assembly.GetValidTypes(x => x.GetInterface("IMod") != null);
            if (mod_types == null || !mod_types.Any())
                return;

            args.inter = new MuseDashModLoader(args.assembly, args.filepath, mod_types);
        }

        public override void CheckAndCreate(ref List<MelonBase> melonTbl)
        {
            foreach (var mod_type in mod_types)
                LoadMod(mod_type, filepath, ref melonTbl);
        }

        private void LoadMod(Type mod_type, string filelocation, ref List<MelonBase> melonTbl)
        {
            var modInstance = Activator.CreateInstance(mod_type) as IMod;

            var mod_name = modInstance.Name;

            if (string.IsNullOrEmpty(mod_name))
                mod_name = mod_type.FullName;

            if (MelonHandler.IsModAlreadyLoaded(mod_name))
            {
                MelonLogger.Error($"Duplicate File {mod_name}: {filelocation}");
                return;
            }

            var mod_version = asm.GetName().Version.ToString();
            if (string.IsNullOrEmpty(mod_version) || mod_version.Equals("0.0.0.0"))
                mod_version = "1.0.0.0";

            var wrapper = new MelonCompatibilityLayer.WrapperData
            {
                Assembly = asm,
                Info = new MelonInfoAttribute(typeof(MelonModWrapper), mod_name, mod_version, modInstance.Author, modInstance.HomePage),
                Games = null,
                Priority = 0,
                Location = filelocation
            }.CreateMelon<MelonModWrapper>();

            if (wrapper == null)
                return;

            wrapper.modInstance = modInstance;
            melonTbl.Add(wrapper);
            (typeof(ModLoader.ModLoader).GetField("mods", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as List<IMod>)?.Add(modInstance);
        }

        private class MelonModWrapper : MelonMod
        {
            internal IMod modInstance;
            
            public override void OnApplicationStart() => modInstance.DoPatching();
            
        }
    }
}