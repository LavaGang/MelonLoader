using System.IO;

namespace MelonLoader
{
    public class Console
    {
        public static bool Enabled = false;

        internal static void Create()
        {
            Imports.AllocConsole();
            System.Console.SetOut(new StreamWriter(System.Console.OpenStandardOutput()) { AutoFlush = true });
            System.Console.SetIn(new StreamReader(System.Console.OpenStandardInput()));
            System.Console.Clear();
            System.Console.Title = (ModHandler.BuildInfo.Name + " v" + ModHandler.BuildInfo.Version + " Open-Beta");
            Imports.SetForegroundWindow(Imports.GetConsoleWindow());
        }
    }
}