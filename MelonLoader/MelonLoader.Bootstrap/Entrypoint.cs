using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap
{
    public static class Entrypoint
    {
#if NET6_0
        [UnmanagedCallersOnly]
#endif
        public static void Entry()
        {
            Console.WriteLine("HELLO WORLD!");
        }
    }   
}