using System;
using System.Reflection;

namespace MelonLoader
{
    internal static class MelonCompatibilityLayer
    {
        private static event EventHandler<MelonCompatibilityLayerResolverEventArgs> LayerResolveEvents;

        static MelonCompatibilityLayer()
        {
            LayerResolveEvents += CompatibilityLayers.Melon.TryResolve;
        }

        internal static MelonCompatibilityLayerResolver Resolve(Assembly asm)
        {
            MelonCompatibilityLayerResolverEventArgs args = new MelonCompatibilityLayerResolverEventArgs();
            args.assembly = asm;
            LayerResolveEvents.Invoke(null, args);
            return args.inter;
        }
    }

    internal class MelonCompatibilityLayerResolver
    {
        internal virtual bool CheckAndCreate(Assembly asm, string filelocation, bool is_plugin, ref MelonBase baseInstance) => false;
    }

    internal class MelonCompatibilityLayerResolverEventArgs : EventArgs
	{
        internal Assembly assembly;
        internal MelonCompatibilityLayerResolver inter;
    }
}