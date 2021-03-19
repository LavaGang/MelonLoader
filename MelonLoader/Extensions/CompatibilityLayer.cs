using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader
{
    internal static class MelonCompatibilityLayer
    {
        internal static void Setup(AppDomain domain)
        {
            CompatibilityLayers.Tomlyn_CL.Setup(domain);
            CompatibilityLayers.TinyJSON_CL.Setup(domain);
            CompatibilityLayers.SharpZipLib_CL.Setup(domain);
            CompatibilityLayers.Mono_Cecil_CL.Setup(domain);
            CompatibilityLayers.MonoMod_CL.Setup(domain);
            CompatibilityLayers.Harmony_CL.Setup(domain);
            CompatibilityLayers.Melon_CL.Setup(domain);
            CompatibilityLayers.IPA_CL.Setup(domain);

            domain.AssemblyResolve += MelonHandler.AssemblyResolver;
        }

        internal static event EventHandler<LayerResolveEventArgs> ResolveAssemblyToLayerResolverEvents;
        internal static Resolver ResolveAssemblyToLayerResolver(Assembly asm)
        {
            LayerResolveEventArgs args = new LayerResolveEventArgs();
            args.assembly = asm;
            ResolveAssemblyToLayerResolverEvents?.Invoke(null, args);
            return args.inter;
        }

        internal static event EventHandler RefreshPluginsTableEvents;
        internal static void RefreshPluginsTable() => RefreshPluginsTableEvents?.Invoke(null, null);

        internal static event EventHandler RefreshModsTableEvents;
        internal static void RefreshModsTable() => RefreshModsTableEvents?.Invoke(null, null);

        internal class Resolver { internal virtual void CheckAndCreate(string filelocation, bool is_plugin, ref List<MelonBase> melonTbl) { } }
        internal class LayerResolveEventArgs : EventArgs
        {
            internal Assembly assembly;
            internal Resolver inter;
        }
    }
}