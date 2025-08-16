#if OSX && NET6_0_OR_GREATER
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using HarmonyLib;
using MelonLoader.Utils;

namespace MelonLoader.Fixes.Dotnet;

// This fix helps il2cppinterop to find GameAssembly.dylib
public class NativeLibraryFix
{
    internal static void Install()
    {
        try
        {
            Core.HarmonyInstance.Patch(AccessTools.Method(typeof(System.Runtime.InteropServices.NativeLibrary),
                    nameof(System.Runtime.InteropServices.NativeLibrary.Load),
                    [typeof(string), typeof(Assembly), typeof(DllImportSearchPath?)]),
                AccessTools.Method(typeof(NativeLibraryFix), nameof(LoadLibrary)).ToNewHarmonyMethod());
        }
        catch (Exception ex) { MelonLogger.Warning($"NativeLibraryFix Exception: {ex}"); }
    }

    private static bool LoadLibrary(ref string __0, Assembly __1, DllImportSearchPath? __2)
    {
        if (__0 != "GameAssembly")
            return true;
        __0 = Path.Combine(MelonEnvironment.GameExecutablePath, "Contents", "Frameworks", $"{__0}.dylib");
        MelonDebug.Msg($"Loading library {__0}");
        return true;
    }
}
#endif