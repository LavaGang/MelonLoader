using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModHelper;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
    internal class MuseDashModLoader : MelonCompatibilityLayer.Resolver
    {
        private readonly Type[] mod_types;

        private MuseDashModLoader(Assembly assembly, string filepath, IEnumerable<Type> types) : base(assembly, filepath)
            => mod_types = types.ToArray();

        internal static void Setup(AppDomain domain)
        {
            // To-Do:
            // Detect if MuseDashModLoader is already Installed
            // Point domain.AssemblyResolve to already installed MuseDashModLoader Assembly
            // Point ResolveAssemblyToLayerResolver to Dummy MelonCompatibilityLayer.Resolver
            
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

            IEnumerable<Type> mod_types = args.assembly.GetValidTypes(x =>
            {
                Type[] interfaces = x.GetInterfaces();
                return (interfaces != null) && interfaces.Any() && interfaces.Contains(typeof(IMod));  // To-Do: Change to Type Reflection based on Setup
            });
            if ((mod_types == null) || !mod_types.Any())
                return;

            args.inter = new MuseDashModLoader(args.assembly, args.filepath, mod_types);
        }

        public override void CheckAndCreate(ref List<MelonBase> melonTbl)
        {
            foreach (var mod_type in mod_types)
                LoadMod(mod_type, ref melonTbl);
        }

        private void LoadMod(Type mod_type, ref List<MelonBase> melonTbl)
        {
            var modInstance = Activator.CreateInstance(mod_type) as IMod;

            var mod_name = modInstance.Name;

            if (string.IsNullOrEmpty(mod_name))
                mod_name = mod_type.FullName;

            if (MelonHandler.IsModAlreadyLoaded(mod_name))
            {
                MelonLogger.Error($"Duplicate File {mod_name}: {FilePath}");
                return;
            }

            var mod_version = Assembly.GetName().Version.ToString();
            if (string.IsNullOrEmpty(mod_version) || mod_version.Equals("0.0.0.0"))
                mod_version = "1.0.0.0";

            var wrapper = new MelonCompatibilityLayer.WrapperData
            {
                Assembly = Assembly,
                Info = new MelonInfoAttribute(typeof(MelonModWrapper), mod_name, mod_version, modInstance.Author, modInstance.HomePage),
                Games = null,
                Priority = 0,
                Location = FilePath
            }.CreateMelon<MelonModWrapper>();

            if (wrapper == null)
                return;

            wrapper.modInstance = modInstance;
            melonTbl.Add(wrapper);
            ModLoader.ModLoader.mods.Add(modInstance);
        }

        private class MelonModWrapper : MelonMod
        {
            internal IMod modInstance;
            
            public override void OnApplicationStart() => modInstance.DoPatching();
            
        }
    }
}