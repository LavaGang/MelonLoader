using System;
using MelonLoader;

namespace MelonLoader
{
    internal class Entrypoint
    {
        private static void Initialize()
        {
            External.Debug.Internal_Msg(ConsoleColor.Magenta, "Initialize", "This works :)");
        }

        private static void Start()
        {
            External.Debug.Internal_Msg(ConsoleColor.Magenta, "Start", "Starting");
        }
    }
}
