using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonConsole is Only Here for Compatibility Reasons. This will be removed in a future update.", true)]
    public class MelonConsole
    {
        [Obsolete("MelonLoader.MelonConsole.SetTitle is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.SetConsoleTitle instead. This will be removed in a future update.", true)]
        public static void SetTitle(string title) => MelonUtils.SetConsoleTitle(title);
    }
}