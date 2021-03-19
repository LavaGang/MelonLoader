using System;

namespace MelonLoader.CompatibilityLayers
{
    class Tomlyn_CL
    {
        internal static void Setup(AppDomain domain) =>
            domain.AssemblyResolve += (sender, args) =>
                args.Name.StartsWith("Tomlyn, Version=")
                ? typeof(Tomlyn_CL).Assembly
                : null;
    }
}