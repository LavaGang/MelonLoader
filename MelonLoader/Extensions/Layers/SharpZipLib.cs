using System;

namespace MelonLoader.CompatibilityLayers
{
    class SharpZipLib_CL
    {
        internal static void Setup(AppDomain domain) =>
            domain.AssemblyResolve += (sender, args) =>
                args.Name.StartsWith("ICSharpCode.SharpZipLib, Version=")
                ? typeof(SharpZipLib_CL).Assembly
                : null;
    }
}