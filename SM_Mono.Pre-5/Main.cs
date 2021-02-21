using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using MelonLoader.Support.Preferences;
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
            string game_version = Application.version;
            MelonLogger.Msg($"Game Version: {game_version}");
            MelonUtils.SetConsoleTitle(GetVersionStrWithGameName(game_version));
            UnityMappers.RegisterMappers();

            Component.Create();
            return new SupportModule_To();
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string GetVersionStrWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameVersion = null);
    }
}