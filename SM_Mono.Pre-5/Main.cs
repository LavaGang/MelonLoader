using UnityEngine;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static ISupportModule_From Interface = null;
        internal static GameObject obj = null;
        internal static Component component = null;
        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;

            Component.Create();
            return new SupportModule_To();
        }
    }
}