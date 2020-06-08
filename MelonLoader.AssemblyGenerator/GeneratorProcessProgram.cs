using System;

namespace MelonLoader.GeneratorProcess
{
    public static class GeneratorProcessProgram
    {
        public static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Logger.LogError("Bad arguments for generator process; expected 2 arguments: <unityVersion> <gameRoot> <gameData>");
                return -1;
            }

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