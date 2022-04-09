#if NET6_0
using HarmonyLib;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace MelonLoader.Fixes
{
    internal class DotnetAssemblyLoadContextFix
    {
        private delegate Assembly DelegateInternalLoad(ReadOnlySpan<byte> arrAssembly, ReadOnlySpan<byte> arrSymbols);

        private static readonly Dictionary<string, Assembly> s_loadfile = new Dictionary<string, Assembly>();

        private static readonly MethodInfo AlcInternalLoad = typeof(AssemblyLoadContext).GetMethod("InternalLoad", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo AlcQCallLoadFromPath = typeof(AssemblyLoadContext).GetMethod("LoadFromPath", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo AlcQCallLoadFromStream = typeof(AssemblyLoadContext).GetMethod("LoadFromStream", BindingFlags.NonPublic | BindingFlags.Static);

        private static DelegateInternalLoad DefaultContextInternalLoad = AlcInternalLoad.CreateDelegate<DelegateInternalLoad>(AssemblyLoadContext.Default);


        internal static void Install()
        {
            Core.HarmonyInstance.Patch(AccessTools.Method(typeof(Assembly), nameof(Assembly.Load), new Type[] { typeof(byte[]), typeof(byte[]) }), new HarmonyMethod(typeof(DotnetAssemblyLoadContextFix), nameof(PreAssemblyLoad)));
            Core.HarmonyInstance.Patch(AccessTools.Method(typeof(Assembly), nameof(Assembly.LoadFile)), new HarmonyMethod(typeof(DotnetAssemblyLoadContextFix), nameof(PreAssemblyLoadFile)));

            //We have to load everything required for the verifier to avoid getting stuck in an infinite loop, prior to hooking AssemblyLoadContext.
            AssemblyVerifier.EnsureInitialized();

            //Now hook ALC.
            Core.HarmonyInstance.Patch(AlcQCallLoadFromPath, new HarmonyMethod(typeof(DotnetAssemblyLoadContextFix), nameof(PreAlcLoadFromPath)));
            Core.HarmonyInstance.Patch(AlcQCallLoadFromStream, new HarmonyMethod(typeof(DotnetAssemblyLoadContextFix), nameof(PreAlcLoadFromStream)));
        }

        public static bool PreAssemblyLoad(byte[] rawAssembly, byte[] rawSymbolStore, ref Assembly __result)
        {
            if(MelonDebug.IsEnabled() && !Environment.StackTrace.Contains("HarmonyLib"))
                MelonDebug.Msg($"[.NET AssemblyLoadContext Fix] Redirecting Assembly.Load call with {rawAssembly.Length}-byte assembly to AssemblyLoadContext.Default. Mod Devs: You may wish to use this explictly.");

            var (ok, reason) = AssemblyVerifier.VerifyByteArray(rawAssembly);
            if (!ok)
            {
                throw new BadImageFormatException();
            }

            __result = DefaultContextInternalLoad(rawAssembly, rawSymbolStore);

            //Prevent loading in non-default context, which is default behaviour of Assembly.Load
            return false;
        }

        public static bool PreAssemblyLoadFile(string path, ref Assembly __result)
        {
            MelonDebug.Msg($"[.NET AssemblyLoadContext Fix] Redirecting Assembly.LoadFile({path}) call to AssemblyLoadContext.Default.LoadFromAssemblyPath. Mod Devs: You may wish to use this explictly.");

            string normalizedPath = Path.GetFullPath(path);

            //Don't need to verify here, as we're passing to the ALC.
            //var (ok, reason) = AssemblyVerifier.VerifyFile(normalizedPath);
            //if (!ok)
            //{
            //    throw new BadImageFormatException();
            //}

            lock (s_loadfile)
            {
                if (s_loadfile.TryGetValue(normalizedPath, out __result))
                    return false;

                __result = AssemblyLoadContext.Default.LoadFromAssemblyPath(normalizedPath);

                s_loadfile.Add(normalizedPath, __result);
            }

            return false;
        }

        public static bool PreAlcLoadFromPath(string ilPath)
        {
            //MelonDebug.Msg($"[ALC FromPath] Validating {ilPath}...");

            //Simple pass-to-verifier and throw if bad.
            var (ok, reason) = AssemblyVerifier.VerifyFile(ilPath);
            if (!ok)
            {
                throw new BadImageFormatException();
            }

            //Continue to run the original runtime QCall.
            return true;
        }

        public static unsafe bool PreAlcLoadFromStream(IntPtr ptrAssemblyArray, int iAssemblyArrayLen)
        {
            //MelonDebug.Msg($"[ALC FromStream] Validating {iAssemblyArrayLen}-byte assembly...");

            byte[] assemblyBytes = new byte[iAssemblyArrayLen];
            Marshal.Copy(ptrAssemblyArray, assemblyBytes, 0, iAssemblyArrayLen);

            //Once again, pass to verifier and throw if bad.
            var (ok, reason) = AssemblyVerifier.VerifyByteArray(assemblyBytes);
            if (!ok)
            {
                throw new BadImageFormatException();
            }

            //And once again, continue to run the runtime QCall.
            return true;
        }

    }
}
#endif