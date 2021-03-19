using System;

namespace MelonLoader.CompatibilityLayers
{
    class Mono_Cecil_CL
    {
        internal static void Setup(AppDomain domain) =>
            domain.AssemblyResolve += (sender, args) => 
                (args.Name.StartsWith("Mono.Cecil, Version=")
                || args.Name.StartsWith("Mono.Cecil.Mdb, Version=")
                || args.Name.StartsWith("Mono.Cecil.Pdb, Version=")
                || args.Name.StartsWith("Mono.Cecil.Rocks, Version="))
                ? typeof(Mono_Cecil_CL).Assembly
                : null;
    }
}