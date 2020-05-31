using System;

namespace MelonLoader.GeneratorProcess
{
    public static class GeneratorProcessProgram
    {
        public static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                MelonModLogger.LogError("Bad arguments for generator process; expected 2 arguments: <unityVersion> <gameRoot> <gameData>");
                return -1;
            }

            try
            {
                return AssemblyGenerator.Main.Initialize(args[0], args[1], args[2]) ? 0 : -2;
            }
            catch (Exception ex)
            {
                MelonModLogger.LogError("Failed to generate assemblies;");
                MelonModLogger.LogError(ex.ToString());
                
                return -3;
            }
        }
    }
}