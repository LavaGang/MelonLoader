#if NET6_0
using HarmonyLib;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
#endif

namespace MelonLoader.Fixes
{
    internal class DotnetAssemblyLoadContextFix
    {

        internal static void Install()
        {
#if NET6_0
            Core.HarmonyInstance.Patch(AccessTools.Method(typeof(Assembly), nameof(Assembly.Load), new Type[] { typeof(byte[]), typeof(byte[]) }), new HarmonyMethod(typeof(DotnetAssemblyLoadContextFix), nameof(PreAssemblyLoad)));
            Core.HarmonyInstance.Patch(AccessTools.Method(typeof(Assembly), nameof(Assembly.LoadFile)), new HarmonyMethod(typeof(DotnetAssemblyLoadContextFix), nameof(PreAssemblyLoadFile)));
#endif
        }

#if NET6_0

        private delegate Assembly DelegateInternalLoad(ReadOnlySpan<byte> arrAssembly, ReadOnlySpan<byte> arrSymbols);

        private static readonly Dictionary<string, Assembly> s_loadfile = new Dictionary<string, Assembly>();

        private static readonly MethodInfo AlcInternalLoad = typeof(AssemblyLoadContext).GetMethod("InternalLoad", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly DelegateInternalLoad DefaultContextInternalLoad = AlcInternalLoad.CreateDelegate<DelegateInternalLoad>(AssemblyLoadContext.Default);

        public static bool PreAssemblyLoad(byte[] rawAssembly, byte[] rawSymbolStore, ref Assembly __result)
        {
            if(MelonDebug.IsEnabled() && !Environment.StackTrace.Contains("HarmonyLib"))
                MelonDebug.Msg($"[.NET AssemblyLoadContext Fix] Redirecting Assembly.Load call with {rawAssembly.Length}-byte assembly to AssemblyLoadContext.Default. Mod Devs: You may wish to use this explictly.");

#if NET6_0
            var (ok, reason) = AssemblyVerifier.LoadRawPatch(rawAssembly);
            if (!ok)
            {
                return false;
            }
#endif
            __result = DefaultContextInternalLoad(rawAssembly, rawSymbolStore);
            return false;
        }

        public static bool PreAssemblyLoadFile(string path, ref Assembly __result)
        {
            MelonDebug.Msg($"[.NET AssemblyLoadContext Fix] Redirecting Assembly.LoadFile({path}) call to AssemblyLoadContext.Default.LoadFromAssemblyPath. Mod Devs: You may wish to use this explictly.");

#if NET6_0
            var (ok, reason) = AssemblyVerifier.LoadFromPatch(path);
            if (!ok)
            {
                return false;
            }
#endif

            string normalizedPath = Path.GetFullPath(path);

            lock (s_loadfile)
            {
                if (s_loadfile.TryGetValue(normalizedPath, out __result))
                    return false;

                __result = AssemblyLoadContext.Default.LoadFromAssemblyPath(normalizedPath);

                s_loadfile.Add(normalizedPath, __result);
            }

            return false;
        }
#endif
    }
}
