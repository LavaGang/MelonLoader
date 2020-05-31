using System;

namespace MelonLoader.GeneratorProcess
{
    public class MelonModLogger
    {
        public static void Log(string str)
        {
            Console.WriteLine(str);
        }
        
        public static void LogError(string str)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ForegroundColor = oldColor;
        }
    }
}