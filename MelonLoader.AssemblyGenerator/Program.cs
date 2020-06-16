using System;

namespace MelonLoader.AssemblyGenerator
{
    public static class Program
    {
        public static bool Force_Regenerate = false;

        public static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Logger.LogError("Bad arguments for generator process; expected arguments: <unityVersion> <gameRoot> <gameData> <regenerate>");
                return -1;
            }

            if (args.Length >= 4)
                Force_Regenerate = true;

            try
            {
                return AssemblyGenerator.Main.Initialize(args[0], args[1], args[2]) ? 0 : -2;
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to generate assemblies;");
                Logger.LogError(ex.ToString());
                
                return -3;
            }
        }
    }
}