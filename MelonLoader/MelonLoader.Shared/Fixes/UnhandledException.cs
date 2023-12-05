using System;
using MelonLoader.Utils;

namespace MelonLoader.Fixes
{
    public static class UnhandledException
    {
        public static void Install(AppDomain domain) =>
            domain.UnhandledException +=
                (sender, args) =>
                    MelonLogger.Error((args.ExceptionObject as Exception).ToString());
    }
}