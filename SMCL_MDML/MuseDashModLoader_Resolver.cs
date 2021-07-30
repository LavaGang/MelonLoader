using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader.Attributes;
using MelonLoader.Melons;
using MelonLoader.Utils;
using ModHelper;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
    internal class MuseDashModLoader_Resolver : MelonCompatibilityLayer.Resolver
    {
        private readonly Type[] mod_types;

        internal MuseDashModLoader_Resolver(Assembly assembly, string filepath, IEnumerable<Type> types) : base(assembly, filepath)
            => mod_types = types.ToArray();

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
            ModLoader.ModLoader.LoadDependency(mod_type.Assembly);
        }

        private class MelonModWrapper : MelonMod
        {
            internal IMod modInstance;
            
            public override void OnApplicationStart() => modInstance.DoPatching();
            
        }
    }
}