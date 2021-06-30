using System;
using System.Collections.Generic;
using System.Linq;
using ModHelper;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
    internal class MuseDashModLoader_Module : MelonCompatibilityLayer.Module
    {
        public void Setup(AppDomain domain)
        {
            // To-Do:
            // Detect if MuseDashModLoader is already Installed
            // Point domain.AssemblyResolve to already installed MuseDashModLoader Assembly
            // Point ResolveAssemblyToLayerResolver to Dummy MelonCompatibilityLayer.Resolver

            domain.AssemblyResolve += (sender, args) =>
                (args.Name.StartsWith("ModHelper, Version=")
                || args.Name.StartsWith("ModLoader, Version="))
                    ? typeof(MuseDashModLoader_Module).Assembly
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

            args.inter = new MuseDashModLoader_Resolver(args.assembly, args.filepath, mod_types);
        }
    }
}