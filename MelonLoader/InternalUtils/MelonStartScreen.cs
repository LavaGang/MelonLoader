using System.IO;
using MelonLoader.Modules;
using UnityVersion = AssetRipper.VersionUtilities.UnityVersion;

namespace MelonLoader.InternalUtils
{
    internal class MelonStartScreen
    {
        // Doesn't support Unity versions lower than 2017.2.0 (yet?)
        // Doesn't support Unity versions lower than 2018 (Crashing Issue)
        // Doesn't support Unity versions higher than to 2020.3.21 (Crashing Issue)
        internal static readonly MelonModule.Info moduleInfo = new MelonModule.Info($"MelonLoader{Path.DirectorySeparatorChar}Dependencies{Path.DirectorySeparatorChar}MelonStartScreen.dll"
            , () => !MelonLaunchOptions.Core.StartScreen || UnityInformationHandler.EngineVersion < new UnityVersion(2018));

        internal static int LoadAndRun(LemonFunc<int> functionToWaitForAsync)
        {
            var module = MelonModule.Load(moduleInfo);
            if (module == null)
                return functionToWaitForAsync();

            var result = module.SendMessage("LoadAndRun", functionToWaitForAsync);
            if (result is int resultCode)
                return resultCode;

            return -1;
        }
    }
}
