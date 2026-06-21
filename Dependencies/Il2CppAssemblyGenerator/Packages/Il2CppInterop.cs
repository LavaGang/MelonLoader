using Il2CppInterop.Common;
using MelonLoader.InternalUtils;
using MelonLoader.Utils;
using System.IO;
using System.Linq;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Il2CppInterop : Models.ExecutablePackage
    {
        internal Il2CppInterop()
        {
            Version = typeof(IL2CPP).Assembly.CustomAttributes
                .Where(x => x.AttributeType.Name == "AssemblyInformationalVersionAttribute")
                .Select(x => x.ConstructorArguments[0].Value.ToString())
                .FirstOrDefault();

            Name = nameof(Il2CppInterop);
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "Il2CppAssemblies");
        }

        internal override bool ShouldSetup()
            => false;

        internal override bool Execute()
        {
            Core.Logger.Msg("Generating Interop Assemblies...");

#if !DEBUG
            try
#endif
            {
                var gameExePath = MelonEnvironment.GameExecutablePath;
                MelonLogger.Msg($"Game Executable Path (managed): {gameExePath}");
                var unstripDirectory = ""; // Todo: supply unstripped assemblies
                BootstrapInterop.Library.Il2CppGenerate(gameExePath, gameExePath.Length, OutputFolder, OutputFolder.Length, unstripDirectory, unstripDirectory.Length);
            }
#if !DEBUG
            catch (System.Exception e)
            {
                Core.Logger.Error("Error Generating Interop Assemblies!", e);
                return false;
            }
#endif

            Core.Logger.Msg("Interop Generation Complete!");
            return true;
        }
    }
}
