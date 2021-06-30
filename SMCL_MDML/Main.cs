using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModHelper;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
    internal class MuseDashModLoader_Module : MelonCompatibilityLayer.Module
    {
        public override void Setup()
        {
            // To-Do:
            // Detect if MuseDashModLoader is already Installed
            // Point domain.AssemblyResolve to already installed MuseDashModLoader Assembly
            // Point GetResolverFromAssembly to Dummy MelonCompatibilityLayer.Resolver

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                (args.Name.StartsWith("ModHelper, Version=")
                || args.Name.StartsWith("ModLoader, Version="))
                    ? typeof(MuseDashModLoader_Module).Assembly
                    : null;
        }

        public override MelonCompatibilityLayer.Resolver GetResolverFromAssembly(Assembly assembly, string filepath)
        {
            IEnumerable<Type> mod_types = assembly.GetValidTypes(x =>
            {
                Type[] interfaces = x.GetInterfaces();
                return (interfaces != null) && interfaces.Any() && interfaces.Contains(typeof(IMod));  // To-Do: Change to Type Reflection based on Setup
            });
            if ((mod_types == null) || !mod_types.Any())
                return null;

            return new MuseDashModLoader_Resolver(assembly, filepath, mod_types);
        }
    }
}