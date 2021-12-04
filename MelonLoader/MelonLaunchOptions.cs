using System;

namespace MelonLoader
{
    public static class MelonLaunchOptions
    {
        internal static void Load()
        {
//#if DEBUG
//            Core.DebugMode = true;
//#endif

            string[] args = Environment.GetCommandLineArgs();
            if ((args == null)
                || (args.Length <= 0))
                return;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.IsNullOrEmpty(arg))
                    continue;
                arg = arg.ToLowerInvariant();
                int valueint = 0;
                string valuestr = null;
                if ((i + 1) < args.Length)
                    valuestr = args[i + 1];
                switch (arg)
                {
                    // Core
//#if !DEBUG
//                    case "--melonloader.debug":
//                        Core.DebugMode = true;
//                        goto default;
//#endif
                    case "--quitfix":
                        Core.QuitFix = true;
                        goto default;
                    case "--melonloader.disablestartscreen":
                        Core.StartScreen = false;
                        goto default;
                    case "--melonloader.loadmodeplugins":
                        if ((i + 1) < args.Length)
                            valuestr = args[i + 1];
                        else
                            goto default;
                        if (string.IsNullOrEmpty(valuestr))
                            goto default;
                        if (!int.TryParse(valuestr, out valueint))
                            goto default;
                        Core.LoadMode_Plugins = (Core.LoadModeEnum)MelonUtils.Clamp(valueint, (int)Core.LoadModeEnum.NORMAL, (int)Core.LoadModeEnum.BOTH);
                        goto default;
                    case "--melonloader.loadmodemods":
                        if ((i + 1) < args.Length)
                            valuestr = args[i + 1];
                        else
                            goto default;
                        if (string.IsNullOrEmpty(valuestr))
                            goto default;
                        if (!int.TryParse(valuestr, out valueint))
                            goto default;
                        Core.LoadMode_Mods = (Core.LoadModeEnum)MelonUtils.Clamp(valueint, (int)Core.LoadModeEnum.NORMAL, (int)Core.LoadModeEnum.BOTH);
                        goto default;

                    // Console
                    case "--melonloader.consolemode":
                        if ((i + 1) < args.Length)
                            valuestr = args[i + 1];
                        else
                            goto default;
                        if (string.IsNullOrEmpty(valuestr))
                            goto default;
                        if (!int.TryParse(valuestr, out valueint))
                            goto default;
                        Console.Mode = (Console.DisplayMode)MelonUtils.Clamp(valueint, (int)Console.DisplayMode.NORMAL, (int)Console.DisplayMode.LEMON);
                        goto default;
                    case "--melonloader.disableunityclc":
                        Console.CleanUnityLogs = false;
                        goto default;

                    // Il2CppAssemblyGenerator
                    case "--melonloader.agfoffline":
                        Il2CppAssemblyGenerator.OfflineMode = true;
                        goto default;
                    case "--melonloader.agfregenerate":
                        Il2CppAssemblyGenerator.ForceRegeneration = true;
                        goto default;
                    case "--melonloader.agfvdumper":
                        if ((i + 1) < args.Length)
                            valuestr = args[i + 1];
                        else
                            goto default;
                        if (string.IsNullOrEmpty(valuestr))
                            goto default;
                        Il2CppAssemblyGenerator.ForceVersion_Dumper = valuestr;
                        goto default;
                    case "--melonloader.agfvunhollower":
                        if ((i + 1) < args.Length)
                            valuestr = args[i + 1];
                        else
                            goto default;
                        if (string.IsNullOrEmpty(valuestr))
                            goto default;
                        Il2CppAssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower = valuestr;
                        goto default;
                    case "--melonloader.agfvunity":
                        if ((i + 1) < args.Length)
                            valuestr = args[i + 1];
                        else
                            goto default;
                        if (string.IsNullOrEmpty(valuestr))
                            goto default;
                        Il2CppAssemblyGenerator.ForceVersion_UnityDependencies = valuestr;
                        goto default;

                    default:
                        break;
                }
            }
        }

#region Args
        public static class Core
        {
            public enum LoadModeEnum
            {
                NORMAL,
                DEV,
                BOTH
            }
            public static LoadModeEnum LoadMode_Plugins { get; internal set; }
            public static LoadModeEnum LoadMode_Mods { get; internal set; }
            //public static bool DebugMode { get; internal set; }
            public static bool QuitFix { get; internal set; }
            public static bool StartScreen { get; internal set; } = true;
        }

        public static class Console
        {
            public enum DisplayMode
            {
                NORMAL,
                MAGENTA,
                RAINBOW,
                RANDOMRAINBOW,
                LEMON
            };
            public static DisplayMode Mode { get; internal set; }
            public static bool CleanUnityLogs { get; internal set; } = true;
        }

        public static class Il2CppAssemblyGenerator
        {
            public static bool ForceRegeneration { get; internal set; }
            public static bool OfflineMode { get; internal set; }
            public static string ForceVersion_Dumper { get; internal set; }
            public static string ForceVersion_Il2CppAssemblyUnhollower { get; internal set; }
            public static string ForceVersion_UnityDependencies { get; internal set; }
        }
#endregion
    }
}