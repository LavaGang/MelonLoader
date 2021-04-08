using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonConsole is Only Here for Compatibility Reasons.")]
    public class MelonConsole
    {
        [Obsolete("MelonLoader.MelonConsole.SetTitle is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.SetConsoleTitle instead.")]
        public static void SetTitle(string title) => MelonUtils.SetConsoleTitle(title);
    }
}