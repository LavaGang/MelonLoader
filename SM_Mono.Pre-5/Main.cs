using MelonLoader.Support.Preferences;
using MelonLoader.SupportModule;
using UnityEngine;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static ISupportModule_From Interface = null;
        internal static GameObject obj = null;
        internal static SM_Component component = null;
        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;
            UnityMappers.RegisterMappers();
            SM_Component.Create();
            return new SupportModule_To();
        }
    }
}