namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static void Initialize() => Core.Initialize();
        internal static void PreStart() => Core.PreStart();
        internal static void Start() => Core.Start();
    }
}
